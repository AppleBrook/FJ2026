using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 单例模式，方便其他任何脚本随时呼叫它
    public static AudioManager Instance;

    [Header("播放器 (Audio Sources)")]
    public AudioSource bgmSource; // 专门放背景音乐的喇叭
    public AudioSource sfxSource; // 专门放各种短促音效的喇叭

    [Header("1. BGM 背景音乐")]
    public AudioClip bgm_Gameplay;

    [Header("2. SFX 游戏内交互")]
    public AudioClip sfx_Call; // 通讯接入

    [Header("3. UI 界面音效")]
    public AudioClip ui_Click; // 词块选择/发送
    public AudioClip ui_Back;  // 退回词块

    [Header("4. 结局音效")]
    public AudioClip end_E;
    public AudioClip end_H;
    public AudioClip end_G;
    public AudioClip end_F;
    public AudioClip end_BE;   // 前面四种暴毙结局共用

    void Awake()
    {
        // 保证整个游戏只有一个 AudioManager，并且切换到结局场景时它不会被销毁！
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 游戏一运行，立刻开始播放 BGM
        PlayBGM(bgm_Gameplay);
    }

    // 播放背景音乐的专用函数
    public void PlayBGM(AudioClip clip)
    {
        if (clip != null && bgmSource != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true; // 开启循环
            bgmSource.Play();
        }
    }

    // 停止背景音乐（结局时会用到）
    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // 播放单次音效的专用函数
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // PlayOneShot 的好处是：短时间内连续点击，声音会叠加重合，而不会互相打断！
            sfxSource.PlayOneShot(clip); 
        }
    }
}