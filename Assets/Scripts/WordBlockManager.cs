using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBlockManager : MonoBehaviour
{
    public static WordBlockManager Instance;
    public string currentSentenceID;

    public enum MessageSource { Earth, Alien }
    [Header("当前回合状态")]
    public MessageSource currentSource; 

    [Header("模具与插槽")]
    public GameObject wordBlockPrefab;
    public Transform leftSideContainer;  
    public Transform rightSideContainer; 
    
    [Header("发送按钮")]
    public Button btnSendToEarth;
    public Button btnSendToAlien;

    [Header("阵营美术图：地球")]
    public Sprite earthNormalSprite;
    public Sprite earthSelectedSprite;
    public Color earthTextColor = Color.black; 

    [Header("阵营美术图：外星")]
    public Sprite alienNormalSprite;
    public Sprite alienSelectedSprite;
    public Color alienTextColor = Color.white; 

    private List<WordBlock> sourceBlocks = new List<WordBlock>();
    private Transform currentSourceContainer;
    public Transform currentTargetContainer;

    void Awake() { Instance = this; }

    public void SetupNewTurn(string sentence, MessageSource source, string id)
    {
        currentSource = source;
        currentSentenceID = id;
        
        ClearContainer(leftSideContainer);
        ClearContainer(rightSideContainer);
        sourceBlocks.Clear();

        if (source == MessageSource.Alien) {
            currentSourceContainer = rightSideContainer;
            currentTargetContainer = leftSideContainer;
            btnSendToEarth.interactable = true;  
            btnSendToAlien.interactable = false;
        } else {
            currentSourceContainer = leftSideContainer;
            currentTargetContainer = rightSideContainer;
            btnSendToEarth.interactable = false;
            btnSendToAlien.interactable = true; 
        }

        Sprite srcNormal = (source == MessageSource.Earth) ? earthNormalSprite : alienNormalSprite;
        Sprite srcSelected = (source == MessageSource.Earth) ? earthSelectedSprite : alienSelectedSprite;
        Color srcTextColor = (source == MessageSource.Earth) ? earthTextColor : alienTextColor;

        string[] words = sentence.Split(' ');
        foreach (string w in words) {
            GameObject newBlock = Instantiate(wordBlockPrefab, currentSourceContainer);
            WordBlock script = newBlock.GetComponent<WordBlock>();
            
            script.Initialize(w, srcNormal, srcSelected, srcTextColor); 
            sourceBlocks.Add(script);

            /* ===== 新增：生成词块时，给它绑定“点击音效” ===== */
            Button btn = newBlock.GetComponent<Button>();
            if (btn != null) {
                btn.onClick.AddListener(() => {
                    if (AudioManager.Instance != null) 
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.ui_Click);
                });
            }
            /* ================================================== */
        }
    }

    private void ClearContainer(Transform container) {
        foreach (Transform child in container) Destroy(child.gameObject);
    }

    public void RebuildTargetSentence()
    {
        ClearContainer(currentTargetContainer);

        Sprite targetNormal = (currentSource == MessageSource.Earth) ? alienNormalSprite : earthNormalSprite;
        Color targetTextColor = (currentSource == MessageSource.Earth) ? alienTextColor : earthTextColor;

        foreach (WordBlock block in sourceBlocks) {
            if (block.isSelected) {
                GameObject clone = Instantiate(wordBlockPrefab, currentTargetContainer);
                
                TextMeshProUGUI cloneText = clone.GetComponentInChildren<TextMeshProUGUI>();
                cloneText.text = block.word;
                cloneText.color = targetTextColor;
                
                Image cloneImg = clone.GetComponent<Image>();
                if (cloneImg != null && targetNormal != null) {
                    cloneImg.sprite = targetNormal;
                }
                
                /* 新增核心逻辑：配置克隆体的遥控器 */
                
                // 1. 克隆体不再需要原本的 WordBlock 脚本了，把它删掉以防冲突
                Destroy(clone.GetComponent<WordBlock>());
                
                // 2. 给克隆体加上我们新写的遥控器脚本
                CloneBlock cloneScript = clone.AddComponent<CloneBlock>();
                
                // 3. 把本体的引用塞给遥控器，让它知道自己该控制谁
                cloneScript.originalBlock = block;
                
                // 4. 拿到克隆体身上的按钮组件，让它在被点击时呼叫遥控器
                Button cloneBtn = clone.GetComponent<Button>();
                cloneBtn.onClick.RemoveAllListeners(); 
                cloneBtn.onClick.AddListener(() => {
                    /* ===== 新增：点击目标区词块时，播放“退回音效” ===== */
                    if (AudioManager.Instance != null) 
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.ui_Back);
                    /* ================================================== */
                    
                    cloneScript.OnCloneClicked();
                });
            }
        }
    }
    
    public int GetSelectedWordCount()
    {
        int count = 0;
        foreach (var block in sourceBlocks) {
            if (block.isSelected) count++;
        }
        return count;
    }
}