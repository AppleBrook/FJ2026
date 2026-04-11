using UnityEngine;
using UnityEngine.EventSystems; // 必须引入这个命名空间才能读取鼠标事件

// 继承三个接口：鼠标按下、鼠标抬起、鼠标移出
public class ButtonTextPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("要跟着移动的 UI (比如文字)")]
    public RectTransform targetTransform;
    
    [Header("按下去时的偏移量 (X, Y)")]
    public Vector2 pressOffset = new Vector2(0, -5f); // 默认往下移动 5 个像素

    private Vector2 originalPosition;
    private bool isPressed = false;

    void Start()
    {
        // 游戏开始时，记录文字原本的位置
        if (targetTransform != null)
        {
            originalPosition = targetTransform.anchoredPosition;
        }
    }

    // 当鼠标点下去的瞬间
    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetTransform != null && !isPressed)
        {
            targetTransform.anchoredPosition = originalPosition + pressOffset;
            isPressed = true;
        }
    }

    // 当鼠标松开的瞬间
    public void OnPointerUp(PointerEventData eventData)
    {
        ResetPosition();
    }

    // 【防卡死机制】：当鼠标按住按钮，但中途拖到了按钮外面才松开时，也要恢复原位
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetPosition();
    }

    private void ResetPosition()
    {
        if (targetTransform != null && isPressed)
        {
            targetTransform.anchoredPosition = originalPosition;
            isPressed = false;
        }
    }
}