using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 _fingerDown;
    private Vector2 _fingerUp;

    public bool DetectSwipeOnlyAfterRelease = false;
    public float SwipeThreshold = 20f;

    // Update is called once per frame
    private void FixedUpdate()
    {

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fingerUp = touch.position;
                _fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!DetectSwipeOnlyAfterRelease)
                {
                    _fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                _fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMoveValue() > SwipeThreshold && VerticalMoveValue() > HorizontalMoveValue())
        {
            //Debug.Log("Vertical");
            if (_fingerDown.y - _fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (_fingerDown.y - _fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            _fingerUp = _fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalMoveValue() > SwipeThreshold && HorizontalMoveValue() > VerticalMoveValue())
        {
            //Debug.Log("Horizontal");
            if (_fingerDown.x - _fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (_fingerDown.x - _fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            _fingerUp = _fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    private float VerticalMoveValue()
    {
        return Mathf.Abs(_fingerDown.y - _fingerUp.y);
    }

    private float HorizontalMoveValue()
    {
        return Mathf.Abs(_fingerDown.x - _fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    public void OnSwipeUp()
    {
        OnSwipeUpAction();
    }

    public void OnSwipeRight()
    {
        OnSwipeRightAction();
    }

    public void OnSwipeDown()
    {
        OnSwipeDownAction();
    }

    public void OnSwipeLeft()
    {
        OnSwipeLeftAction();
    }

    public event Action OnSwipeUpHandler;
    public void OnSwipeUpAction()
    {
        OnSwipeUpHandler?.Invoke();
    }

    public event Action OnSwipeRightHandler;
    public void OnSwipeRightAction()
    {
        OnSwipeRightHandler?.Invoke();
    }

    public event Action OnSwipeDownHandler;
    public void OnSwipeDownAction()
    {
        OnSwipeDownHandler?.Invoke();
    }

    public event Action OnSwipeLeftHandler;
    public void OnSwipeLeftAction()
    {
        OnSwipeLeftHandler?.Invoke();
    }
}
