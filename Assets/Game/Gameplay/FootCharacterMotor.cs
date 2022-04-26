using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FootCharacterMotor : MonoBehaviour
{
    [SerializeField] GameObject target;
    public static FootCharacterMotor Ins;
    public GameObject posFoot;
    public bool isMoveLeg = false;
    bool isFailTrigger = false;
    public FixedJoint fixedJointFoot;
    private void Awake()
    {
        Ins = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        fixedJointFoot.connectedBody = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(SoccerPlayerController.isStop == true)
        {
            /*fixedJointFoot.connectedBody = target.GetComponent<Rigidbody>();*/
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Pedal" && isFailTrigger == false)
        {
            
            isFailTrigger = true;
            StartCoroutine(Delay());
            SoccerPlayerController.isStop = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pedal")
        {
            //SoccerPlayerController.isStop = false;
            LegCharacter.Ins.isPedal = false;
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        fixedJointFoot.connectedBody = target.GetComponent<Rigidbody>();
    }
}
