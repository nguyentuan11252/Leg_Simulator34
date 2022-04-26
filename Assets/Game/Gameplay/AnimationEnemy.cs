using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnemy : MonoBehaviour
{
    Animator anim;
    string Scare = "Scare";
    string IDLE = "Writhe";
    public static AnimationEnemy Ins;
    private void Awake()
    {
        Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void SetAnimation(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Scare:
                anim.SetTrigger(Scare);
                break;
            case AnimationType.Idle:
                anim.Play(IDLE);
                break;

        }
    }
    public enum AnimationType
    {
        Scare,
        Idle
    }
}
