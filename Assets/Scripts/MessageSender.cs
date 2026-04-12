using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MessageSender : MonoBehaviour
{
    public static MessageSender Instance;
    
    /* 新增：防连点锁 */
    private bool isProcessing = false; 

    void Awake() => Instance = this;

    public void SendCurrentMessage()
    {
        /* 核心改动 1：如果锁上了，直接把玩家的点击拦截掉，当无事发生 */
        if (isProcessing) 
        {
            return; 
        }

        int wordCount = WordBlockManager.Instance.GetSelectedWordCount();
        if (wordCount < 4)
        {
            Debug.LogWarning("发送失败：通讯协议要求至少发送 4 个词块！");
            return;
        }

        // 提取玩家拼凑的最终字符串
        string finalString = "";
        var targetContainer = WordBlockManager.Instance.currentTargetContainer;
        foreach (Transform child in targetContainer)
        {
            var text = child.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) finalString += text.text;
        }

        string id = WordBlockManager.Instance.currentSentenceID;
        List<MessageData> rules = CSVImporter.Instance.GetRulesByID(id);

        if (rules == null || rules.Count == 0) return;

        MessageData matchedRule = null;
        var sortedRules = rules.OrderBy(r => r.priority).ToList();

        foreach (var rule in sortedRules)
        {
            if (rule.mustInclude.Contains("DEFAULT")) {
                matchedRule = rule;
                continue; 
            }

            bool hasMust = rule.mustInclude.All(w => string.IsNullOrEmpty(w) || finalString.Contains(w));
            bool hasForbidden = rule.mustNotInclude.Any(w => !string.IsNullOrEmpty(w) && finalString.Contains(w));

            if (hasMust && !hasForbidden) {
                matchedRule = rule;
                break; 
            }
        }

        if (matchedRule != null)
        {
            /* 核心改动 2：一旦匹配成功，立刻上锁！*/
            isProcessing = true; 

            if (FeedbackUIManager.Instance != null) {
                FeedbackUIManager.Instance.ShowFeedback(matchedRule.feedback);
            }
            
            Debug.Log($"匹配成功！触发优先级: {matchedRule.priority}");
            
            GameManager.Instance.ApplyStatChanges(
                matchedRule.panicChange,
                matchedRule.arroganceChange,
                matchedRule.friendlinessChange,
                matchedRule.accuracyChange
            );
            
            // 呼叫 DayManager 等待 1.5 秒后出下一句
            DayManager.Instance.Invoke("LoadNextSentence", 1.5f);
            
            /* 核心改动 3：同样等待 1.5 秒后，把锁解开 */
            Invoke("UnlockSending", 1.5f); 
        }
    }

    /* 新增：解开防连点锁的函数 */
    private void UnlockSending()
    {
        isProcessing = false;
    }
}