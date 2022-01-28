using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : MonoBehaviour
{

    private MovementController _movementController;
    private PowerUpBehavior _powerUpBehavior;

    public MovementController MovementController { get => _movementController; set => _movementController = value; }
    public PowerUpBehavior PowerUpBehavior { get => _powerUpBehavior; set => _powerUpBehavior = value; }

    private void Awake()
    {
        // Do something when eaten
        GameManager.Instance.PacmanDying += Respawn;
    }

    private void OnDestroy()
    {
        GameManager.Instance.PacmanDying -= Respawn;
    }

    private void Start()
    {
        LoadControllerComponents();
    }

    private void Respawn()
    {
        MovementController.SetNextDirection(DirectionEnum.Left);
        MovementController.ResetMovement();
    }

    private void LoadControllerComponents()
    {
        MovementController = GetComponent<MovementController>();

        if (MovementController == null)
        {
            Debug.LogError("No MovementController found in the PacMan components");
        }

        PowerUpBehavior = GetComponent<PowerUpBehavior>();

        if (PowerUpBehavior == null)
        {
            Debug.LogError("No PowerUpBehavior found in the PacMan components");
        }
    }

}
