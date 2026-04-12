using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 

public class EndingManager : MonoBehaviour
{
    [Header("UI 绑定")]
    public Image cgImage;
    public TextMeshProUGUI endingText;
    public GameObject btnReturnMenu; 
    public GameObject btnNextLine;   

    [Header("结局 CG 图片 (按需拖入美术素材)")]
    public Sprite cg_End_A;
    public Sprite cg_End_B;
    public Sprite cg_End_C;
    public Sprite cg_End_D;
    public Sprite cg_End_E;
    public Sprite cg_End_F;
    public Sprite cg_End_G;
    public Sprite cg_End_H;

    [Header("打字机设置")]
    public float typingSpeed = 0.05f; // 每个字弹出的间隔时间

    private Queue<string> endingLines = new Queue<string>();
    
    /* ========== 新增：打字机核心变量 ========== */
    private bool isTyping = false;          
    private string currentLineText = "";    
    private Coroutine typingCoroutine;      
    /* ========================================= */

    void Start()
    {
        if (btnReturnMenu != null) btnReturnMenu.SetActive(false);
        if (btnNextLine != null) btnNextLine.SetActive(false);
        if (endingText != null) endingText.gameObject.SetActive(false);

        string id = string.IsNullOrEmpty(GlobalData.currentEndingID) ? "End_H" : GlobalData.currentEndingID;
        
        Debug.Log($"<color=cyan>正在展示结局：{id}</color>");
        
        /* ================= 全局音效接管 ================= */
        if (AudioManager.Instance != null)
        {
            // 注意这里：把所有的 PlaySFX 换成了 PlayBGM！
            switch(id)
            {
                case "End_E": AudioManager.Instance.PlayBGM(AudioManager.Instance.end_E); break;
                case "End_H": AudioManager.Instance.PlayBGM(AudioManager.Instance.end_H); break;
                case "End_G": AudioManager.Instance.PlayBGM(AudioManager.Instance.end_G); break;
                case "End_F": AudioManager.Instance.PlayBGM(AudioManager.Instance.end_F); break;
                default: AudioManager.Instance.PlayBGM(AudioManager.Instance.end_BE); break; 
            }
        }
        /* ====================================================== */

        LoadEndingData(id);
        StartCoroutine(StartEndingSequence());
    }

    private IEnumerator StartEndingSequence()
    {
        // 【修改点 1】留白时间从 0.5 秒加长到了 1 秒！
        yield return new WaitForSeconds(1f);

        if (endingText != null) endingText.gameObject.SetActive(true);
        if (btnNextLine != null) btnNextLine.SetActive(true);

        PlayNextLine();
    }

    private void LoadEndingData(string endingID)
    {
        endingLines.Clear();

        switch (endingID)
        {
            case "End_A":
                if (cg_End_A != null) cgImage.sprite = cg_End_A;
                endingLines.Enqueue("你和外星人之间的一次次通信，\n都在地球上被解读为末日的先兆。");
                endingLines.Enqueue("人们心头的阴影被恐惧喂养得越来越大，越来越壮，\n最后，核弹在陆地上次第升起，目标是地球。");
                endingLines.Enqueue("你在空间站看着地球自爆了。\n蔚蓝色的碎片绕着空间站缓缓旋转，里面包裹着你的家人的余温。");
                endingLines.Enqueue("你现在是世界上最后一个人类了。");
                endingLines.Enqueue("——结局 A：【疯狂】——");
                break;

            case "End_B":
                if (cg_End_B != null) cgImage.sprite = cg_End_B;
                endingLines.Enqueue("太阳系内无新事，\n每一场战争都事关傲慢，事关偏见。");
                endingLines.Enqueue("而每一场毁灭，都源于名为“文明”的傲慢，和名为“物种”的偏见。");
                endingLines.Enqueue("地球主动向外星人发射了核导弹。\n你站在空间站，看着一缕炙热的光芒划过群星，带来了死亡。");
                endingLines.Enqueue("——结局 B：【傲慢与偏见】——");
                break;

            case "End_C":
                if (cg_End_C != null) cgImage.sprite = cg_End_C;
                endingLines.Enqueue("猜疑链一旦锁死，\n沟通便化作毫无意义的杂音。\n停留在寂静深空的外星文明对吵闹的地球失去了信任与耐心。");
                endingLines.Enqueue("地球在外星人高效的打击下崩解了。\n蔚蓝色的碎片绕着空间站缓缓旋转，里面包裹着你的家人的余温。");
                endingLines.Enqueue("你现在是世界上最后一个人类了。");
                endingLines.Enqueue("——结局 C：【无效杂音】——");
                break;

            case "End_D":
                if (cg_End_D != null) cgImage.sprite = cg_End_D;
                endingLines.Enqueue("你无害的小植物死了。\n它柔软的草叶在你眼前慢慢枯萎。");
                endingLines.Enqueue("今夜，你不关心人类。");
                endingLines.Enqueue("你亲手结束了这场孤独的远征，\n一具无主躯壳在群星间悠悠漂浮。");
                endingLines.Enqueue("今夜，宇宙热闹而寂寥。");
                endingLines.Enqueue("——结局 D：【今夜你不关心人类】——");
                break;

            case "End_E":
                if (cg_End_E != null) cgImage.sprite = cg_End_E;
                endingLines.Enqueue("在克制与真诚中，\n你赢得了外星人的高度信任。\n地球和外星文明正式结盟。");
                endingLines.Enqueue("你作为第一次接触外星人的“首席外交官”被载入史册，青史留名。\n此后，每当你仰望星空，都会想起那段孤寂又美好的奋斗岁月。");
                endingLines.Enqueue("——结局 E：【共荣】——");
                break;

            case "End_F":
                if (cg_End_F != null) cgImage.sprite = cg_End_F;
                endingLines.Enqueue("为了讨好外星人，\n你卑躬屈膝地将地球粉饰为一个温顺无害的文明。");
                endingLines.Enqueue("外星人收起了武器，但带来了项圈。\n地球变成了他们的“星际采矿场”和“异宠繁育基地”");
                endingLines.Enqueue("因为“沟通有功”，你和你的家人作为最昂贵的异宠，被卖了个好价钱。\n你们将住进恒温的黄金笼子，安度余生。");
                endingLines.Enqueue("——结局 F：【黄金笼中鸟】——");
                break;

            case "End_G":
                if (cg_End_G != null) cgImage.sprite = cg_End_G;
                endingLines.Enqueue("由于你的传信充满了火药味和虚张声势，\n地球和外星人都不敢轻举妄动。");
                endingLines.Enqueue("太阳系边界布满森森军队，地球进入了长达数百年的“太空冷战”纪元，\n文明的所有资源全部流向军工。");
                endingLines.Enqueue("你回到地球后，与家人在昏暗的防空洞中短暂重聚。\n没多久，你收到了新的入伍通知，走向星际战壕。");
                endingLines.Enqueue("——结局 G：【永夜铁幕】——");
                break;

            case "End_H":
            default:
                if (cg_End_H != null) cgImage.sprite = cg_End_H;
                endingLines.Enqueue("这三天的交流像是一场跨服聊天，双方都没太搞懂对方的意思，但也懒得深究。");
                endingLines.Enqueue("外星舰队觉得地球无聊透顶，调转船头飞向了半人马座。");
                endingLines.Enqueue("你被上级以“业务能力低下”为由解雇，回家种地了。\n在贫瘠的黄土地上，你偶尔抬头看向星空，会怀疑那几天只是你因孤独而产生的一场幻觉。");
                endingLines.Enqueue("——结局 H：【庸人的幸存】——");
                break;
        }
    }

    public void PlayNextLine()
    {
        if (isTyping)
        {
            // 瞬间打完
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            endingText.text = currentLineText;
            isTyping = false;
            return; 
        }

        if (endingLines.Count > 0)
        {
            currentLineText = endingLines.Dequeue();
            typingCoroutine = StartCoroutine(TypeText(currentLineText));
        }
        else
        {
            if (btnNextLine != null) btnNextLine.SetActive(false);
            StartCoroutine(ShowReturnMenuDelayed());
        }
    }

    private IEnumerator TypeText(string lineToType)
    {
        isTyping = true;
        endingText.text = ""; 

        foreach (char letter in lineToType.ToCharArray())
        {
            endingText.text += letter;
            yield return new WaitForSeconds(typingSpeed); 
        }

        isTyping = false;
    }

    private IEnumerator ShowReturnMenuDelayed()
    {
        // 这里的 1 秒等待没变
        yield return new WaitForSeconds(1f);
        if (btnReturnMenu != null) btnReturnMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        // 用代码隔空呼叫存在于游戏里的声音管理器
        if (AudioManager.Instance != null)
        {
            // 播放点击音效
            AudioManager.Instance.PlaySFX(AudioManager.Instance.ui_Click);
            
            // 【新增这一句】：在切回主菜单前，重新让大喇叭播放日常 BGM！
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgm_Gameplay);
        }
        
        GlobalData.currentEndingID = "";
        SceneManager.LoadScene("GameScene"); 
    }
}