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

            /* 确保这一行至少有11个格子，防止表格没填满导致程序崩溃 */
            if (columns.Length >= 11)
            {
                /* 拿出一张新的空白图纸 */
                MessageData newData = new MessageData();
                
                /* 开始照着格子依次填入数据 */
                newData.id = columns[0];
                newData.level = columns[1];
                
                /* 第3个格子是句子，用斜杠把它切成词块数组 */
                newData.words = columns[2].Split('/'); 
                
                /* int.Parse 的作用是把文字变成真正的数字 */
                newData.priority = int.Parse(columns[3]);
                
                /* 用空格切开包含的词，放进列表里 */
                newData.mustInclude = new List<string>(columns[4].Split(' '));
                newData.mustNotInclude = new List<string>(columns[5].Split(' '));
                
                /* 继续读取数值变动 */
                newData.panicChange = int.Parse(columns[6]);
                newData.arroganceChange = int.Parse(columns[7]);
                newData.friendlinessChange = int.Parse(columns[8]);
                newData.accuracyChange = int.Parse(columns[9]);
                
                newData.feedback = columns[10];

                /* 把填好数据的图纸，放进大箱子里保存起来 */
                allMessages.Add(newData);
            }
        }
        
        Debug.Log("数据读取完毕！成功存入 " + allMessages.Count + " 条电报数据。");
    }
}