using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("背景物理机关")]
    public RectTransform windowBlind; // 拖入那块带标题的遮光板 (Img_WindowBlind)
    public float slideSpeed = 400f;   // 舷窗上升速度（可根据美术图大小自行微调）

    [Header("显示器 UI 层")]
    public GameObject panelStartScreen; // 显示器待机屏 (含开始按钮)
    public GameObject panelDesktop;     // 显示器工作桌面 (含邮件图标)

    void Start()
    {
        // 游戏刚打开时：强制显示待机屏，隐藏带有邮件的工作桌面
        if (panelStartScreen != null) panelStartScreen.SetActive(true);
        if (panelDesktop != null) panelDesktop.SetActive(false);
    }

    // 绑定给“开始游戏”按钮的核心函数
    public void OnStartGameClicked()
    {
        // 1. 显示器画面瞬间切换：干掉待机屏，亮起操作系统桌面
        if (panelStartScreen != null) panelStartScreen.SetActive(false);
        if (panelDesktop != null) panelDesktop.SetActive(true);

        // 2. 触发背景机关：升起舷窗！
        if (windowBlind != null)
        {
            StartCoroutine(SlideBlindUp());
        }
    }

    private IEnumerator SlideBlindUp()
    {
        // 计算目标高度（让它往上滑出它自身的高度，彻底让出视野）
        float targetY = windowBlind.anchoredPosition.y + windowBlind.rect.height;

        // 只要还没滑到目标高度，就一直往上移
        while (windowBlind.anchoredPosition.y < targetY)
        {
            // Vector2.up 就是 (0, 1)，意味着只在 Y 轴向上移动
            windowBlind.anchoredPosition += Vector2.up * slideSpeed * Time.deltaTime;
            yield return null; // 等待下一帧画面
        }

        // 彻底滑出视野后，把它隐藏掉以节省性能
        windowBlind.gameObject.SetActive(false);
    }
}