using UnityEngine;

public enum MovementMode
{
    Chase,
    Scatter,
    Frightened
}

/// <summary>
/// Manage phantom general behaviour and orchestrate for decision
/// </summary>
public class PhantomController : MonoBehaviour
{
    private const float CHASE_DURATION = 20f;
    private const float SCATTER_DURATION = 7f;
    private const int SCATTER_TRESHOLD = 4;

    public PlayerController target;

    protected PhantomMovementController _phantomMovementController;

    private int _scatterModeCount = 0;
    private float _currentModeStartTime = 0f;
    private MovementMode _currentMode = MovementMode.Chase;

    private bool CanChase { get { return Time.time - _currentModeStartTime >= GetScatterDuration() && _currentMode == MovementMode.Scatter; } }
    private bool CanScatter { get { return Time.time - _currentModeStartTime >= CHASE_DURATION && _currentMode == MovementMode.Chase && _scatterModeCount < SCATTER_TRESHOLD; } }

    private void Awake()
    {
        _phantomMovementController = GetComponent<PhantomMovementController>();

        if (_phantomMovementController == null)
        {
            Debug.LogError("No PhantomMovementController found in the Phantom's components");
        }

        _phantomMovementController.intersectionReached += SetNewDirection;

        InitializeMode();
    }

    private void Start()
    {
        GameManager.Instance.timeIntervalElapsed += UpdateMode;
    }

    private void OnDestroy()
    {
        _phantomMovementController.intersectionReached -= SetNewDirection;
        GameManager.Instance.timeIntervalElapsed -= UpdateMode;
    }

    private void SetNewDirection()
    {
        Vector3 destination = _currentMode == MovementMode.Chase ? GetDestination() : _phantomMovementController.fallbackDestination.position;

        _phantomMovementController.SetDirectionToDestination(destination);
    }

    private void InitializeMode()
    {
        SetMode(MovementMode.Scatter);
    }

    private void SetMode(MovementMode mode)
    {
        Debug.Log($"Set mode <color=yellow>{mode}</color> on {gameObject.name}");

        _currentMode = mode;
        _currentModeStartTime = Time.time;

        if (mode == MovementMode.Scatter)
        {
            _scatterModeCount++;
        }
    }

    private void UpdateMode()
    {
        float elapsedTime = Time.time - _currentModeStartTime;

        switch (_currentMode)
        {
            case MovementMode.Chase:
                if (CanScatter)
                {
                    SetMode(MovementMode.Scatter);
                }
            break;
            case MovementMode.Scatter:
                if (CanChase)
                {
                    SetMode(MovementMode.Chase);
                }
            break;
            case MovementMode.Frightened:

            break;
        }
    }

    private float GetScatterDuration()
    {
        return _scatterModeCount > 2 ? SCATTER_DURATION - 2f : SCATTER_DURATION;
    }

    /// <summary>
    /// Get a position for destination
    /// Default behaviour set destination as target position
    /// </summary>
    /// <returns>The position phantom will try to reach</returns>
    protected virtual Vector3 GetDestination()
    {
        return target.position;
    }
}