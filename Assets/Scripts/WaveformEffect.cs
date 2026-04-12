using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveformEffect : MonoBehaviour
{
    public static WaveformEffect Instance;

    [Header("UI 绑定")]
    // 注意：这里不再需要绑定外框 Container 了，因为我们不需要隐藏它
    public Image waveformImage;          // 拖入：Img_WaveformLine (中间会跳动的线条)

    [Header("波形图序列帧 (拖入4张PNG)")]
    public Sprite[] waveSprites; 

    void Awake() { Instance = this; }

    void Start()
    {
        // 游戏开始时，确保显示的是平稳状态（第一张图），不要隐藏它
        if (waveSprites.Length > 0 && waveformImage != null) 
        {
            waveformImage.sprite = waveSprites[0];
        }
    }

    public void PlayWave(float duration, System.Action onComplete)
    {
        // 直接开始播放协程，不操作显隐
        StartCoroutine(WaveRoutine(duration, onComplete));
    }

    private IEnumerator WaveRoutine(float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        
        // 只要时间没到，就疯狂随机切图
        while (elapsed < duration)
        {
            if (waveSprites.Length > 0 && waveformImage != null)
            {
                // 从你拖入的 4 张图里随机抽一张显示
                int randomIndex = Random.Range(0, waveSprites.Length);
                waveformImage.sprite = waveSprites[randomIndex];
            }
            
            float waitTime = Random.Range(0.05f, 0.1f); 
            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;
        }

        // ================= 核心修改点 =================
        // 播放结束，强制切回平稳直线（第一张图）
        // 并且绝对不隐藏它，让它一直亮着！
        if (waveSprites.Length > 0 && waveformImage != null) 
        {
            waveformImage.sprite = waveSprites[0];
        }
        // ==============================================
        
        // 通知外部：演出结束，可以显示真正的文字了！
        if (onComplete != null) onComplete.Invoke();
    }
}