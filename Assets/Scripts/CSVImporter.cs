using UnityEngine;
using System.Collections.Generic;

public class CSVImporter : MonoBehaviour
{
    /* 这是一个空箱子，准备用来装我们翻译好的所有表格数据 */
    public List<MessageData> allMessages = new List<MessageData>();

    /* 这是一个插槽，以后我们会在Unity面板里把表格文件拖进去 */
    public TextAsset csvFile;

    /* 游戏刚启动时，就会自动呼叫读取功能 */
    void Start()
    {
        LoadData();
    }

public void LoadData()
    {
        /* 先检查一下插槽里有没有放文件 */
        if (csvFile == null)
        {
            Debug.Log("读取失败：没有找到表格文件！");
            return;
        }

        /* 每次读取前，先把旧数据清空，防止重复 */
        allMessages.Clear();

        /* 把整个文件的文字，按照“换行符”切开，变成一行一行的数组 */
        string[] lines = csvFile.text.Split('\n');
        
        /* 从第1行开始往下读（跳过第0行，因为第0行是表头标题，不需要读进去） */
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            /* 如果遇到空行，就直接跳过去看下一行 */
            if (string.IsNullOrEmpty(line)) continue;

            /* 按照逗号，把这一行切成一个个的格子（列） */
            string[] columns = line.Split(',');

            /* 因为加了一列，现在一行至少要有 12 个格子 */
            if (columns.Length >= 12)
            {
                MessageData newData = new MessageData();
                
                /* 第 1、2 列没变 */
                newData.id = columns[0];
                newData.level = columns[1];
                
                /* 新增：读取第 3 列作为来源 */
                newData.source = columns[2];
                
                /* 后面的格子全部往后挪了一位，索引都要加 1 */
                /* 第 4 列：句子词块 */
                newData.words = columns[3].Split('/'); 
                
                /* 第 5 列：优先级 */
                newData.priority = int.Parse(columns[4]);
                
                /* 第 6、7 列：包含与不包含 */
                newData.mustInclude = new List<string>(columns[5].Split(' '));
                newData.mustNotInclude = new List<string>(columns[6].Split(' '));
                
                /* 第 8, 9, 10, 11 列：数值变动 */
                newData.panicChange = int.Parse(columns[7]);
                newData.arroganceChange = int.Parse(columns[8]);
                newData.friendlinessChange = int.Parse(columns[9]);
                newData.accuracyChange = int.Parse(columns[10]);
                
                /* 第 12 列：反馈内容 */
                newData.feedback = columns[11];

                allMessages.Add(newData);
            }

        }
        
        Debug.Log("数据读取完毕！成功存入 " + allMessages.Count + " 条电报数据。");
    }
}