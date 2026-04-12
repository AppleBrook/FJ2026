using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    private Queue<string> dailyMessageQueue = new Queue<string>();
    private Queue<string> currentVoiceLogs = new Queue<string>(); // 【新增】用来排队播放语音文本
    
    [Header("关卡管理")]
    public int currentDayIndex = 1; 
    public string currentDay => "Day " + currentDayIndex;

    [Header("UI：开场系统")]
    public GameObject panelDesktop; 
    public GameObject panelEmail;   

    [Header("UI：工作台系统 (需要隐藏的部分)")]
    public GameObject topBar;
    public GameObject middleDisplay;
    public GameObject centerZone;

    [Header("UI：下班结算系统")]
    public GameObject panelEndOfDay;      
    public GameObject btnNextLine;        
    public GameObject btnNextDay;         
    public TextMeshProUGUI txtVoiceLog;   

    void Awake() { Instance = this; }

    void Start()
    {
        // 1. 游戏刚启动：只显示桌面。隐藏邮件、下班面板
        if (panelDesktop != null) panelDesktop.SetActive(true);
        if (panelEmail != null) panelEmail.SetActive(false);
        if (panelEndOfDay != null) panelEndOfDay.SetActive(false);

        // 2. 游戏刚启动时，把冷冰冰的工作台全藏起来！
        if (topBar != null) topBar.SetActive(false);
        if (middleDisplay != null) middleDisplay.SetActive(false);
        if (centerZone != null) centerZone.SetActive(false);
    }

    public void OpenEmail() { if (panelEmail != null) panelEmail.SetActive(true); }

    public void CloseEmailAndStartDay()
    {
        if (panelEmail != null) panelEmail.SetActive(false);
        if (panelDesktop != null) panelDesktop.SetActive(false); 

        // 看完邮件，点击“开始工作”后，瞬间唤醒工作台！
        if (topBar != null) topBar.SetActive(true);
        if (middleDisplay != null) middleDisplay.SetActive(true);
        if (centerZone != null) centerZone.SetActive(true);

        StartDay(); 
    }

    public void StartDay()
    {
        dailyMessageQueue.Clear();
        var todaysMessages = CSVImporter.Instance.allMessages.Where(m => m.level == currentDay).ToList();
        var uniqueIDs = todaysMessages.Select(m => m.id).Distinct().ToList();

        foreach (string id in uniqueIDs) dailyMessageQueue.Enqueue(id);

        Debug.Log($"--- {currentDay} 开始工作！---");
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

            /* 新增：呼叫视觉管理器，让它把不要的按钮塞到后面去！*/
            if (ButtonVisualManager.Instance != null) {
                ButtonVisualManager.Instance.UpdateButtons(src);
            }
        }
        else
        {
            // 队列空了，进入下班语音环节！
            StartVoiceLogSequence();
        }
    }

    // ================== 下班结算演出逻辑 ==================
    private void StartVoiceLogSequence()
    {
        // ================= 第二类：第三天结算结局（熬过三天后触发） =================
        if (currentDayIndex >= 3)
        {
            // 【已替换】默认结局 H：庸人的幸存
            string finalEnding = "End_H"; 

            int p = GameManager.Instance.panic;
            int a = GameManager.Instance.arrogance;
            int f = GameManager.Instance.friendliness;

            // 结局 E：【共荣】
            if (p >= 50 && p < 70 && a >= 50 && a < 70 && f >= 80) {
                finalEnding = "End_E";
            }
            // 结局 F：【黄金笼中鸟】
            else if (p < 50 && a <= 30 && f >= 60) {
                finalEnding = "End_F";
            }
            // 结局 G：【永夜铁幕】
            else if ((a >= 70 || p >= 70) && f < 40) {
                finalEnding = "End_G";
            }

            // 直接触发结局大清算
            GameManager.Instance.TriggerEnding(finalEnding);
            return;
        }

        // --- 否则，打开平时的下班结算面板 ---
        if (panelEndOfDay != null) panelEndOfDay.SetActive(true);
        if (btnNextDay != null) btnNextDay.SetActive(false); 
        if (btnNextLine != null) btnNextLine.SetActive(true); 

        currentVoiceLogs.Clear();
        int acc = GameManager.Instance.accuracy;
        
        // ==========================================
        // 第一部分：长官通讯 (每天固定先播长官)
        // ==========================================
        currentVoiceLogs.Enqueue("【……语音通讯接入中……】");
        currentVoiceLogs.Enqueue("【已接入：地球联合军指挥部】");

        if (acc >= 60) {
            currentVoiceLogs.Enqueue("你的工作完成得非常完美，\n地球向你致以最高的敬意。");
        } else if (acc >= 30) {
            currentVoiceLogs.Enqueue("你的工作完成得马马虎虎，\n地球希望你能再接再厉。");
        } else {
            currentVoiceLogs.Enqueue("你的工作完成得非常糟糕!\n地球失望地看着你和你的家人。");
        }
        currentVoiceLogs.Enqueue("【语音通讯已结束。】");


        // ==========================================
        // 第二部分：家人通讯 (根据天数区分妻子/女儿)
        // ==========================================
        if (currentDayIndex == 1)
        {
            // ---------- Day 1: 妻子 ----------
            currentVoiceLogs.Enqueue("【……语音通讯接入中……】");
            currentVoiceLogs.Enqueue("【已接入：妻子】");

            if (acc >= 60) {
                currentVoiceLogs.Enqueue("亲爱的，你太厉害了。");
                currentVoiceLogs.Enqueue("今天的广播里全是你的名字，\n听说你做的非常好。");
                currentVoiceLogs.Enqueue("希望你注意休息，我们等你回家。");
            } else if (acc >= 30) {
                currentVoiceLogs.Enqueue("亲爱的，\n今天遇到什么烦心事了吗？");
                currentVoiceLogs.Enqueue("不、不用担心我们，专心你的工作吧。");
                currentVoiceLogs.Enqueue("我们等你回家。");
            } else {
                currentVoiceLogs.Enqueue("………………");
                currentVoiceLogs.Enqueue("哦，哦，不好意思，只是挨了一顿饿，你呢？");
                currentVoiceLogs.Enqueue("有没有好好吃饭？");
                currentVoiceLogs.Enqueue("我们等你回家。");
            }
        }
        else if (currentDayIndex == 2)
        {
            // ---------- Day 2: 女儿 ----------
            currentVoiceLogs.Enqueue("【……语音通讯接入中……】");
            currentVoiceLogs.Enqueue("【已接入：宝贝】");

            if (acc >= 60) {
                currentVoiceLogs.Enqueue("妈妈，今天有叔叔阿姨送了我礼物。");
                currentVoiceLogs.Enqueue("他们说你是全地球最厉害的人，正在和外星人讲故事。我好崇拜你呀！");
                currentVoiceLogs.Enqueue("你什么时候可以回来给我讲故事呢？");
            } else if (acc >= 30) {
                currentVoiceLogs.Enqueue("妈妈，今天学校放假了，老师的表情好严肃。");
                currentVoiceLogs.Enqueue("妈咪说只要你在努力工作，大家就都会没事的。");
                currentVoiceLogs.Enqueue("我给你画了一幅画，等你回来就给你看。");
            } else {
                currentVoiceLogs.Enqueue("刚才窗户外面好响，妈咪抱紧我不让我看，她说那是烟花。");
                currentVoiceLogs.Enqueue("妈妈快回来好不好。");
                currentVoiceLogs.Enqueue("你现在就回来。我好想你。");
            }
        }

        // 统一收尾
        currentVoiceLogs.Enqueue("【语音通讯已结束。】");

        // 4. 开始播放第一句话
        PlayNextVoiceLog();
    }

    // 每次点击屏幕，播放下一句话
    public void PlayNextVoiceLog()
    {
        if (currentVoiceLogs.Count > 0)
        {
            txtVoiceLog.text = currentVoiceLogs.Dequeue();
        }
        else
        {
            // 没话说了，弹出“进入下一天”按钮
            txtVoiceLog.text = "【通讯已切断】";
            if (btnNextLine != null) btnNextLine.SetActive(false); // 关掉点击
            if (btnNextDay != null) btnNextDay.SetActive(true);    // 显示下一天按钮
        }
    }

    // 点击“进入下一天”按钮
    public void GoToNextDay()
    {
        if (panelEndOfDay != null) panelEndOfDay.SetActive(false);

        // 严格按照“>60加1，反之减1”的规则更新宠物状态
        if (GameManager.Instance.accuracy >= 60) {
            GameManager.Instance.petState++;
        } else {
            GameManager.Instance.petState--;
        }

        // 保证宠物状态最低不小于0
        if (GameManager.Instance.petState < 0) GameManager.Instance.petState = 0;
        
        // 宠物状态更新后，检测一下会不会引发宠物死亡结局（结局D）
        GameManager.Instance.CheckInstantDeath();

        // 重置血条等状态，天数推进
        GameManager.Instance.ResetDailyStats(); 
        currentDayIndex++; 

        // 不再回桌面，直接开始第二天/第三天的新流水线！
        StartDay();
    }
}