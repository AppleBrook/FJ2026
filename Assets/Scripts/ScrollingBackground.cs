using UnityEngine;
using UnityEngine.UI; // 注意：RawImage 属于 UI 命名空间

[RequireComponent(typeof(RawImage))]
public class ScrollingBackground : MonoBehaviour
{
    private RawImage bgImage;
    
    [Header("滚动速度设置")]
    [Tooltip("X代表水平速度，Y代表垂直速度。正数向左/下，负数向右/上。建议设得很小！")]
    public Vector2 scrollSpeed = new Vector2(0.02f, 0f); // 默认向左极其缓慢地飘动

    void Start()
    {
        // 自动获取身上的 RawImage 组件
        bgImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (bgImage != null)
        {
            // 获取当前图片的 UV 矩形
            Rect currentRect = bgImage.uvRect;
            
            // 根据时间和速度计算偏移量（乘以 Time.deltaTime 确保不同帧率下速度一致）
            currentRect.x += scrollSpeed.x * Time.deltaTime;
            currentRect.y += scrollSpeed.y * Time.deltaTime;
            
            // 把偏移后的坐标还给图片，实现滚动！
            bgImage.uvRect = currentRect;
        }
    }
}