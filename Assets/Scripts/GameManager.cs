using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("核心数值")]
    public int panic = 50;
    public int arrogance = 50;
    public int friendliness = 50;
    public int accuracy = 50;
    public int petState = 2; // 初始状态为2（正常）

    void Awake() { Instance = this; }

    // 把名字改成了 ApplyStatChanges，解决 MessageSender 的报错
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
    public void CheckInstantDeath()
    {
        if (panic >= 100) {
            TriggerEnding("End_A"); // 结局 A：【疯狂】（恐慌爆表）
        }
        else if (arrogance >= 100) {
            TriggerEnding("End_B"); // 结局 B：【傲慢与偏见】（傲慢爆表）
        }
        else if (friendliness <= 0) {
            TriggerEnding("End_C"); // 结局 C：【无效杂音】（友好度归零）
        }
        else if (petState <= 0) {
            TriggerEnding("End_D"); // 结局 D：【今夜你不关心人类】（宠物死亡）
        }
    }

    public void ResetDailyStats()
    {
        accuracy = 50; // 每天开始时准确率归位
    }

    public void TriggerEnding(string endingID)
    {
        Debug.Log($"<color=red>★★★ 触发结局：{endingID} ★★★</color>");
        
        // 把结局ID存入超脱三界之外的 GlobalData
        GlobalData.currentEndingID = endingID;
        
        // 强行跳转到结局场景！
        SceneManager.LoadScene("EndingScene");
    }
}