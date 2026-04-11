using UnityEngine;
using UnityEngine.UI; // 控制颜色需要用到
using TMPro;

public class WordBlock : MonoBehaviour
{
    public bool isSelected = false; // 核心状态：记录自己有没有被选中

    private Image backgroundImage;  // 背景图组件
    private Color normalColor = Color.white; // 默认颜色：白
    private Color selectedColor = Color.gray; // 选中颜色：黄（测试用，以后换图片）

    void Awake()
    {
        // 刚出生时，获取自己的背景组件，并设置为默认颜色
        backgroundImage = GetComponent<Image>();
        backgroundImage.color = normalColor;
    }

    // 这个函数等下用来绑定鼠标点击事件
    public void ToggleSelection()
    {
        // 【新增核心限制】：如果我现在还没被选上，且桌面上已经选满 5 个了，直接拒绝！
        if (!isSelected && WordBlockManager.Instance.GetSelectedWordCount() >= 5)
        {
            Debug.Log("超载警告：最多只能选择 5 个词语！");
            return; 
        }
        // 状态反转：如果是 false 就变 true，如果是 true 就变 false
        isSelected = !isSelected;

        // 根据最新状态，切换颜色
        if (isSelected)
        {
            backgroundImage.color = selectedColor;
        }
        else
        {
            backgroundImage.color = normalColor;
        }

        // 【新加的这一行】：每次被点击后，呼叫大管家重新拼凑句子！
        if (WordBlockManager.Instance != null)
        {
            WordBlockManager.Instance.RebuildTargetSentence();
        }
    }
    
}