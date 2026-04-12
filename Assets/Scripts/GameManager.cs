using UnityEngine;
using UnityEngine.SceneManagement; 

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
    public void CheckInstantDeath()
    {
        if (panic >= 100) {
            TriggerEnding("End_A"); // 结局 A：【疯狂】
        }
        else if (arrogance >= 100) {
            TriggerEnding("End_B"); // 结局 B：【傲慢与偏见】
        }
        else if (friendliness <= 0) {
            TriggerEnding("End_C"); // 结局 C：【无效杂音】
        }
        else if (petState <= 0) {
            TriggerEnding("End_D"); // 结局 D：【今夜你不关心人类】（植物枯萎）
        }
    }

    public void ResetDailyStats()
    {
        /* 核心修改：每天开始时准确率重置为 35 */
        accuracy = 35; 
    }

    public void TriggerEnding(string endingID)
    {
        Debug.Log($"<color=red>★★★ 触发结局：{endingID} ★★★</color>");
        
        GlobalData.currentEndingID = endingID;
        SceneManager.LoadScene("EndingScene");
    }
}