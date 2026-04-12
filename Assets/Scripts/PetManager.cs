using UnityEngine;
using UnityEngine.UI;

public class PetManager : MonoBehaviour
{
    public static PetManager Instance;
    private Animator animator;

    void Awake() 
    { 
        Instance = this; 
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 游戏一开始，先刷新一次状态
        UpdatePetAnimation();
    }

    // 这个函数会把当天的宠物状态告诉动画机
    public void UpdatePetAnimation()
    {
        if (animator != null && GameManager.Instance != null)
        {
            // 给动画机发送名为 "State" 的数字信号
            animator.SetInteger("State", GameManager.Instance.petState);
        }
    }
}