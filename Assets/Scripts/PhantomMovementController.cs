using System;
using UnityEngine;

/// <summary>
/// Manage phantom navigation
/// </summary>
public class PhantomMovementController : MovementController
{
    public event Action IntersectionReached;

    public override void EvaluateNextDirection()
    {
        // Reaching intersection need destination reevaluation
        if (CurrentNode.IsIntersection)
        {
            // Check for target position to update direction
            IntersectionReached?.Invoke();
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
    public void FollowTarget(Transform target)
    {
        DirectionEnum direction = DirectionEnum.None;
        NodeController destination;

        // Target is on our left
        if (target.position.x < transform.position.x
            && CurrentDirection != DirectionEnum.Right
            && TryGetNextNode(DirectionEnum.Left, out destination))
        {
            direction = DirectionEnum.Left;
        }
        // Target is on our right
        else if (target.position.x > transform.position.x
            && CurrentDirection != DirectionEnum.Left
            && TryGetNextNode(DirectionEnum.Right, out destination))
        {
            direction = DirectionEnum.Right;
        }
        // Target is above us
        else if (target.position.z > transform.position.z
            && CurrentDirection != DirectionEnum.Bottom
            && TryGetNextNode(DirectionEnum.Top, out destination))
        {
            direction = DirectionEnum.Top;
        }
        // Target is below us
        else if (target.position.z < transform.position.z
            && CurrentDirection != DirectionEnum.Top
            && TryGetNextNode(DirectionEnum.Bottom, out destination))
        {
            direction = DirectionEnum.Bottom;
        }

        Debug.Log($"Set phantom direction to {direction}");

        // If we can actually move into this direction
        if (TryGetNextNode(direction, out destination))
        {
            SetNextDirection(direction);
        }
    }
}