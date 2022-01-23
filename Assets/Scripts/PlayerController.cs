using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private MovementController _playerMovementController;
    private SwipeDetector _playerSwipeDetector;

    public MovementController PlayerMovementController { get => _playerMovementController; set => _playerMovementController = value; }
    public SwipeDetector PlayerSwipeDetector { get => _playerSwipeDetector; set => _playerSwipeDetector = value; }

    public InputUIController PlayerUIController;

    private void Awake()
    {
        PlayerMovementController = GetComponent<MovementController>();

        if(PlayerMovementController == null)
        {
            Debug.LogError("No MovementController found in the Player's components");
        }

        PlayerSwipeDetector = GetComponent<SwipeDetector>();

        if(PlayerSwipeDetector == null)
        {
            Debug.LogError("No SwipeDetector found in the Player's components");
        }

        PlayerSwipeDetector.OnSwipeUpHandler += SetNextDirectionUp;
        PlayerSwipeDetector.OnSwipeRightHandler += SetNextDirectionRight;
        PlayerSwipeDetector.OnSwipeDownHandler += SetNextDirectionDown;
        PlayerSwipeDetector.OnSwipeLeftHandler += SetNextDirectionLeft;
    }

    private void OnDestroy()
    {
        PlayerSwipeDetector.OnSwipeUpHandler -= SetNextDirectionUp;
        PlayerSwipeDetector.OnSwipeRightHandler -= SetNextDirectionRight;
        PlayerSwipeDetector.OnSwipeDownHandler -= SetNextDirectionDown;
        PlayerSwipeDetector.OnSwipeLeftHandler -= SetNextDirectionLeft;
    }

    private void SetNextDirectionUp()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Up);

        try
        {
            PlayerUIController.UpdateInputFeedback(DirectionEnum.Up);
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SetNextDirectionRight()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Right);

        try
        {
            PlayerUIController.UpdateInputFeedback(DirectionEnum.Right);
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SetNextDirectionDown()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Down);

        try
        {
            PlayerUIController.UpdateInputFeedback(DirectionEnum.Down);
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SetNextDirectionLeft()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Left);

        try
        {
            PlayerUIController.UpdateInputFeedback(DirectionEnum.Left);
        } catch(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
