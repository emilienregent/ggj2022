using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private MovementController _movementController;
    private SwipeDetector _swipeDetector;

    public MovementController MovementController { get => _movementController; set => _movementController = value; }
    public SwipeDetector SwipeDetector { get => _swipeDetector; set => _swipeDetector = value; }

    private void Awake()
    {
        MovementController = GetComponent<MovementController>();

        if(MovementController == null)
        {
            Debug.LogError("No MovementController found in the Player's components");
        }

        SwipeDetector = GetComponent<SwipeDetector>();

        if(SwipeDetector == null)
        {
            Debug.LogError("No SwipeDetector found in the Player's components");
        }

        SwipeDetector.OnSwipeUpHandler += SetNextDirectionToTop;
        SwipeDetector.OnSwipeRightHandler += SetNextDirectionToRight;
        SwipeDetector.OnSwipeDownHandler += SetNextDirectionToBottom;
        SwipeDetector.OnSwipeLeftHandler += SetNextDirectionToLeft;
    }

    private void OnDestroy()
    {
        SwipeDetector.OnSwipeUpHandler -= SetNextDirectionToTop;
        SwipeDetector.OnSwipeRightHandler -= SetNextDirectionToRight;
        SwipeDetector.OnSwipeDownHandler -= SetNextDirectionToBottom;
        SwipeDetector.OnSwipeLeftHandler -= SetNextDirectionToLeft;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetNextDirectionToTop()
    {
        MovementController.NextDirection = DirectionEnum.Top;
    }

    private void SetNextDirectionToRight()
    {
        MovementController.NextDirection = DirectionEnum.Right;
    }

    private void SetNextDirectionToBottom()
    {
        MovementController.NextDirection = DirectionEnum.Bottom;
    }

    private void SetNextDirectionToLeft()
    {
        MovementController.NextDirection = DirectionEnum.Left;
    }
}
