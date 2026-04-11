public static class GlobalData
{
    // 这个变量因为是 static，所以它超脱于所有场景之外，永远存在于内存中。
    // 我们会在 GameScene 给它赋值，然后在 EndingScene 读取它！
    public static string currentEndingID = "None"; 
}