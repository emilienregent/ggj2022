using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private MovementController _playerMovementController;
    //private SwipeDetector _playerSwipeDetector;
    private PowerUpBehavior _playerPowerUpBehavior;
    private VirtualJoystickInput _playerVirtualJoystick;

    public MovementController PlayerMovementController { get => _playerMovementController; set => _playerMovementController = value; }
    //public SwipeDetector PlayerSwipeDetector { get => _playerSwipeDetector; set => _playerSwipeDetector = value; }
    public PowerUpBehavior PlayerPowerUpBehavior { get => _playerPowerUpBehavior; set => _playerPowerUpBehavior = value; }
    public VirtualJoystickInput PlayerVirtualJoystick { get => _playerVirtualJoystick; set => _playerVirtualJoystick = value; }

    //public InputUIController PlayerUIController;

    private void Awake()
    {
        PlayerMovementController = GetComponent<MovementController>();

        if(PlayerMovementController == null)
        {
            Debug.LogError("No MovementController found in the Player's components");
        }

        //PlayerSwipeDetector = GetComponent<SwipeDetector>();

        //if(PlayerSwipeDetector == null)
        //{
        //    Debug.LogError("No SwipeDetector found in the Player's components");
        //}

        PlayerVirtualJoystick = GetComponent<VirtualJoystickInput>();

        if(PlayerVirtualJoystick == null)
        {
            Debug.LogError("No VirtualJoystickHandler found in the Player's components");
        }

        PlayerPowerUpBehavior = GetComponent<PowerUpBehavior>();

        if(PlayerPowerUpBehavior == null)
        {
            Debug.LogError("No PowerUpBehavior found in the Player's components");
        }

        //PlayerSwipeDetector.OnSwipeUpHandler += SetNextDirectionUp;
        //PlayerSwipeDetector.OnSwipeRightHandler += SetNextDirectionRight;
        //PlayerSwipeDetector.OnSwipeDownHandler += SetNextDirectionDown;
        //PlayerSwipeDetector.OnSwipeLeftHandler += SetNextDirectionLeft;

        // Do something when eaten
        GameManager.Instance.PacmanDying += Respawn;
    }

    private void Update()
    {
        if(PlayerVirtualJoystick.CurrentDirection != DirectionEnum.None && PlayerMovementController.CurrentDirection != PlayerVirtualJoystick.CurrentDirection)
        {
            PlayerMovementController.SetNextDirection(PlayerVirtualJoystick.CurrentDirection);
        }
    }

    private void OnDestroy()
    {
        //PlayerSwipeDetector.OnSwipeUpHandler -= SetNextDirectionUp;
        //PlayerSwipeDetector.OnSwipeRightHandler -= SetNextDirectionRight;
        //PlayerSwipeDetector.OnSwipeDownHandler -= SetNextDirectionDown;
        //PlayerSwipeDetector.OnSwipeLeftHandler -= SetNextDirectionLeft;

        GameManager.Instance.PacmanDying -= Respawn;
    }

    private void SetNextDirectionUp()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Up);

        //try
        //{
        //    PlayerUIController.UpdateInputFeedback(DirectionEnum.Up);
        //} catch (Exception e)
        //{
        //    Debug.LogException(e);
        //}
    }

    private void SetNextDirectionRight()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Right);

        //try
        //{
        //    PlayerUIController.UpdateInputFeedback(DirectionEnum.Right);
        //} catch (Exception e)
        //{
        //    Debug.LogException(e);
        //}
    }

    private void SetNextDirectionDown()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Down);

        //try
        //{
        //    PlayerUIController.UpdateInputFeedback(DirectionEnum.Down);
        //} catch (Exception e)
        //{
        //    Debug.LogException(e);
        //}
    }

    private void SetNextDirectionLeft()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Left);

        //try
        //{
        //    PlayerUIController.UpdateInputFeedback(DirectionEnum.Left);
        //} catch(Exception e)
        //{
        //    Debug.LogException(e);
        //}
    }

    private void Respawn()
    {
        SetNextDirectionLeft();
        PlayerMovementController.ResetMovement();
    }
}
