using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* 这是一个单例，方便我们在其他脚本里直接找到这个大管家 */
    public static GameManager Instance;

    /* 地球恐慌度 */
    public int panic = 50;

    /* 地球傲慢度 */
    public int arrogance = 50;

    /* 外星友好度 */
    public int friendliness = 50;

    /* 每日准确率 */
    public int accuracy = 50;

    /* 宠物状态：0是死亡，5是超级好 */
    public int petState = 3;
    
    // 在 GameManager.cs 里面找个空位加入这个函数
    
    void Awake()
    {
        /* 确保游戏里始终只有一个大管家，不要有替身 */
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /* 这是一个通用的加减分方法，每次电报发送后都会调用它 */
    public void ApplyStatChanges(int panicChange, int arroganceChange, int friendlinessChange, int accuracyChange)
    {
        /* Mathf.Clamp 就像一个坚固的栅栏，确保我们的数值加减之后，永远被困在 0 到 100 之间 */
        panic = Mathf.Clamp(panic + panicChange, 0, 100);
        arrogance = Mathf.Clamp(arrogance + arroganceChange, 0, 100);
        friendliness = Mathf.Clamp(friendliness + friendlinessChange, 0, 100);
        accuracy = Mathf.Clamp(accuracy + accuracyChange, 0, 100);

        /* 每次数值发生变化后，我们都需要立刻检查一下有没有触发暴毙结局 */
        CheckInstantDeath();
    }
    
    /* 检查是否触发中途暴毙结局 */
    private void CheckInstantDeath()
    {
        /* 结局 A：恐慌度爆表 */
        if (panic >= 100)
        {
            /* Debug.Log 可以让文字显示在 Unity 底部的控制台里，方便我们现在做测试 */
            Debug.Log("触发结局 A：【地球的自我毁灭】民众彻底失控，核弹盲目发射。");
            /* 以后这里会写跳转到结局画面的代码，现在先用文字代替 */
        }
        /* 结局 B：傲慢度爆表 */
        else if (arrogance >= 100)
        {
            Debug.Log("触发结局 B：【傲慢的代价】地球军方主动挑衅，被外星主舰蒸发。");
        }
        /* 结局 C：友好度降到冰点 */
        else if (friendliness <= 0)
        {
            Debug.Log("触发结局 C：【星际降维打击】外星使团被激怒，投放了黑洞。");
        }
        /* 结局 D：精神崩溃 */
        else if (petState <= 0)
        {
            Debug.Log("触发结局 D：【精神过载】接线员精神崩溃，砸键盘跑路。");
        }
    }
    
    /* 每日结算后调用，准备迎接新的一天 */
    public void ResetDailyStats()
    {
        /* 在重置准确率之前，先让 FeedbackManager 结算一下今天的宠物状态 */
        FeedbackManager.Instance.CalculatePetState(accuracy);

        /* 算完账之后，再把准确率强制重置为 50，其他数值继承昨天的保持不变 */
        accuracy = 50;
        
        Debug.Log("新的一天开始了，准确率已重置为 50！");
    }
}