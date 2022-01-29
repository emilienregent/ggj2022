using UnityEngine;

public class PacManController : MonoBehaviour
{
    private PacManMovementController _movementController;
    private PowerUpBehavior _powerUpBehavior;

    public PacManMovementController MovementController { get => _movementController; set => _movementController = value; }
    public PowerUpBehavior PowerUpBehavior { get => _powerUpBehavior; set => _powerUpBehavior = value; }

    private void Awake()
    {
        // Do something when eaten
        GameManager.Instance.PacmanDying += Respawn;
    }

    private void OnDestroy()
    {
        GameManager.Instance.PacmanDying -= Respawn;

        _movementController.intersectionReached -= SetNewDirection;
    }

    private void Start()
    {
        LoadControllerComponents();
    }

    private void SetNewDirection()
    {
        _movementController.SetDirectionToDestination();
    }

    private void Respawn()
    {
        _movementController.SetNextDirection(DirectionEnum.Left);
        _movementController.ResetMovement();
    }

    private void LoadControllerComponents()
    {
        MovementController = GetComponent<PacManMovementController>();

        if (MovementController == null)
        {
            Debug.LogError("No PacManMovementController found in the PacMan components");
        }

        PowerUpBehavior = GetComponent<PowerUpBehavior>();

        if (PowerUpBehavior == null)
        {
            Debug.LogError("No PowerUpBehavior found in the PacMan components");
        }

        _movementController.intersectionReached += SetNewDirection;
    }
}
