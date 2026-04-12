using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBlock : MonoBehaviour
{
    [Header("UI 组件绑定")]
    public TextMeshProUGUI wordText;
    public Image blockImage; // 控制背景图组件

    [HideInInspector]
    public string word;
    [HideInInspector]
    public bool isSelected = false;

    // 内部记下自己当前应该用哪两张图
    private Sprite normalSprite;
    private Sprite selectedSprite;

    /* 初始化函数：生成词块时，把字和属于它的两张图传进来 */
    /* 修改：多加了一个 Color txtColor 参数 */
    public void Initialize(string text, Sprite normal, Sprite selected, Color txtColor)
    {
        word = text;
        if (wordText != null) 
        {
            wordText.text = text;
            wordText.color = txtColor; /* 新增：穿上衣服的同时，改变字体颜色 */
        }
        isSelected = false;

        normalSprite = normal;
        selectedSprite = selected;

        if (blockImage != null && normalSprite != null)
        {
            blockImage.sprite = normalSprite;
        }
    }

    /* 恢复了你原来的函数名，这样你在 Unity 里绑定的按钮就不会断开 */
    public void ToggleSelection()
    {
        // 【恢复核心限制】：如果还没被选上，且桌面上已经选满 5 个了，直接拒绝！
        if (!isSelected && WordBlockManager.Instance.GetSelectedWordCount() >= 5)
        {
            Debug.Log("超载警告：最多只能选择 5 个词语！");
            return; 
        }

        // 状态反转
        isSelected = !isSelected;

        // 【核心改动】：不再改变颜色，而是根据状态切换美术图片
        if (blockImage != null)
        {
            if (isSelected)
            {
                blockImage.sprite = selectedSprite;
            }
            else
            {
                blockImage.sprite = normalSprite;
            }
        }

        // 【恢复原本正确的呼叫】：通知大管家重新拼凑句子！
        if (WordBlockManager.Instance != null)
        {
            WordBlockManager.Instance.RebuildTargetSentence();
        }
    }
}