using UnityEngine;
using UnityEngine.UI;

public class CloneBlock : MonoBehaviour
{
    [HideInInspector]
    public WordBlock originalBlock; // 记住自己的本体是谁

    // 当玩家点击这个克隆体时，触发这个函数
    public void OnCloneClicked()
    {
        if (originalBlock != null)
        {
            // 就像隔空按下了本体的按钮一样！
            // 本体收到信号后，会自动换回没选中的美术图，并且通知大管家重新排版（排版时这个克隆体自然就会消失啦）
            originalBlock.ToggleSelection();
        }
    }
}