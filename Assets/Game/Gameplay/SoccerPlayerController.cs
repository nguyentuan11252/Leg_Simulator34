using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayerController : MonoBehaviour
{
    [SerializeField] private Collider foot;
    [SerializeField] private Rigidbody footBody;
    [SerializeField] private float force = 10.0f;
    public static bool isStop = false;
    public static bool isPlay = false;

    private enum State
    {
        Idle,
        Captured,
    }
    private void Start()
    {
        isPlay = true;
    }
    private State _state = State.Idle;
    private Plane _plane = new Plane(-Vector3.forward, Vector3.zero);

    void Update()
    {
        if (isStop == false)
        {
            if (isPlay == false)
            {
                switch (_state)
                {
                    case State.Idle:
                        {
                            if (!Input.GetMouseButtonDown(0)) return;
                            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (foot.Raycast(ray, out var hit, 1000))
                            {
                                Debug.Log("point");
                                _state = State.Captured;
                            }
                        }
                        break;
                    case State.Captured:
                        {
                            if (Input.GetMouseButtonUp(0))
                            {
                                _state = State.Idle;
                                return;

                            }
                            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (!_plane.Raycast(ray, out var enter)) return;
                            var pos = ray.GetPoint(enter);
                            var v = pos - footBody.position;
                            //v.x = 3;
                            Debug.Log("Captured");
                            footBody.AddForce(v * force);
                        }
                        break;
                }
            }
        }
    }
}
