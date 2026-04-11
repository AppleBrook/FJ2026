using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class UI_TopBarManager : MonoBehaviour
{
    public Slider panicSlider;
    public Slider arroganceSlider;
    public Slider friendlinessSlider;
    public TextMeshProUGUI dayText;

    // 动画速度：因为 Slider 的范围是 0 到 1，
    // 我们希望最长 0.5 秒走完全程，所以速度是 1 / 0.5 = 2.0
    private float animationSpeed = 2.0f;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            // 1. 获取 GameManager 里的目标真实数值 (0~1 比例)
            float targetPanic = GameManager.Instance.panic / 100f;
            float targetArrogance = GameManager.Instance.arrogance / 100f;
            float targetFriendliness = GameManager.Instance.friendliness / 100f;

            // 2. 核心动画魔法：让当前血条的值，以固定的速度，向目标值“追”过去！
            panicSlider.value = Mathf.MoveTowards(panicSlider.value, targetPanic, animationSpeed * Time.deltaTime);
            arroganceSlider.value = Mathf.MoveTowards(arroganceSlider.value, targetArrogance, animationSpeed * Time.deltaTime);
            friendlinessSlider.value = Mathf.MoveTowards(friendlinessSlider.value, targetFriendliness, animationSpeed * Time.deltaTime);
            
            // 顺便把天数同步了 (读取 DayManager 里的天数)
            if (DayManager.Instance != null)
            {
                // 把 "Day 1" 变成大写显示在 UI 上
                dayText.text = DayManager.Instance.currentDay.ToUpper();
            }
        }
    }
}