using System.Collections.Generic;

/* 注意：这个类没有 ": MonoBehaviour" 的尾巴，因为它不需要挂在游戏物体上，它只是个纯数据图纸 */
[System.Serializable]
public class MessageData
{
    /* 对应表格第1列：ID */
    public string id;

    /* 对应表格第2列：关卡 */
    public string level;

    /* 对应表格第4列：拆分数据（我们直接把它存成一个词块数组） */
    public string[] words;

    /* 对应表格第5列：优先级 */
    public int priority;

    /* 对应表格第6列：必须包含（存成一个列表） */
    public List<string> mustInclude = new List<string>();

    /* 对应表格第7列：不能包含（存成一个列表） */
    public List<string> mustNotInclude = new List<string>();

    /* 对应表格第8,9,10,11列：数值变化 */
    public int panicChange;
    public int arroganceChange;
    public int friendlinessChange;
    public int accuracyChange;

    /* 对应表格最后1列：反馈文本 */
    public string feedback;
}