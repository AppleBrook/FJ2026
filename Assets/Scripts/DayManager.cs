using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    private Queue<string> dailyMessageQueue = new Queue<string>();
    
    [Header("关卡管理")]
    public string currentDay = "Day 1"; 

    [Header("开场 UI 引用")]
    public GameObject panelDesktop; // 电脑桌面层
    public GameObject panelEmail;   // 邮件弹窗层

    void Awake() { Instance = this; }

    void Start()
    {
        // 游戏刚运行，处于等待状态：显示桌面，隐藏邮件弹窗，盖住下方工作台
        if (panelDesktop != null) panelDesktop.SetActive(true);
        if (panelEmail != null) panelEmail.SetActive(false);
    }

    // 玩家点击桌面“邮件图标”时呼叫这个函数
    public void OpenEmail()
    {
        if (panelEmail != null) panelEmail.SetActive(true);
    }

    // 玩家阅读完毕，点击“开始工作”时呼叫这个函数
    public void CloseEmailAndStartDay()
    {
        if (panelEmail != null) panelEmail.SetActive(false);
        if (panelDesktop != null) panelDesktop.SetActive(false); // 撤掉桌面，露出工作台！
        
        StartDay(); // 正式启动发牌流水线
    }

    public void StartDay()
    {
        dailyMessageQueue.Clear();

        var todaysMessages = CSVImporter.Instance.allMessages.Where(m => m.level == currentDay).ToList();
        var uniqueIDs = todaysMessages.Select(m => m.id).Distinct().ToList();

        foreach (string id in uniqueIDs) dailyMessageQueue.Enqueue(id);

        Debug.Log($"<color=cyan>--- {currentDay} 开始工作！今日共有 {dailyMessageQueue.Count} 封电报 ---</color>");
        LoadNextSentence();
    }

    public void LoadNextSentence()
    {
        if (dailyMessageQueue.Count > 0)
        {
            string nextID = dailyMessageQueue.Dequeue();
            MessageData data = CSVImporter.Instance.GetRulesByID(nextID)[0];

            string sentence = string.Join(" ", data.words);
            WordBlockManager.MessageSource src = data.source == "外星人" ? 
                WordBlockManager.MessageSource.Alien : WordBlockManager.MessageSource.Earth;

            WordBlockManager.Instance.SetupNewTurn(sentence, src, data.id);
        }
        else
        {
            Debug.Log("<color=yellow>今天的电报处理完毕！准备进入下班结算...</color>");
            // 留空：等确认这个开场没问题，我们再接 任务 3 的结算逻辑
        }
    }
}