using UnityEngine;

public class PacManMovementController : MovementController
{
    private Vector3 _currentDestination;

    protected override void OnBeforeUpdate()
    {
        if (Mathf.Approximately(transform.position.x, _currentDestination.x) && Mathf.Approximately(transform.position.z, _currentDestination.z))
        {
            SetCurrentDestination();
        }
    }

    public void SetCurrentDestination()
    {
        GameObject pellet = GameManager.Instance.GetRandomPellet();

        _currentDestination = pellet.transform.position;
    }

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
    public void SetDirectionToDestination()
    {
        DirectionEnum direction = DirectionEnum.None;
        DirectionEnum directionWithoutPhantom = GetDirectionWithoutPhantom();
        NodeController node;

        // Target is above us
        if (_currentDestination.z > transform.position.z
            && CurrentDirection != DirectionEnum.Down
            && TryGetNextNode(DirectionEnum.Up, out node)
            && directionWithoutPhantom.HasFlag(DirectionEnum.Up))
        {
            direction = DirectionEnum.Up;
        }
        // Target is below us
        else if (_currentDestination.z < transform.position.z
            && CurrentDirection != DirectionEnum.Up
            && TryGetNextNode(DirectionEnum.Down, out node)
            && directionWithoutPhantom.HasFlag(DirectionEnum.Down))
        {
            direction = DirectionEnum.Down;
        }
        // Target is on our left
        else if (_currentDestination.x < transform.position.x
            && CurrentDirection != DirectionEnum.Right
            && TryGetNextNode(DirectionEnum.Left, out node)
            && directionWithoutPhantom.HasFlag(DirectionEnum.Left))
        {
            direction = DirectionEnum.Left;
        }
        // Target is on our right
        else if (_currentDestination.x > transform.position.x
            && CurrentDirection != DirectionEnum.Left
            && TryGetNextNode(DirectionEnum.Right, out node)
            && directionWithoutPhantom.HasFlag(DirectionEnum.Right))
        {
            direction = DirectionEnum.Right;
        }

        // If we end up with a valid direction
        if (direction != DirectionEnum.None)
        {
            SetNextDirection(direction);
        }
        else
        {
            ReverseDirection();
        }
    }

    private DirectionEnum GetDirectionWithoutPhantom()
    {
        DirectionEnum mask = DirectionEnum.None;

        foreach (DirectionEnum direction in CurrentNode.Directions)
        {
            if (!NodeHasPhantom(CurrentNode, direction))
            {
                mask |= direction;
            }
        }

        return mask;
    }

    private bool NodeHasPhantom(NodeController node, DirectionEnum direction)
    {
        if (node.HasPhantom())
        {
            return true;
        }

        if (TryGetNextNode(direction, out NodeController nextNode))
        {
            return nextNode.HasPhantom();
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_currentDestination + Vector3.up, Vector3.one);
    }
}