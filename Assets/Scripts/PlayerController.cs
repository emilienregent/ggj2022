using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private MovementController _playerMovementController;
    private SwipeDetector _playerSwipeDetector;

    public MovementController PlayerMovementController { get => _playerMovementController; set => _playerMovementController = value; }
    public SwipeDetector PlayerSwipeDetector { get => _playerSwipeDetector; set => _playerSwipeDetector = value; }

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

        PlayerSwipeDetector.OnSwipeUpHandler += SetNextDirectionToTop;
        PlayerSwipeDetector.OnSwipeRightHandler += SetNextDirectionToRight;
        PlayerSwipeDetector.OnSwipeDownHandler += SetNextDirectionToBottom;
        PlayerSwipeDetector.OnSwipeLeftHandler += SetNextDirectionToLeft;
    }

    private void OnDestroy()
    {
        PlayerSwipeDetector.OnSwipeUpHandler -= SetNextDirectionToTop;
        PlayerSwipeDetector.OnSwipeRightHandler -= SetNextDirectionToRight;
        PlayerSwipeDetector.OnSwipeDownHandler -= SetNextDirectionToBottom;
        PlayerSwipeDetector.OnSwipeLeftHandler -= SetNextDirectionToLeft;
    }

    private void SetNextDirectionToTop()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Top);
    }

    private void SetNextDirectionToRight()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Right);
    }

    private void SetNextDirectionToBottom()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Bottom);
    }

    private void SetNextDirectionToLeft()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Left);
    }
}
