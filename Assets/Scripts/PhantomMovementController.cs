using System;
using UnityEngine;

/// <summary>
/// Manage phantom navigation
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class PhantomMovementController : MovementController
{
    public event Action intersectionReached;

    public Transform fallbackDestination;

    public override void EvaluateNextDirection()
    {
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

        // Apply change of direction on destination
        base.EvaluateNextDirection();
    }

    /// <summary>
    /// Compare position to target to find new direction
    /// <remarks>Can't use the direction where we come from</remarks>
    /// </summary>
    public void SetDirectionToDestination(Vector3 destination)
    {
        DirectionEnum direction = DirectionEnum.None;
        NodeController node;

        // Target is on our left
        if (destination.x < transform.position.x
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
        // Target is above us
        else if (destination.z > transform.position.z
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

        // If we end up with a valid direction
        if (direction != DirectionEnum.None)
        {
            SetNextDirection(direction);
        }
    }
}