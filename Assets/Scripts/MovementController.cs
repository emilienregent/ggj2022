using System;
using Unity.Collections;
using UnityEngine;

public enum DirectionEnum
{
    None = 0,

    Up = 1 << 0, // 1
    Right = 1 << 1, // 2
    Down = 1 << 2, // 4
    Left = 1 << 3, // 8

    Horizontal = Left | Right, // 10
    Vertical = Up | Down // 5
};

public class MovementController : MonoBehaviour
{
    // Coefficient applyied to speed
    protected const float SC_PACMAN_NORMAL = 0.80f;
    protected const float SC_PACMAN_PELLETS = 0.71f;
    protected const float SC_PACMAN_FRIGHTENED = 0.90f;

    private const int SPEED_REDUCTION_DURATION = 3; // In frame

    public Transform Model;
    public NodeController StartingNode;

    public float Speed = 4f;
    [ReadOnly] public float CurrentSpeed = 0f;

    private DirectionEnum _previousDirection;
    private DirectionEnum _currentDirection;
    private DirectionEnum _nextDirection;
    private NodeController _destinationNode;
    private NodeController _currentNode;
    private bool _hasSpeedReduced = false;
    private int _speedReductionFrameCount = 0;

    public DirectionEnum CurrentDirection { get => _currentDirection; set => _currentDirection = value; }
    public DirectionEnum NextDirection { get => _nextDirection; set => _nextDirection = value; }
    public DirectionEnum PreviousDirection { get => _previousDirection; set => _previousDirection = value; }
    public NodeController DestinationNode { get => _destinationNode; set => _destinationNode = value; }
    public NodeController CurrentNode
    {
        get => _currentNode;
        set
        {
            if (_currentNode != null)
            {
                _currentNode.LeaveNode();
            }

            _currentNode = value;
            _currentNode.EnterNode(this);
        }
    }

    public event Action intersectionReached;

    protected virtual void Awake()
    {
        PowerUpEvents.Instance.PowerUpPhaseStarting += SetFrightenedSpeed;
        PowerUpEvents.Instance.PowerUpPhaseEnding += SetNormalSpeed;

        GameManager.Instance.PelletCollected += SetPelletSpeed;
    }
        
    private void Start()
    {
        ResetMovement();
    }
    
    private void Update()
    {
        // No movement during pause
        if(GameManager.Instance.IsPaused == true)
        {
            return;
        }

        if (DestinationNode != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, DestinationNode.gameObject.transform.position, CurrentSpeed * Time.deltaTime);

            if (Mathf.Approximately(Vector3.Distance(transform.position, DestinationNode.transform.position), 0f))
            {
                CurrentNode = DestinationNode;
                EvaluateNextDirection();
            }
        }

        if (++_speedReductionFrameCount > SPEED_REDUCTION_DURATION)
        {
            _hasSpeedReduced = false;
            SetNormalSpeed();
        }
    }

    public virtual void EvaluateNextDirection()
    {
        // If we can go to the wanted direction, we go with it
        if (TryGetNextNode(NextDirection, out NodeController destination))
        {
            // We update the current direction since we can go in the direction we want
            if(CurrentDirection != NextDirection)
            {
                PreviousDirection = CurrentDirection;
                CurrentDirection = NextDirection;
                UpdateRotation();
            }
            SetNewDestination(destination);
        }
        else
        {
            FallbackNextDirection();
        }
    }

    protected virtual void FallbackNextDirection()
    {
        // There is no node available in the direction we wanted, so we keep going in the current direction
        if (TryGetNextNode(CurrentDirection, out NodeController destination))
        {
            SetNewDestination(destination);
        }
    }

    public virtual void ReverseDirection()
    {
        DirectionEnum oppositeDirection;

        DirectionEnum currentAxis = DirectionEnum.Horizontal.HasFlag(CurrentDirection) ? DirectionEnum.Horizontal : DirectionEnum.Vertical;

        if (currentAxis == DirectionEnum.Horizontal)
        {
            oppositeDirection = CurrentDirection == DirectionEnum.Left ? DirectionEnum.Right : DirectionEnum.Left;
        }
        else
        {
            oppositeDirection = CurrentDirection == DirectionEnum.Up ? DirectionEnum.Down : DirectionEnum.Up;
        }

        if (TryGetNextNode(oppositeDirection, out var node))
        {
            SetNextDirection(oppositeDirection);
        }
        else
        {
            SetRandomDirection();
        }
    }

    public virtual void SetRandomDirection()
    {
        DirectionEnum newDirection = CurrentDirection;

        while (newDirection.Equals(CurrentDirection) && !CurrentNode.IsDeadEnd)
        {
            newDirection = CurrentNode.GetRandomDirection();
        }

        SetNextDirection(newDirection);
    }

    protected void ReachIntersection()
    {
        intersectionReached?.Invoke();
    }

    public void SetNextDirection(DirectionEnum newDirection)
    {
        NextDirection = newDirection;
    }

    private void SetNewDestination(NodeController destination)
    {
        DestinationNode = destination;
        transform.LookAt(DestinationNode.gameObject.transform);
    }

    public bool TryGetNextNode(DirectionEnum direction, out NodeController destination)
    {
        destination = null;

        if (direction == DirectionEnum.None)
        {
            return false;
        }

        switch (direction)
        {
            case DirectionEnum.Up:
                if (CurrentNode.CanMoveUp == true)
                {
                    destination = CurrentNode.NodeUp;
                    return true;
                }
                break;

            case DirectionEnum.Right:
                if (CurrentNode.CanMoveRight == true)
                {
                    destination = CurrentNode.NodeRight;
                    return true;
                }
                break;

            case DirectionEnum.Down:
                if (CurrentNode.CanMoveDown == true)
                {
                    destination = CurrentNode.NodeDown;
                    return true;
                }
                break;

            case DirectionEnum.Left:
                if (CurrentNode.CanMoveLeft == true)
                {
                    destination = CurrentNode.NodeLeft;
                    return true;
                }
                break;
        }

        return false;
    }

    protected virtual void SetNormalSpeed()
    {
        SetCurrentSpeed(SC_PACMAN_NORMAL);
    }

    protected virtual void SetFrightenedSpeed()
    {
        SetCurrentSpeed(SC_PACMAN_FRIGHTENED);
    }

    protected virtual void SetPelletSpeed()
    {
        if (!_hasSpeedReduced)
        {
            _hasSpeedReduced = true;
            _speedReductionFrameCount = 0;

            SetCurrentSpeed(SC_PACMAN_PELLETS);
        }
    }

    protected virtual void SetCurrentSpeed(float speedCoefficient)
    {
        CurrentSpeed = Speed * speedCoefficient;
    }

    protected virtual void UpdateRotation()
    {
        // Pas d'bras, pas d'chocolat
        if (Model == null)
        {
            return;
        }

        Quaternion rotation;
        if (CurrentDirection == DirectionEnum.Right)
        {
            rotation = Quaternion.Euler(0, 180, 270);
        } else
        {
            rotation = Quaternion.Euler(0, 180, 90);
        }
        Model.localRotation = rotation;
    }

    public virtual void ResetMovement()
    {
        transform.position = StartingNode.gameObject.transform.position;

        PreviousDirection = DirectionEnum.Up;
        CurrentDirection = DirectionEnum.Left;
        NextDirection = DirectionEnum.Left;

        UpdateRotation();
        SetNormalSpeed();

        CurrentNode = StartingNode;

        EvaluateNextDirection();
    }

    private void OnDestroy()
    {
        PowerUpEvents.Instance.PowerUpPhaseStarting -= SetFrightenedSpeed;
        PowerUpEvents.Instance.PowerUpPhaseEnding -= SetNormalSpeed;

        GameManager.Instance.PelletCollected -= SetPelletSpeed;
    }
}
