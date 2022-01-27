using System;
using UnityEngine;

/// <summary>
/// Manage phantom navigation
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class PhantomMovementController : MovementController
{
    public event Action spawnReached;
    public event Action intersectionReached;

    [HideInInspector]
    public bool canTriggerSpawn;

    public Transform spawnDestination;
    public Transform fallbackDestination;

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
            intersectionReached?.Invoke();
        }
        // Corner force to follow the path
        else if (CurrentNode.IsCorner)
        {
            var direction = CurrentNode.GetDirectionOut(CurrentDirection);

            SetNextDirection(direction);
        }
        else if (CurrentNode.IsDeadEnd)
        {
            ReverseDirection();
        }

        // Apply change of direction on destination
        base.EvaluateNextDirection();
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

    public void SetRandomDirection()
    {
        DirectionEnum newDirection = CurrentDirection;

        while (newDirection.Equals(CurrentDirection) && !CurrentNode.IsDeadEnd)
        {
            newDirection = CurrentNode.GetRandomDirection();
        }

        SetNextDirection(newDirection);
    }

    public void ReverseDirection()
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

    protected override void UpdateRotation()
    {
        // Do nothing for ghost
    }

    public override void ResetMovement()
    {
        Vector3 spawnPoint = StartingNode.transform.position;

        spawnPoint.y = transform.position.y;

        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;

        CurrentNode = StartingNode;

        CurrentDirection = DirectionEnum.Left;
        NextDirection = DirectionEnum.Left;

        canTriggerSpawn = true;

        EvaluateNextDirection();
    }
}