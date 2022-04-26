using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 3;
    public static bool isStop;
    [SerializeField]
    private Transform _touchPos;
    /*[SerializeField]
    private Transform _targetPos;*/
    private bool _touched;
    [SerializeField]
    private Camera mainCamera;
    public GameObject posCamera;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 velocity = Vector3.right * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
        if (isStop == false)
        {
            AnimationController.Ins.SetAnimation(AnimationController.AnimationType.isRun);
        }*/
        if (isStop == true)
        {
            AnimationController.Ins.SetAnimation(AnimationController.AnimationType.idle);
            speed = 0;
            //DragLeg();
        }

        //// Drag

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            AnimationController.Ins.SetAnimation(AnimationController.AnimationType.isIdle);
            isStop = true;
            Camera.main.transform.position = posCamera.transform.position;
            Camera.main.transform.rotation = posCamera.transform.rotation;
            AnimationController.fullBody.enabled = true;
            Foot.boxColliderFoot.enabled = true;
        }

    }

    void DragLeg()
    {
        Vector3 posMouse = Input.mousePosition;
        //posMouse.z = 1f;
        Ray ray = mainCamera.ScreenPointToRay(posMouse);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity))
        {
            if (raycastHit.collider != null)
            {
                Debug.Log("test va cham " + raycastHit.collider.name);
            }
            if (Input.GetMouseButtonDown(0))
            {
                _touched = true;
                if (_touched == true)
                {
                    _touchPos.position = raycastHit.point;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _touched = false;
            }
            if (!_touched) return;
            Vector3 g = raycastHit.point;
            g.z = -0.2f;
            _touchPos.position = g;
            Debug.Log("Mouse: " + posMouse);
            if (_touchPos.position.x <= 1.3f)
            {
                _touchPos.position = new Vector3(1.3f, _touchPos.position.y, _touchPos.position.z);
            }


        }
    }

}
