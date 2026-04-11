using UnityEngine;
using System.Collections.Generic;

public class CSVImporter : MonoBehaviour
{
    public static CSVImporter Instance;

    public List<MessageData> allMessages = new List<MessageData>();
    public TextAsset csvFile;

    void Awake()
    {
        Instance = this;
        LoadData();
    }

    public void LoadData()
    {
        if (csvFile == null)
        {
            Debug.LogError("读取失败：没有找到表格文件！");
            return;
        }

        allMessages.Clear();
        string[] lines = csvFile.text.Split('\n');
        
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length >= 12 && !string.IsNullOrWhiteSpace(columns[0]))
            {
                MessageData newData = new MessageData();
                
                newData.id = columns[0].Trim();
                newData.level = columns[1].Trim();
                newData.source = columns[2].Trim();
                newData.words = columns[3].Trim().Split('/'); 
                
                // 字符串部分正常读取
                newData.mustInclude = new List<string>(columns[5].Trim().Split(' '));
                newData.mustNotInclude = new List<string>(columns[6].Trim().Split(' '));
                newData.feedback = columns[11].Trim();

                // 【终极防爆盾】：尝试转换数字，如果格子为空或格式不对，自动设为 0，绝对不报错！
                int.TryParse(columns[4].Trim(), out newData.priority);
                int.TryParse(columns[7].Trim(), out newData.panicChange);
                int.TryParse(columns[8].Trim(), out newData.arroganceChange);
                int.TryParse(columns[9].Trim(), out newData.friendlinessChange);
                int.TryParse(columns[10].Trim(), out newData.accuracyChange);

                allMessages.Add(newData);
            }
            else
            {
                Debug.LogWarning($"<color=yellow>忽略了残缺的 CSV 行 (第 {i + 1} 行)，可能是文案漏填了基础信息。</color>");
            }
        }
        
        Debug.Log("<color=green>数据读取完毕！成功存入 " + allMessages.Count + " 条电报数据。</color>");
    }

    public List<MessageData> GetRulesByID(string targetID)
    {
        return allMessages.FindAll(m => m.id == targetID);
    }
}