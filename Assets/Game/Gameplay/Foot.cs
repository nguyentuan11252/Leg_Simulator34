using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    public static BoxCollider boxColliderFoot;
    // Start is called before the first frame update
    void Start()
    {
        boxColliderFoot = GetComponent<BoxCollider>();
        boxColliderFoot.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Player.isStop = false;
        }
    }
}
