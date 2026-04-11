using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBlockManager : MonoBehaviour
{
    public static WordBlockManager Instance; // 设为单例，方便词块随时呼叫它

    [Header("模具与容器")]
    public GameObject wordBlockPrefab;
    public Transform rightSideContainer; // 外星人区 (源)
    public Transform leftSideContainer;  // 地球区 (目标)

    // 极其重要：用一个列表把外星人区的词块按生成顺序全部装起来
    private List<WordBlock> alienBlocks = new List<WordBlock>();

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        SpawnTestSentence();
    }

    void SpawnTestSentence()
    {
        string[] testWords = { "我们", "的", "首领", "想", "见", "你" };

        foreach (string word in testWords)
        {
            GameObject newBlock = Instantiate(wordBlockPrefab, rightSideContainer);
            
            TextMeshProUGUI textComp = newBlock.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null) textComp.text = word;

            // 把每个生成出来的词块脚本，按顺序塞进列表里记住
            WordBlock blockScript = newBlock.GetComponent<WordBlock>();
            if (blockScript != null)
            {
                alienBlocks.Add(blockScript);
            }
        }
    }

    // 核心算法：每次有词块被点击，就重新刷新一次地球区的显示
    public void RebuildEarthSentence()
    {
        // 1. 拆除地球区现有的所有旧词块（清空画板）
        foreach (Transform child in leftSideContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 按照我们在列表里记住的“原始顺序”，逐个检查外星人区的词块
        foreach (WordBlock block in alienBlocks)
        {
            // 3. 如果这个词块处于“被选中”状态
            if (block.isSelected == true)
            {
                // 4. 就在地球区克隆一个一模一样的外壳
                GameObject cloneBlock = Instantiate(wordBlockPrefab, leftSideContainer);
                
                // 把字原封不动抄过来
                cloneBlock.GetComponentInChildren<TextMeshProUGUI>().text = block.GetComponentInChildren<TextMeshProUGUI>().text;

                // 【精髓】：剥夺克隆体的交互能力！把它身上的 Button 和 WordBlock 脚本全删了
                // 让它变成一块只能看、不能点的“死木头”，彻底断绝套娃 Bug！
                Destroy(cloneBlock.GetComponent<UnityEngine.UI.Button>());
                Destroy(cloneBlock.GetComponent<WordBlock>());
            }
        }
    }
}