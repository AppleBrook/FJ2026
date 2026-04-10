using UnityEngine;
using UnityEngine.UI; // 注意：如果你用的是 TextMeshPro，这里要换成 using TMPro;

public class UI_TopBarManager : MonoBehaviour
{
    /* 拖拽引用的插槽 */
    public Text panicText;
    public Text arroganceText;
    public Text friendlinessText;
    public Text dayText;

    void Update()
    {
        /* 每一帧都实时同步 GameManager 里的数值 */
        panicText.text = "恐慌: " + GameManager.Instance.panic;
        arroganceText.text = "傲慢: " + GameManager.Instance.arrogance;
        friendlinessText.text = "友好: " + GameManager.Instance.friendliness;
        dayText.text = "DAY 1"; // 暂时写死，以后再动态更新
    }
}