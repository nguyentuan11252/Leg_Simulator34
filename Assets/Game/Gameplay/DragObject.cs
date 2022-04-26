using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{

    /*[Header("Swipe Move Setting")]
    public float speedSwipeMove;
    public float swipeWidth;

    Vector3 startPlayerPos;

    bool isSwipeMove;
    float swipeScale;
    float screenWidth;

    [Header("Mouse Move")]
    Vector3 startMousePos;
    Vector3 preMousePos;
    Vector3 currentMousePos;

    //y
    [Header("Swipe Move Setting Y")]
    public float speedSwipeMoveY;
    public float swipeHeight;

    Vector3 startPlayerPosY;

    bool isSwipeMoveY;
    float swipeScaleY;
    float screenHeight;

    [Header("Mouse MoveY")]
    Vector3 startMousePosY;
    Vector3 preMousePosY;
    Vector3 currentMousePosY;

    private void Awake()
    {

        screenWidth = Screen.width;
        swipeScale = swipeWidth / screenWidth;

        screenHeight = Screen.height;
        swipeScaleY = swipeHeight / screenHeight;
    }
    private void Update()
    {
        MouseMovePlayer();
    }

    void MouseMovePlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            preMousePos = startMousePos = Input.mousePosition;
            startPlayerPos = transform.position;
            //
            preMousePosY = startMousePosY = Input.mousePosition;
            startPlayerPosY = transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            currentMousePos = Input.mousePosition;
            if (currentMousePos.x != preMousePos.x)
            {
                MouseMovement(currentMousePos.x - startMousePos.x);
                preMousePos = currentMousePos;
            }
            //y
            currentMousePosY = Input.mousePosition;
            if(currentMousePosY.y != preMousePosY.y)
            {
                MouseMovement(currentMousePosY.y - startMousePosY.y);
                preMousePosY = currentMousePosY;
            }
        }
    }
    void MouseMovement(float moveDistance)
    {
        float playerPosX = moveDistance*  speedSwipeMove* swipeScale;
        float playerPosY = moveDistance * speedSwipeMoveY * swipeScaleY;
        Vector3 parentPos = transform.position;
        parentPos.x = startPlayerPos.x + playerPosX;
        parentPos.y = startPlayerPosY.y + playerPosY;
        transform.position = parentPos;
        if (isSwipeMove)
        {
            Debug.Log("test: " + parentPos);
            
        }
        
    }*/
    public GameObject footLeg;
    private void Update()
    {
        footLeg.transform.position = transform.position;
    }
}
