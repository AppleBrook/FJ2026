using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackUIManager : MonoBehaviour
{
    public static FeedbackUIManager Instance;

    [Header("拖入刚才建好的反馈文字")]
    public TextMeshProUGUI feedbackText;

    [Header("显示持续时间")]
    public float displayDuration = 1.5f; // 和你等待下一句话的时间保持一致

    void Awake()
    {
        Instance = this;
        // 游戏刚开始时，确保它是隐藏/清空状态
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }

    // 呼叫这个函数来展示反馈
    public void ShowFeedback(string message)
    {
        if (string.IsNullOrEmpty(message)) 
        {
            return;
        }

        if (feedbackText != null)
        {
            /* 核心修改：在文字前后加上你要求的横杠 */
            feedbackText.text = "- " + message + " -"; 
            
            feedbackText.gameObject.SetActive(true);
            
            StopAllCoroutines();
            StartCoroutine(HideFeedbackAfterDelay());
        }
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        
        // 时间到，关掉文字
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }
}