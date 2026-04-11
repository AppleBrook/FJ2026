using UnityEngine;
using UnityEngine.UI; // 必须加这行，程序才认识 Slider（滑动条）
using TMPro; 

public class UI_TopBarManager : MonoBehaviour
{
    /* 之前的文本插槽换成了血条插槽 */
    public Slider panicSlider;
    public Slider arroganceSlider;
    public Slider friendlinessSlider;

    /* 保留的天数文字插槽 */
    public TextMeshProUGUI dayText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            /* * 注意：因为 GameManager 里的数值通常是 0 到 100，
             * 而你刚才截图里的 Slider 默认最大值(Max Value)是 1，
             * 所以我们把数值除以 100f 转换成比例塞给血条。
             */
            panicSlider.value = GameManager.Instance.panic / 100f;
            arroganceSlider.value = GameManager.Instance.arrogance / 100f;
            friendlinessSlider.value = GameManager.Instance.friendliness / 100f;
            
            dayText.text = "DAY 1"; 
        }
    }
}