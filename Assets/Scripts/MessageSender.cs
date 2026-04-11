using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MessageSender : MonoBehaviour
{
    public static MessageSender Instance;
    void Awake() => Instance = this;

    // 这是按钮现在唯一应该呼叫的函数！
    public void SendCurrentMessage()
    {
        // 1. 检查下限：不足 4 个词，直接拦截！
        int wordCount = WordBlockManager.Instance.GetSelectedWordCount();
        if (wordCount < 4)
        {
            Debug.LogWarning("发送失败：通讯协议要求至少发送 4 个词块！当前只有 " + wordCount + " 个。");
            return;
        }

        // 2. 提取玩家拼凑的最终字符串
        string finalString = "";
        var targetContainer = WordBlockManager.Instance.currentTargetContainer;
        foreach (Transform child in targetContainer)
        {
            var text = child.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) finalString += text.text;
        }

        // 3. 获取当前句子的 ID，去档案库找规则
        string id = WordBlockManager.Instance.currentSentenceID;
        List<MessageData> rules = CSVImporter.Instance.GetRulesByID(id);

        if (rules == null || rules.Count == 0)
        {
            Debug.LogError("致命错误：CSV 里找不到 ID 为 " + id + " 的规则！");
            return;
        }

        // 4. 核心匹配算法（按优先级从 1 往下找）
        MessageData matchedRule = null;
        var sortedRules = rules.OrderBy(r => r.priority).ToList();

        foreach (var rule in sortedRules)
        {
            // 如果这行是兜底规则，先记下来，但继续往下找看有没有更精准的
            if (rule.mustInclude.Contains("DEFAULT")) {
                matchedRule = rule;
                continue; 
            }

            // 检查“必须包含”和“不能包含”
            bool hasMust = rule.mustInclude.All(w => string.IsNullOrEmpty(w) || finalString.Contains(w));
            bool hasForbidden = rule.mustNotInclude.Any(w => !string.IsNullOrEmpty(w) && finalString.Contains(w));

            // 如果精准命中！
            if (hasMust && !hasForbidden) {
                matchedRule = rule;
                break; // 找到了最高优先级的，立刻停手
            }
        }

        // 5. 触发结算！
        if (matchedRule != null)
        {
            Debug.Log($"<color=green>匹配成功！触发优先级: {matchedRule.priority}，反馈: {matchedRule.feedback}</color>");
            
            // 呼叫 GameManager 扣血条
            GameManager.Instance.ApplyStatChanges(
                matchedRule.panicChange,
                matchedRule.arroganceChange,
                matchedRule.friendlinessChange,
                matchedRule.accuracyChange
            );
            
            DayManager.Instance.Invoke("LoadNextSentence", 1.5f);
        }
        else
        {
            Debug.LogWarning("没有任何规则匹配成功（连 DEFAULT 都没有）！");
        }
    }
}