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
    /* ========== 新增：地球文字颜色 ========== */
    public Color earthTextColor = Color.black; 

    [Header("阵营美术图：外星")]
    public Sprite alienNormalSprite;
    public Sprite alienSelectedSprite;
    /* ========== 新增：外星文字颜色 ========== */
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
        
        /* 新增：判断源头该用什么文字颜色 */
        Color srcTextColor = (source == MessageSource.Earth) ? earthTextColor : alienTextColor;

        string[] words = sentence.Split(' ');
        foreach (string w in words) {
            GameObject newBlock = Instantiate(wordBlockPrefab, currentSourceContainer);
            WordBlock script = newBlock.GetComponent<WordBlock>();
            
            /* 修改：把文字颜色也传进去！ */
            script.Initialize(w, srcNormal, srcSelected, srcTextColor); 
            
            sourceBlocks.Add(script);
        }
    }

    private void ClearContainer(Transform container) {
        foreach (Transform child in container) Destroy(child.gameObject);
    }

    public void RebuildTargetSentence()
    {
        ClearContainer(currentTargetContainer);

        Sprite targetNormal = (currentSource == MessageSource.Earth) ? alienNormalSprite : earthNormalSprite;
        
        /* 新增：判断目标区域（翻译区）该用什么文字颜色 */
        Color targetTextColor = (currentSource == MessageSource.Earth) ? alienTextColor : earthTextColor;

        foreach (WordBlock block in sourceBlocks) {
            if (block.isSelected) {
                GameObject clone = Instantiate(wordBlockPrefab, currentTargetContainer);
                
                TextMeshProUGUI cloneText = clone.GetComponentInChildren<TextMeshProUGUI>();
                cloneText.text = block.word;
                /* 新增：把克隆出来的文字颜色也改掉！ */
                cloneText.color = targetTextColor;
                
                Image cloneImg = clone.GetComponent<Image>();
                if (cloneImg != null && targetNormal != null) {
                    cloneImg.sprite = targetNormal;
                }
                
                Destroy(clone.GetComponent<Button>());
                Destroy(clone.GetComponent<WordBlock>());
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