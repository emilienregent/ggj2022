using System;
using UnityEngine;

/// <summary>
/// Manage phantom navigation
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class PhantomMovementController : MovementController
{
    // Coefficient applyied to speed
    protected const float SC_PHANTOM_NORMAL = 0.75f;
    protected const float SC_PHANTOM_FRIGHTENED = 0.50f;
    protected const float SC_PHANTOM_WARP = 0.40f;

    public event Action spawnReached;

    [HideInInspector]
    public bool canTriggerSpawn;

    public Transform spawnDestination;
    public Transform fallbackDestination;

    private Quaternion _startRotation;
    private Quaternion _startLocalRotation;

    protected DirectionEnum _defaultDirection = DirectionEnum.Left;

    protected override void Awake() {
        base.Awake();
        _startRotation = transform.rotation;
        _startLocalRotation = transform.localRotation;
    }

    public override void EvaluateNextDirection()
    {
        if (canTriggerSpawn && CurrentNode.transform.position.Equals(spawnDestination.position))
        {
            spawnReached?.Invoke();
        }

        // Reaching intersection need destination reevaluation
        if (CurrentNode.IsIntersection)
        {
            // Check for target position to update direction
            ReachIntersection();
        }
        // Corner force to follow the path
        else if (CurrentNode.IsCorner)
        {
            SetNextDirectionInCorner();
        }
        else if (CurrentNode.IsDeadEnd)
        {
            ReverseDirection();
        }

        // Apply change of direction on destination
        base.EvaluateNextDirection();
    }

    protected virtual void SetNextDirectionInCorner()
    {
        var direction = CurrentNode.GetDirectionOut(CurrentDirection);

        SetNextDirection(direction);
    }

    protected override void FallbackNextDirection()
    {
        // If everything else has failed we move into a random direction
        SetRandomDirection();

        base.FallbackNextDirection();
    }

    /// <summary>
    /// Compare position to target to find new direction
    /// <remarks>Can't use the direction where we come from</remarks>
    /// </summary>
    public void SetDirectionToDestination(Vector3 destination)
    {
        DirectionEnum direction = DirectionEnum.None;
        NodeController node;

        // Target is above us
        if (destination.z > transform.position.z
            && CurrentDirection != DirectionEnum.Down
            && TryGetNextNode(DirectionEnum.Up, out node))
        {
            direction = DirectionEnum.Up;
        }
        // Target is below us
        else if (destination.z < transform.position.z
            && CurrentDirection != DirectionEnum.Up
            && TryGetNextNode(DirectionEnum.Down, out node))
        {
            direction = DirectionEnum.Down;
        }
        // Target is on our left
        else if (destination.x < transform.position.x
            && CurrentDirection != DirectionEnum.Right
            && TryGetNextNode(DirectionEnum.Left, out node))
        {
            direction = DirectionEnum.Left;
        }
        // Target is on our right
        else if (destination.x > transform.position.x
            && CurrentDirection != DirectionEnum.Left
            && TryGetNextNode(DirectionEnum.Right, out node))
        {
            direction = DirectionEnum.Right;
        }

        // If we end up with a valid direction
        if (direction != DirectionEnum.None)
        {
            SetNextDirection(direction);
        }
    }

    protected override void UpdateRotation()
    {
        // Pas d'bras, pas d'chocolat
        if(Model == null)
        {
            return;
        }

        Quaternion rotation;
        switch(CurrentDirection)
        {
            case DirectionEnum.Down:
                rotation = Quaternion.Euler(270, 90, 270);
                break;
            case DirectionEnum.Up:
                rotation = Quaternion.Euler(270, 90, 90);
                break;
            case DirectionEnum.Left:
                rotation = Quaternion.Euler(270, 270, 0);
                break;
            case DirectionEnum.Right:
                rotation = Quaternion.Euler(270, 90, 0);
                break;
            default:
                rotation = Quaternion.Euler(270, 270, 270);
                break;
        }
      
        Model.localRotation = rotation;
    }

    public override void ResetMovement()
    {
        SetStartingNode();
        
        transform.rotation = _startRotation;
        transform.localRotation = _startLocalRotation;

        CurrentDirection = _defaultDirection;
        NextDirection = _defaultDirection;

        canTriggerSpawn = true;
        UpdateRotation();
        SetNormalSpeed();
        EvaluateNextDirection();
    }

    protected override void SetStartingNode()
    {
        Vector3 spawnPoint = StartingNode.transform.position;

        spawnPoint.y = transform.position.y;

        transform.position = spawnPoint;

        CurrentNode = StartingNode;
    }

    protected override void SetNormalSpeed()
    {
        SetCurrentSpeed(SC_PHANTOM_NORMAL);
    }

    protected override void SetFrightenedSpeed()
    {
        SetCurrentSpeed(SC_PHANTOM_FRIGHTENED);
    }
}