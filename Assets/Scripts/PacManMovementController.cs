using UnityEngine;

public class PacManMovementController : MovementController
{
    public override void EvaluateNextDirection()
    {
        if (GameManager.Instance.CurrentState == GameState.GHOST)
        {
            // Reaching intersection need destination reevaluation
            if (CurrentNode.IsIntersection)
            {
                // Check for target position to update direction
                ReachIntersection();
            }
            // Corner force to follow the path
            else if (CurrentNode.IsCorner)
            {
                var direction = CurrentNode.GetDirectionOut(CurrentDirection);

                SetNextDirection(direction);
            }
        }

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
}