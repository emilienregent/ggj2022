using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private MovementController _playerMovementController;
    private PowerUpBehavior _playerPowerUpBehavior;
    private VirtualJoystickInput _playerVirtualJoystick;

    public MovementController PlayerMovementController { get => _playerMovementController; set => _playerMovementController = value; }
    public PowerUpBehavior PlayerPowerUpBehavior { get => _playerPowerUpBehavior; set => _playerPowerUpBehavior = value; }
    public VirtualJoystickInput PlayerVirtualJoystick { get => _playerVirtualJoystick; set => _playerVirtualJoystick = value; }

    [Header("Game References")]
    public GameObject CurrentController;
    public MovementController PacMan;
    public PhantomMovementController Blinky;

    //public InputUIController PlayerUIController;

    private void Awake()
    {
        // Do something when eaten
        GameManager.Instance.PacmanDying += Respawn;

        // Do something when the Game State changes
        GameManager.Instance.OnChangeStateHandler += UpdateController;
    }

    private void Start()
    {
        UpdateController();
        LoadControllerComponents();
    }

    private void OnDestroy()
    {
        GameManager.Instance.PacmanDying -= Respawn;
        GameManager.Instance.OnChangeStateHandler -= UpdateController;
    }

    private void Update()
    {
        if (PlayerMovementController.CurrentDirection != PlayerVirtualJoystick.CurrentDirection)
        {
            switch (PlayerVirtualJoystick.CurrentDirection)
            {
                case DirectionEnum.Up:
                    SetNextDirectionUp();
                    break;

                case DirectionEnum.Right:
                    SetNextDirectionRight();
                    break;
                case DirectionEnum.Down:
                    SetNextDirectionDown();
                    break;
                case DirectionEnum.Left:
                    SetNextDirectionLeft();
                    break;
            }
        }
    }

    private void SetNextDirectionUp()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Up);
    }

    private void SetNextDirectionRight()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Right);
    }

    private void SetNextDirectionDown()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Down);
    }

    private void SetNextDirectionLeft()
    {
        PlayerMovementController.SetNextDirection(DirectionEnum.Left);
    }

    private void Respawn()
    {
        if(GameManager.Instance.CurrentState == GameState.PACMAN)
        {
            SetNextDirectionLeft();
            PlayerMovementController.ResetMovement();
        }
    }

    public void UpdateController()
    {
        switch(GameManager.Instance.CurrentState)
        {
            case GameState.PACMAN:
                CurrentController = PacMan.gameObject;
                LoadControllerComponents();
                break;

            case GameState.GHOST:
                Debug.Log(Blinky);
                CurrentController = Blinky.gameObject;
                LoadControllerComponents();
                break;
        }
    }

    private void LoadControllerComponents()
    {
        switch(GameManager.Instance.CurrentState)
        {
            case GameState.PACMAN:
                PlayerMovementController = CurrentController.GetComponent<MovementController>();

                if (PlayerMovementController == null)
                {
                    Debug.LogError("No MovementController found in the Player's (PacMan) components");
                }

                PlayerVirtualJoystick = CurrentController.GetComponent<VirtualJoystickInput>();

                if (PlayerVirtualJoystick == null)
                {
                    Debug.LogError("No VirtualJoystickHandler found in the Player's (PacMan) components");
                }

                PlayerPowerUpBehavior = CurrentController.GetComponent<PowerUpBehavior>();

                if (PlayerPowerUpBehavior == null)
                {
                    Debug.LogError("No PowerUpBehavior found in the Player's (PacMan) components");
                }
                break;

            case GameState.GHOST:
                PlayerMovementController = CurrentController.GetComponent<PhantomMovementController>();

                if (PlayerMovementController == null)
                {
                    Debug.LogError("No PhantomMovementController found in the Player's (Blinky) components");
                }
                break;
        }

    }
}
