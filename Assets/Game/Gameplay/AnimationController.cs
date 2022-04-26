using RootMotion.FinalIK;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static FullBodyBipedIK fullBody;
    public static AnimationController Ins;
    Animator animator;
    string RUN_KEY = "IsRun";
    string IDLE_KEY = "DS_onehand_idle_A";
    private void Awake()
    {
        Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        fullBody = GetComponent<FullBodyBipedIK>();
        fullBody.enabled = false;
        animator.enabled = false;
    }
    public void SetAnimation(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.isRun:
                animator.SetBool(RUN_KEY,true);
                break;
            case AnimationType.isIdle:
                animator.Play(IDLE_KEY);
                break;
            case AnimationType.idle:
                animator.SetBool(RUN_KEY,false);
                break;
            
        }
    }
    public enum AnimationType
    {
        isRun,
        isIdle,
        idle

    }
}
