using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LegCharacter : MonoBehaviour
{
    [SerializeField]
    private bool _touched;
    [SerializeField]
    private Camera mainCamera;
    public bool isPedal = false;
    bool isMoveLeg = false;
    public Transform _posPedal;
    public static LegCharacter Ins;
    public bool isMouseUp = false;
    public Transform posStart;
    public GameObject pedalObj;
    public GameObject tagetPlayer;
    public GameObject resetPedal;
    public GameObject footCharacter;
    Vector3 _startVc;
    Vector3 _endVc;
    float v;
    [SerializeField]
    private ParticleSystem particleSmoke;
    [SerializeField]
    private ParticleSystem particleSmoke2;
    [SerializeField]
    private ParticleSystem particleWin;
    public static bool isDealth = false;

    private void Awake()
    {
        Ins = this;
    }
    void Start()
    {
        _startVc = Vector3.zero;
        _endVc = Vector3.zero;
        particleSmoke.Stop();
        particleSmoke2.Stop();
        particleWin.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (SoccerPlayerController.isStop == true)
        {
            
            footCharacter.transform.position = _posPedal.position;
            
            DragFootAndPedal();
        }
    }
    void DragFootAndPedal()
    {
        Vector3 posMouse = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(posMouse);

        if (Input.GetMouseButtonDown(0))
        {
            _startVc = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            v = (_startVc.y - _endVc.y) * 0.5f;
            _touched = false;
            if (v != 0)
            {
                float time2 = 0;
                if (Time.time - time2 >= 0.5f)
                {
                    time2 = Time.time;
                    FootCharacterMotor.Ins.isMoveLeg = true;
                    pedalObj.transform.DORotate(new Vector3(0, 0, transform.rotation.z + 50f), 1 - v);
                    StartCoroutine(ResetPedal());
                    particleSmoke.Play();
                }
                if (v > 0.2f)
                {
                    //Run
                    //phut phao
                    isDealth = true;
                    SoccerPlayerController.isStop = false;
                    particleSmoke2.Play();
                    StartCoroutine(ParticleWin());
                }
                else if (v < 0.2f)
                {
                    particleSmoke2.Stop();
                    //rung
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _endVc = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            

        }
        if (!_touched) return;
        
    }
    IEnumerator ResetPedal()
    {
        yield return new WaitForSeconds((_startVc.y - _endVc.y) * 0.5f);
        pedalObj.transform.DORotate(new Vector3(0, 0, transform.rotation.z), 1 - v);

        StartCoroutine(ResetPedal1());

    }
    IEnumerator ResetPedal1()
    {
        yield return new WaitForSeconds(1 - v);
        //FootCharacterMotor.Ins.RbCharacterFail();

    }
    IEnumerator ParticleWin()
    {
        yield return new WaitForSeconds(2);
        particleWin.Play();
    }
}
