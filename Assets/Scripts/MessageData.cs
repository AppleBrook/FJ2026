using System.Collections.Generic;

[System.Serializable]
public class MessageData
{
    /* 对应表格第1列：ID */
    public string id;

    /* 对应表格第2列：关卡 */
    public string level;

    /* 新增：对应表格第3列：来源（地球 或 外星人） */
    public string source;

    /* 对应表格第4列：拆分数据 */
    public string[] words;

    /* 对应表格第5列：优先级 */
    public int priority;

    /* 对应表格第6列：必须包含 */
    public List<string> mustInclude = new List<string>();

    /* 对应表格第7列：不能包含 */
    public List<string> mustNotInclude = new List<string>();

    /* 对应表格第8,9,10,11列：数值变化 */
    public int panicChange;
    public int arroganceChange;
    public int friendlinessChange;
    public int accuracyChange;

    /* 对应表格最后1列：反馈文本 */
    public string feedback;
}