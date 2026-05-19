using UnityEngine;
using UnityEngine.UI;

public class ButtonVisualManager : MonoBehaviour
{
    public static ButtonVisualManager Instance;

    [Header("发给地球 按钮")]
    public Transform btnEarth;

    [Header("发给外星 按钮")]
    public Transform btnAlien;

    void Awake() => Instance = this;

    // 这个函数会根据当前是谁发消息，直接控制按钮本体的出现与消失
    public void UpdateButtons(WordBlockManager.MessageSource src)
    {
        if (src == WordBlockManager.MessageSource.Alien)
        {
            // 外星人发来的，显示“发给地球”按钮，彻底隐藏“发给外星”按钮
            btnEarth.gameObject.SetActive(true);
            btnAlien.gameObject.SetActive(false);
        }
        else
        {
            // 地球发来的，彻底隐藏“发给地球”按钮，显示“发给外星”按钮
            btnEarth.gameObject.SetActive(false);
            btnAlien.gameObject.SetActive(true);
        }
    }
}