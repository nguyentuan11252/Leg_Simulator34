using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayGameController : MonoBehaviour
{
    public static PlayGameController Ins;
    float zPosition;
    bool isMouseDragging;
    Vector3 offSet;
    public Camera mainCamera;
    /* #endregion

     #region Inspector Variables*/
    [SerializeField]
    public UnityEvent OnBeginDrag;
    [SerializeField]
    public UnityEvent OnEndDrag;

    Vector3 _startVc;
    Vector3 _endVc;
    public float v;
    private void Awake()
    {
        Ins = this;
    }
    private void Start()
    {
        zPosition = mainCamera.WorldToScreenPoint(transform.position).z;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startVc = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            if (!isMouseDragging)
            {
                BeginDrag();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
            _endVc = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (isMouseDragging)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPosition);
            transform.position = mainCamera.ScreenToWorldPoint(position + new Vector3(offSet.x, offSet.y));
        }
        v = (_startVc.y - _endVc.y) * 0.5f;
    }
    public void BeginDrag()
    {
        OnBeginDrag.Invoke();
        isMouseDragging = true;
        offSet = mainCamera.WorldToScreenPoint(transform.position) - Input.mousePosition;
    }
    public void EndDrag()
    {
        OnEndDrag.Invoke();
        isMouseDragging = false;
    }


}
