using UnityEngine;

public enum MovementMode
{
    None,

    Spawn,
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

    [Header("Phantom house rules")]
    [Tooltip("Order index to leave the house. Lowest index first. 0 means already out.")]
    public int leaveIndex;
    [Tooltip("How many pellet have to be collected before leaving the phantom house")]
    public int pelletLimit;

    private int _pelletCount;

    private PhantomGraphicController _phantomGraphicController;
    protected PhantomMovementController _phantomMovementController;

    private int _scatterModeCount = 0;
    private float _currentModeStartTime = 0f;
    private float _frightenedModeStartTime = 0f;
    private MovementMode _currentMode = MovementMode.None;
    private MovementMode _previousMode = MovementMode.None;

    private bool CanChase { get { return Time.time - _currentModeStartTime >= GetScatterDuration() && _currentMode == MovementMode.Scatter; } }
    private bool CanScatter { get { return Time.time - _currentModeStartTime >= CHASE_DURATION && _currentMode == MovementMode.Chase && _scatterModeCount < SCATTER_TRESHOLD; } }
    private bool IsNextToLeave { get { return 4 - leaveIndex == GameManager.Instance.PhantomInHouse; } }

    private void Awake()
    {
        _phantomGraphicController = GetComponent<PhantomGraphicController>();
        _phantomMovementController = GetComponent<PhantomMovementController>();

        UnityEngine.Assertions.Assert.IsNotNull(_phantomGraphicController, "No PhantomGraphicController found in the Phantom's components");
        UnityEngine.Assertions.Assert.IsNotNull(_phantomMovementController, "No PhantomMovementController found in the Phantom's components");

        _phantomMovementController.spawnReached += LeaveSpawnMode;
        _phantomMovementController.intersectionReached += SetNewDirection;

        GameManager.Instance.PacmanDying += Respawn;

        if (leaveIndex > 0)
        {
            EnterPhantomHouse();
        }
        else
        {
            LeaveSpawnMode();
        }
    }

    private void Start()
    {
        PowerUpEvents.Instance.TimeIntervalElapsed += UpdateMode;
    }

    private void OnDestroy()
    {
        _phantomMovementController.spawnReached -= LeaveSpawnMode;
        _phantomMovementController.intersectionReached -= SetNewDirection;

        GameManager.Instance.PacmanDying -= Respawn;
        GameManager.Instance.PelletCollected -= IncrementPelletCount;

        PowerUpEvents.Instance.TimeIntervalElapsed -= UpdateMode;
        PowerUpEvents.Instance.PowerUpPhaseStarting -= EnterFrightenedMode;
        PowerUpEvents.Instance.PowerUpPhaseEnding -= LeaveFrightenedMode;
    }

    private void IncrementPelletCount()
    {
        if (IsNextToLeave)
        {
            _pelletCount++;

            Debug.Log($"Still {pelletLimit - _pelletCount} pellet for {gameObject.name} to leave the house.");

            if (_pelletCount >= pelletLimit)
            {
                LeavePhantomHouse();
            }
        }
    }

    private void EnterPhantomHouse()
    {
        GameManager.Instance.EnterPhantomHouse();

        GameManager.Instance.PelletCollected += IncrementPelletCount;

        SetMode(MovementMode.None);
    }

    private void LeavePhantomHouse()
    {
        GameManager.Instance.LeavePhantomHouse();

        GameManager.Instance.PelletCollected -= IncrementPelletCount;

        Debug.Log($"{gameObject.name} is leaving the house !");

        EnterSpawnMode();
    }

    private void SetNewDirection()
    {
        if (_currentMode == MovementMode.Frightened)
        {
            _phantomMovementController.SetRandomDirection();
        }
        else
        {
            Vector3 destination = _currentMode == MovementMode.Spawn ? Vector3.forward * 1000f
                : _currentMode == MovementMode.Chase ? GetDestination() : _phantomMovementController.fallbackDestination.position;

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

    private void EnterSpawnMode()
    {
        SetMode(MovementMode.Spawn);
        SetNewDirection();

        Debug.Log($"Enter mode <color=yellow>{_currentMode}</color> on {gameObject.name}");
    }

    private void LeaveSpawnMode()
    {
        _phantomMovementController.canTriggerSpawn = false;

        // From now on phantom can be frightened
        PowerUpEvents.Instance.PowerUpPhaseStarting -= EnterFrightenedMode;
        PowerUpEvents.Instance.PowerUpPhaseStarting += EnterFrightenedMode;
        PowerUpEvents.Instance.PowerUpPhaseEnding -= LeaveFrightenedMode;
        PowerUpEvents.Instance.PowerUpPhaseEnding += LeaveFrightenedMode;

        EnterScatterMode();
    }

    private void EnterChaseMode()
    {
        SetMode(MovementMode.Chase);

        _currentModeStartTime = Time.time;

        Debug.Log($"Enter mode <color=yellow>{_currentMode}</color> on {gameObject.name}");
    }

    private void EnterScatterMode()
    {
        SetMode(MovementMode.Scatter);

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
        if (_currentMode == MovementMode.Frightened)
        {
            return;
        }

        SetMode(MovementMode.Frightened);

        _currentModeStartTime = Time.time;
        _frightenedModeStartTime = Time.time;

        _phantomMovementController.ReverseDirection();

        Debug.Log($"Enter mode <color=red>{_currentMode}</color> on {gameObject.name}");
    }

    private void LeaveFrightenedMode()
    {
        if (_currentMode != MovementMode.Frightened)
        {
            return;
        }

        MovementMode mode = _previousMode == MovementMode.Chase || _previousMode == MovementMode.Scatter ? _previousMode : MovementMode.Chase;

        SetMode(mode, savePrevious:false);

        //Set start time X seconds before actual Time.time, where X is the paused duration of previous mode
        _currentModeStartTime = Time.time - _frightenedModeStartTime - _currentModeStartTime;

        Debug.Log($"Resume mode <color=blue>{_currentMode}</color> on {gameObject.name}");
    }

    private void SetMode(MovementMode mode, bool savePrevious = true)
    {
        if (savePrevious)
        {
            _previousMode = _currentMode;
        }

        _currentMode = mode;

        _phantomGraphicController.SetMode(mode);
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
        if(GameManager.Instance.CurrentState == GameState.GHOST && other.CompareTag(GameManager.GHOST_TAG))
        {
            // /!\ TODO : CHECKER QUE LE FANTÖME EST CONTROLE PAR LE JOUEUR /!\
            // /!\ TODO : SI C'EST TOUJOURS BLINKY, CHECKER LE NOM DU GO ? /!\
            GameManager.Instance.EatPhantom();
            Respawn();
        }

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

        if (leaveIndex > 0)
        {
            EnterPhantomHouse();
        }
        else
        {
            LeaveSpawnMode();
        }
    }
}