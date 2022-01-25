using UnityEngine;

public enum MovementMode
{
    None,

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
    private float _frightenedModeStartTime = 0f;
    private MovementMode _currentMode = MovementMode.None;
    private MovementMode _previousMode = MovementMode.None;

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
        GameManager.Instance.PacmanDying += Respawn;

        EnterScatterMode();
    }

    private void Start()
    {
        GameManager.Instance.TimeIntervalElapsed += UpdateMode;

        GameManager.Instance.PowerUpPhaseStarting += EnterFrightenedMode;
        GameManager.Instance.PowerUpPhaseEnding += ResumePreviousMode;
    }

    private void OnDestroy()
    {
        _phantomMovementController.intersectionReached -= SetNewDirection;
        GameManager.Instance.PacmanDying -= Respawn;

        GameManager.Instance.TimeIntervalElapsed -= UpdateMode;
        GameManager.Instance.PowerUpPhaseStarting -= EnterFrightenedMode;
        GameManager.Instance.PowerUpPhaseEnding -= ResumePreviousMode;
    }

    private void SetNewDirection()
    {
        if (_currentMode == MovementMode.Frightened)
        {
            _phantomMovementController.SetRandomDirection();
        }
        else
        {
            Vector3 destination = _currentMode == MovementMode.Chase ? GetDestination() : _phantomMovementController.fallbackDestination.position;

            _phantomMovementController.SetDirectionToDestination(destination);
        }
    }

    private void UpdateMode()
    {
        if (_currentMode == MovementMode.Frightened)
        {
            return;
        }

        float elapsedTime = Time.time - _currentModeStartTime;

        switch (_currentMode)
        {
            case MovementMode.Chase:
                if (CanScatter)
                {
                    EnterScatterMode();
                }
            break;
            case MovementMode.Scatter:
                if (CanChase)
                {
                    EnterChaseMode();
                }
            break;
        }
    }

    private void EnterChaseMode()
    {
        _previousMode = _currentMode;
        _currentMode = MovementMode.Chase;

        _currentModeStartTime = Time.time;

        Debug.Log($"Enter mode <color=yellow>{_currentMode}</color> on {gameObject.name}");
    }

    private void EnterScatterMode()
    {
        _previousMode = _currentMode;
        _currentMode = MovementMode.Scatter;

        _currentModeStartTime = Time.time;

        // Count switch to scatter mode but not when we resume it
        if (_previousMode != MovementMode.Frightened)
        {
            _scatterModeCount++;
        }

        Debug.Log($"Enter mode <color=yellow>{_currentMode}</color> on {gameObject.name}");
    }

    private void EnterFrightenedMode()
    {
        _previousMode = _currentMode;
        _currentMode = MovementMode.Frightened;

        _currentModeStartTime = Time.time;
        _frightenedModeStartTime = Time.time;

        Debug.Log($"Enter mode <color=red>{_currentMode}</color> on {gameObject.name}");
    }

    private void ResumePreviousMode()
    {
        MovementMode newMode = _previousMode;

        _previousMode = _currentMode;
        _currentMode = newMode;

        //Set start time X seconds before actual Time.time, where X is the paused duration of previous mode
        _currentModeStartTime = Time.time - _frightenedModeStartTime - _currentModeStartTime;

        Debug.Log($"Resume mode <color=blue>{_currentMode}</color> on {gameObject.name}");
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
        return target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.PACMAN_TAG))
        {
            if (_currentMode == MovementMode.Frightened)
            {
                GameManager.Instance.EatPhantom();

                Respawn();
            }
            else
            {
                GameManager.Instance.EatPacman();
            }
        }
    }

    private void Respawn()
    {
        _phantomMovementController.ResetMovement();

        ResumePreviousMode();
    }
}