using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBlockManager : MonoBehaviour
{
    public static WordBlockManager Instance;
    public string currentSentenceID;

    // 定义来源枚举
    public enum MessageSource { Earth, Alien }
    [Header("当前回合状态")]
    public MessageSource currentSource; 

    [Header("模具与插槽")]
    public GameObject wordBlockPrefab;
    public Transform leftSideContainer;  // 地球区
    public Transform rightSideContainer; // 外星区
    
    [Header("发送按钮")]
    public Button btnSendToEarth;
    public Button btnSendToAlien;

    private List<WordBlock> sourceBlocks = new List<WordBlock>();
    private Transform currentSourceContainer;
    public Transform currentTargetContainer;

    void Awake() { Instance = this; }

    // 核心函数：设置新的一轮对话
    public void SetupNewTurn(string sentence, MessageSource source, string id)
    {
        currentSource = source;
        currentSentenceID = id;
        
        // 1. 清空两侧所有旧词块
        ClearContainer(leftSideContainer);
        ClearContainer(rightSideContainer);
        sourceBlocks.Clear();

        // 2. 判定谁是“源”，谁是“目标”
        if (source == MessageSource.Alien) {
            currentSourceContainer = rightSideContainer;
            currentTargetContainer = leftSideContainer;
            btnSendToEarth.interactable = true;  // 只能发给地球
            btnSendToAlien.interactable = false;
        } else {
            currentSourceContainer = leftSideContainer;
            currentTargetContainer = rightSideContainer;
            btnSendToEarth.interactable = false;
            btnSendToAlien.interactable = true; // 只能发给外星人
        }

        // 3. 生成原始词块
        string[] words = sentence.Split(' ');
        foreach (string w in words) {
            GameObject newBlock = Instantiate(wordBlockPrefab, currentSourceContainer);
            newBlock.GetComponentInChildren<TextMeshProUGUI>().text = w;
            WordBlock script = newBlock.GetComponent<WordBlock>();
            sourceBlocks.Add(script);
        }
    }

    private void ClearContainer(Transform container) {
        foreach (Transform child in container) Destroy(child.gameObject);
    }

    // 当词块被点击时，由 WordBlock 脚本呼叫
    public void RebuildTargetSentence()
    {
        ClearContainer(currentTargetContainer);

        foreach (WordBlock block in sourceBlocks) {
            if (block.isSelected) {
                GameObject clone = Instantiate(wordBlockPrefab, currentTargetContainer);
                clone.GetComponentInChildren<TextMeshProUGUI>().text = block.GetComponentInChildren<TextMeshProUGUI>().text;
                
                // 禁用克隆体的交互
                Destroy(clone.GetComponent<Button>());
                Destroy(clone.GetComponent<WordBlock>());
            }
        }
    }
    
    // 用来统计现在选了几个词
    public int GetSelectedWordCount()
    {
        int count = 0;
        foreach (var block in sourceBlocks) {
            if (block.isSelected) count++;
        }
        return count;
    }
}