using UnityEngine;
using System.Collections;

public class SystemUIManager : MonoBehaviour
{
    [Header("UI 面板绑定")]
    public GameObject panelCredits;    // 拖入：制作组面板
    public GameObject panelCopyToast;  // 拖入：“已复制”弹窗面板

    void Start()
    {
        // 游戏开始时，确保制作组面板和弹窗是隐藏的
        if (panelCredits != null) panelCredits.SetActive(false);
        if (panelCopyToast != null) panelCopyToast.SetActive(false);
    }

    // --- 1. 退出游戏 ---
    public void QuitGame()
    {
        Debug.Log("玩家点击了退出游戏！(在编辑器中只会看到这条日志，打包后才会真正退出)");
        Application.Quit();
    }

    // --- 2. 打开/关闭制作组面板 ---
    public void OpenCredits()
    {
        if (panelCredits != null) panelCredits.SetActive(true);
    }

    public void CloseCredits()
    {
        if (panelCredits != null) panelCredits.SetActive(false);
    }

    // --- 3. 复制文本到剪贴板并弹窗 ---
    // 注意这个函数带了一个 string 参数！
    public void CopyToClipboard(string textToCopy)
    {
        // 这一行就是 Unity 自带的绝赞复制功能！
        GUIUtility.systemCopyBuffer = textToCopy;
        
        Debug.Log("已复制到剪贴板：" + textToCopy);

        // 停止之前的倒计时（防连点），重新开始 0.5 秒的弹窗倒计时
        StopAllCoroutines();
        StartCoroutine(ShowToast());
    }

    private IEnumerator ShowToast()
    {
        if (panelCopyToast != null)
        {
            panelCopyToast.SetActive(true);  // 显示“已复制”
            yield return new WaitForSeconds(0.5f); // 等待0.5秒
            panelCopyToast.SetActive(false); // 隐藏
        }
    }
}