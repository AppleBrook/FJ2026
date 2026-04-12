using UnityEngine;
using UnityEngine.UI;

// 强制要求挂载这个脚本的物体必须有 Button 组件
[RequireComponent(typeof(Button))]
public class ButtonSoundHelper : MonoBehaviour
{
    void Start()
    {
        // 游戏一开始，自动拿到自己身上的按钮
        Button btn = GetComponent<Button>();
        
        // 给按钮自动绑定点击事件（纯代码绑定，跨场景绝对不会丢！）
        btn.onClick.AddListener(() => {
            if (AudioManager.Instance != null)
            {
                // 自动呼叫永远活着的那个大喇叭，播放 UI 点击音效
                AudioManager.Instance.PlaySFX(AudioManager.Instance.ui_Click);
            }
        });
    }
}