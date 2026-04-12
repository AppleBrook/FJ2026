using UnityEngine;
using UnityEngine.UI;

public class ButtonVisualManager : MonoBehaviour
{
    public static ButtonVisualManager Instance;

    [Header("发给地球 按钮")]
    public Transform btnEarth;
    public GameObject textEarth; // 拖入按钮下面的 Text 子物体

    [Header("发给外星 按钮")]
    public Transform btnAlien;
    public GameObject textAlien; // 拖入按钮下面的 Text 子物体

    void Awake() => Instance = this;

    // 这个函数会根据当前是谁发消息，自动把按钮一前一后错开
    public void UpdateButtons(WordBlockManager.MessageSource src)
    {
        if (src == WordBlockManager.MessageSource.Alien)
        {
            // 外星人发来的，我们需要点“发给地球”按钮
            BringToFront(btnEarth, textEarth);
            SendToBack(btnAlien, textAlien);
        }
        else
        {
            // 地球发来的，我们需要点“发给外星”按钮
            BringToFront(btnAlien, textAlien);
            SendToBack(btnEarth, textEarth);
        }
    }

    private void BringToFront(Transform btn, GameObject txt)
    {
        // 放到 Hierarchy 的最下面 = 渲染在画面的最前面！
        btn.SetAsLastSibling(); 
        txt.SetActive(true);    // 显示文字
        btn.GetComponent<Button>().interactable = true; // 开启可点击
    }

    private void SendToBack(Transform btn, GameObject txt)
    {
        // 放到 Hierarchy 的最上面 = 渲染在画面的最后面（被左右两侧的面板完美挡住边角！）
        btn.SetAsFirstSibling(); 
        txt.SetActive(false);    // 隐藏文字
        btn.GetComponent<Button>().interactable = false; // 关闭可点击
    }
}