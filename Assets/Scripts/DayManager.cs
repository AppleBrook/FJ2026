using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    // 这是一个先进先出的排队队列
    private Queue<string> dailyMessageQueue = new Queue<string>();
    
    [Header("当前关卡")]
    public string currentDay = "Day 1"; // 必须和 CSV 表格里的第二列拼写一模一样

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 延迟 0.5 秒启动，确保表格数据已经完全加载进内存
        Invoke("StartDay", 0.5f);
    }

    public void StartDay()
    {
        dailyMessageQueue.Clear();

        // 1. 把表格里属于 "Day 1" 的所有行抓出来
        var todaysMessages = CSVImporter.Instance.allMessages.Where(m => m.level == currentDay).ToList();

        // 2. 核心逻辑：提取不重复的 ID！(防止同一句话因为有多个规则而重复出现)
        var uniqueIDs = todaysMessages.Select(m => m.id).Distinct().ToList();

        // 3. 把这些唯一的 ID 按顺序塞进队列
        foreach (string id in uniqueIDs)
        {
            dailyMessageQueue.Enqueue(id);
        }

        Debug.Log($"<color=cyan>--- {currentDay} 开始工作！今日共有 {dailyMessageQueue.Count} 封电报 ---</color>");

        // 4. 立刻发出今天的第一句话
        LoadNextSentence();
    }

    public void LoadNextSentence()
    {
        if (dailyMessageQueue.Count > 0)
        {
            // 从队列最前面拿出一个 ID，并且把它从队列里删掉 (出队)
            string nextID = dailyMessageQueue.Dequeue();

            // 去表格里随便找一条这个 ID 的规则，因为我们只需要它的 original sentence 和 source
            MessageData data = CSVImporter.Instance.GetRulesByID(nextID)[0];

            string sentence = string.Join(" ", data.words);
            WordBlockManager.MessageSource src = data.source == "外星人" ? 
                WordBlockManager.MessageSource.Alien : WordBlockManager.MessageSource.Earth;

            // 让 WordBlockManager 把这句话刷到屏幕上
            WordBlockManager.Instance.SetupNewTurn(sentence, src, data.id);
        }
        else
        {
            // 队列空了，代表下班！
            Debug.Log("<color=yellow>今天的电报处理完毕！准备进入结算环节...</color>");
            // 下一步我们会在这里接上 宠物结算和过场画面
        }
    }
}