using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;          // 新增：为了让系统认识 Image 组件
using System.Collections;      // 新增：为了让系统认识 IEnumerator 协程

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("核心数值")]
    public int panic = 50;
    public int arrogance = 50;
    public int friendliness = 50;
    /* 核心修改：每日初始准确率降为 35，增加难度与压迫感 */
    public int accuracy = 35; 
    public int petState = 2; // 初始状态为2（正常）
    
    [Header("死亡演出设定")]
    public Image deathFadeImage; // 拖入刚刚建好的 Img_DeathFade
    public float fadeDuration = 1.5f; // 黑幕渐变的时长

    void Awake() { Instance = this; }

    public void ApplyStatChanges(int pChange, int aChange, int fChange, int accChange)
    {
        panic += pChange;
        arrogance += aChange;
        friendliness += fChange;
        accuracy += accChange;
        
        // 每次发完消息数值变动后，立刻检测是否暴毙！
        CheckInstantDeath(); 
    }

    // ================= 第一类：中途暴毙结局（随时触发） =================
    public bool CheckInstantDeath()
    {
        if (panic >= 100) {
            TriggerEnding("End_A"); 
            return true; // 触发结局后，返回 true
        }
        else if (arrogance >= 100) {
            TriggerEnding("End_B"); 
            return true;
        }
        else if (friendliness <= 0) {
            TriggerEnding("End_C"); 
            return true;
        }
        else if (petState <= 0) {
            TriggerEnding("End_D"); 
            return true;
        }
        
        return false; // 如果活得好好的，返回 false
    }

    public void ResetDailyStats()
    {
        /* 核心修改：每天开始时准确率重置为 35 */
        accuracy = 35; 
    }

    public void TriggerEnding(string endingID)
    {
        GlobalData.currentEndingID = endingID;
        
        // 判断是不是暴毙结局（A/B/C/D）。如果是，并且绑定了黑幕，就执行死亡演出
        if ((endingID == "End_A" || endingID == "End_B" || endingID == "End_C" || endingID == "End_D") 
            && deathFadeImage != null)
        {
            StartCoroutine(DeathFadeAndLoad());
        }
        else
        {
            // 如果是正常结局（E/F/G/H），或者是测试忘了绑图，就直接跳，不墨迹
            SceneManager.LoadScene("EndingScene");
        }
    }

    // 新增：死亡黑幕渐渐变暗的协程
    private IEnumerator DeathFadeAndLoad()
    {
        // 1. 确保黑幕是激活状态
        deathFadeImage.gameObject.SetActive(true);
        Color c = deathFadeImage.color;
        
        // 2. 停掉日常 BGM，可以提前放点心跳声或者寂静的音效（可选）
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();

        // 3. 开始渐变
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            // 计算当前的透明度（从 0 慢慢变成 1）
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            deathFadeImage.color = c;
            
            // 等待下一帧继续
            yield return null; 
        }

        // 4. 彻底全黑后，再跳转到结局场景
        SceneManager.LoadScene("EndingScene");
    }
}