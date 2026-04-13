using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    
    private Coroutine autoAdvanceCoroutine;
    private Queue<string> dailyMessageQueue = new Queue<string>();
    private Queue<string> currentVoiceLogs = new Queue<string>(); 
    
    [Header("关卡管理")]
    public int currentDayIndex = 1; 
    public string currentDay => "Day " + currentDayIndex;

    [Header("UI：开场系统")]
    public GameObject panelDesktop; 
    public GameObject panelEmail;   

    [Header("UI：工作台系统")]
    public GameObject topBar;
    public GameObject middleDisplay;
    public GameObject centerZone;

    [Header("UI：下班结算系统")]
    public GameObject panelEndOfDay;      
    public GameObject btnNextLine;        
    public GameObject btnNextDay;         
    public TextMeshProUGUI txtVoiceLog;   

    /* ========== 新增：打字机核心变量 ========== */
    [Header("打字机设置")]
    public float typingSpeed = 0.05f; 
    
    private bool isTyping = false;          
    private string currentLineText = "";    
    private Coroutine typingCoroutine;      
    /* ========================================= */

    void Awake() { Instance = this; }

    void Start()
    {
        if (panelDesktop != null) panelDesktop.SetActive(true);
        if (panelEmail != null) panelEmail.SetActive(false);
        if (panelEndOfDay != null) panelEndOfDay.SetActive(false);

        if (topBar != null) topBar.SetActive(false);
        if (middleDisplay != null) middleDisplay.SetActive(false);
        if (centerZone != null) centerZone.SetActive(false);

        // ================= 核心修复：把丢失的背景音乐加回来！ =================
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgm_Gameplay);
        }
        // ====================================================================
    }

    public void OpenEmail() { if (panelEmail != null) panelEmail.SetActive(true); }

    public void CloseEmailAndStartDay()
    {
        if (panelEmail != null) panelEmail.SetActive(false);
        if (panelDesktop != null) panelDesktop.SetActive(false); 

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
        
        // 每日开工强制开锁（防连点Bug终极保险）
        if (MessageSender.Instance != null) {
            MessageSender.Instance.UnlockSending();
        }
        
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

            // ================= 波形图演出逻辑 =================
            if (src == WordBlockManager.MessageSource.Alien)
            {
                int wordCount = data.words.Length;
                float waveDuration = Mathf.Clamp(wordCount * 0.2f, 0.5f, 2.0f);

                if (WaveformEffect.Instance != null)
                {
                    WaveformEffect.Instance.PlayWave(waveDuration, () => 
                    {
                        WordBlockManager.Instance.SetupNewTurn(sentence, src, data.id);
                        if (ButtonVisualManager.Instance != null) {
                            ButtonVisualManager.Instance.UpdateButtons(src);
                        }
                        if (MessageSender.Instance != null)
                        {
                            MessageSender.Instance.UnlockSending();
                        }
                    });
                }
                else
                {
                    WordBlockManager.Instance.SetupNewTurn(sentence, src, data.id);
                    if (ButtonVisualManager.Instance != null) ButtonVisualManager.Instance.UpdateButtons(src);
            
                    if (MessageSender.Instance != null) MessageSender.Instance.UnlockSending();
                }
            }
            else
            {
                WordBlockManager.Instance.SetupNewTurn(sentence, src, data.id);
                if (ButtonVisualManager.Instance != null) {
                    ButtonVisualManager.Instance.UpdateButtons(src);
                }
                if (MessageSender.Instance != null) MessageSender.Instance.UnlockSending();
            }
            // =========================================================
        }
        else
        {
            StartVoiceLogSequence();
        }
    }

    private void StartVoiceLogSequence()
    {
        if (currentDayIndex >= 3)
        {
            string finalEnding = "End_H"; 

            int p = GameManager.Instance.panic;
            int a = GameManager.Instance.arrogance;
            int f = GameManager.Instance.friendliness;

            if (f >= 75 && p >= 40 && p <= 70 && a >= 40 && a <= 70) {
                finalEnding = "End_E";
            }
            else if (f >= 50 && a <= 35) {
                finalEnding = "End_F";
            }
            else if (f < 50 && (p >= 65 || a >= 65)) {
                finalEnding = "End_G";
            }

            GameManager.Instance.TriggerEnding(finalEnding);
            return;
        }

        if (panelEndOfDay != null) panelEndOfDay.SetActive(true);
        if (btnNextDay != null) btnNextDay.SetActive(false); 
        if (btnNextLine != null) btnNextLine.SetActive(true); 

        currentVoiceLogs.Clear();
        int acc = GameManager.Instance.accuracy;
        
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

        if (currentDayIndex == 1)
        {
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

        currentVoiceLogs.Enqueue("【语音通讯已结束。】");

        PlayNextVoiceLog();
    }

    public void PlayNextVoiceLog()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            txtVoiceLog.text = currentLineText;
            isTyping = false;
            
            CheckForAutoAdvance(currentLineText);
            return; 
        }

        if (currentVoiceLogs.Count > 0)
        {
            currentLineText = currentVoiceLogs.Dequeue();

            if (currentLineText.Contains("接入中") && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx_Call);
            }

            typingCoroutine = StartCoroutine(TypeText(currentLineText));
        }
        else
        {
            txtVoiceLog.text = "【通讯已切断】";
            if (btnNextLine != null) btnNextLine.SetActive(false); 
            if (btnNextDay != null) btnNextDay.SetActive(true);    
        }
    }

    private IEnumerator TypeText(string lineToType)
    {
        isTyping = true;
        txtVoiceLog.text = ""; 

        foreach (char letter in lineToType.ToCharArray())
        {
            txtVoiceLog.text += letter;
            yield return new WaitForSeconds(typingSpeed); 
        }

        isTyping = false;
        CheckForAutoAdvance(lineToType);
    }

    private void CheckForAutoAdvance(string line)
    {
        if (line.Contains("【") && line.Contains("】"))
        {
            if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = StartCoroutine(AutoAdvanceVoiceLog());
        }
    }

    private IEnumerator AutoAdvanceVoiceLog()
    {
        yield return new WaitForSeconds(2f);
        PlayNextVoiceLog(); 
    }

    public void GoToNextDay()
    {
        if (panelEndOfDay != null) panelEndOfDay.SetActive(false);

        if (GameManager.Instance.accuracy >= 60) {
            GameManager.Instance.petState++;
        } else {
            GameManager.Instance.petState--;
        }

        if (GameManager.Instance.petState > 3) GameManager.Instance.petState = 3;
        if (GameManager.Instance.petState < 0) GameManager.Instance.petState = 0;
        
        if (PetManager.Instance != null) {
            PetManager.Instance.UpdatePetAnimation();
        }
        
        if (GameManager.Instance.CheckInstantDeath() == true) 
        {
            return; 
        }

        GameManager.Instance.ResetDailyStats(); 
        currentDayIndex++; 

        StartDay();
    }
}