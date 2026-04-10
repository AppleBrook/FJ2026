using UnityEngine;

/* 我们在这里定义三个评价档位（枚举），就像给成绩评级一样 */
public enum PerformanceTier
{
    Low,    
    Medium, 
    High    
}

public class FeedbackManager : MonoBehaviour
{
    /* 同样做一个单例大管家，方便随时呼叫 */
    public static FeedbackManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /* 流程步骤 1：看分评级。根据准确率，返回对应的档位 */
    public PerformanceTier GetTier(int accuracy)
    {
        if (accuracy < 30) 
        {
            return PerformanceTier.Low;
        }
        else if (accuracy < 60) 
        {
            return PerformanceTier.Medium;
        }
        else 
        {
            return PerformanceTier.High;
        }
    }

    /* 流程步骤 2：宠物状态结算。每天下班时根据准确率来决定宠物的死活 */
    public void CalculatePetState(int accuracy)
    {
        /* 先拿到今天的评价档位 */
        PerformanceTier todayTier = GetTier(accuracy);

        if (todayTier == PerformanceTier.High)
        {
            /* 如果是 High 档（>=60），宠物状态 +1，最高不超过 5 */
            GameManager.Instance.petState = Mathf.Min(GameManager.Instance.petState + 1, 5);
            Debug.Log("准确率达标！宠物状态变好，当前状态：" + GameManager.Instance.petState);
        }
        else
        {
            /* 如果是 Low 或 Medium 档（<60），宠物状态 -1，最低不低于 0 */
            GameManager.Instance.petState = Mathf.Max(GameManager.Instance.petState - 1, 0);
            Debug.Log("准确率不佳...宠物状态变差，当前状态：" + GameManager.Instance.petState);
        }
    }
}