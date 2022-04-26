using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleWinG3;
    Rigidbody rb;
    public static bool isDealth = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particleWinG3.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Dealth")
        {
            isDealth = true;
            particleWinG3.Play();
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Foot")
        {
            //AnimationEnemy.Ins.SetAnimation(AnimationEnemy.AnimationType.Scare);
        }
    }
}
