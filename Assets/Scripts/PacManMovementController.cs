using System.Collections.Generic;
using UnityEngine;

public struct Path
{
    public bool IsValid;
    public NodeController Start;
    public List<NodeController> Nodes;
}

public class PacManMovementController : MovementController
{
    private const int INVALID_DIRECTION_DURATION = 4;

    private Vector3 _finalDestination;
    private Dictionary<DirectionEnum, Path> _availablePaths = new Dictionary<DirectionEnum, Path>();
    private int _invalidDirectionMask = 0;
    private int _invalidDirectionEvaluationCount = 0;

    protected override void OnBeforeUpdate()
    {
        if (Mathf.Approximately(transform.position.x, _finalDestination.x) && Mathf.Approximately(transform.position.z, _finalDestination.z))
        {
            SetCurrentDestination();
        }
    }

    public void SetCurrentDestination()
    {
        GameObject pellet = GameManager.Instance.GetRandomPellet();

        _finalDestination = pellet.transform.position;
    }

    public override void EvaluateNextDirection()
    {
        if (GameManager.Instance.CurrentState == GameState.GHOST)
        {
            FindPath();

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
    /// </summary>
    public void SetDirectionToDestination()
    {
        DirectionEnum preferredDirection = GetPreferredDirection();

        if (IsDirectionValid(preferredDirection))
        {
            SetNextDirection(preferredDirection);
        }
        else
        {
            _invalidDirectionMask |= (int)preferredDirection;

            SetNextDirection(CurrentDirection);
        }

        if (++_invalidDirectionEvaluationCount >= INVALID_DIRECTION_DURATION)
        {
            _invalidDirectionMask = 0;
            _invalidDirectionEvaluationCount = 0;

            //SetDirectionToDestination();
        }
    }

    private DirectionEnum GetPreferredDirection()
    {
        // Target is above us
        if ((_invalidDirectionMask & (int)DirectionEnum.Up) == 0 && _finalDestination.z > transform.position.z)
        {
            return DirectionEnum.Up;
        }
        // Target is on our right
        else if ((_invalidDirectionMask & (int)DirectionEnum.Right) == 0 && _finalDestination.x > transform.position.x)
        {
            return DirectionEnum.Right;
        }
        // Target is below us
        else if ((_invalidDirectionMask & (int)DirectionEnum.Down) == 0 && _finalDestination.z < transform.position.z)
        {
            return DirectionEnum.Down;
        }
        // Target is on our left
        else if ((_invalidDirectionMask & (int)DirectionEnum.Left) == 0 && _finalDestination.x < transform.position.x)
        {
            return DirectionEnum.Left;
        }

        return DirectionEnum.None;
    }

    private bool IsDirectionValid(DirectionEnum direction)
    {
        if (_availablePaths.TryGetValue(direction, out Path path))
        {
            return path.IsValid;
        }

        return false;
    }

    private void FindPath()
    {
        _availablePaths.Clear();

        foreach (DirectionEnum direction in CurrentNode.Directions)
        {
            Path path = new Path() { Nodes = new List<NodeController>() };

            if (CurrentNode.TryGetNextNode(direction, out path.Start))
            {
                SetNextPathNode(path.Start, direction, ref path);

                _availablePaths[direction] = path;
            }
        }
    }

    private void SetNextPathNode(NodeController node, DirectionEnum direction, ref Path path)
    {
        NodeController nextNode;

        if (node.TryGetNextNode(direction, out nextNode))
        {
            path.Nodes.Add(nextNode);
            path.IsValid = !nextNode.HasPhantom();

            if (path.IsValid)
            {
                SetNextPathNode(nextNode, direction, ref path);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance.CurrentState == GameState.GHOST)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_finalDestination + Vector3.up, Vector3.one);

            foreach (Path path in _availablePaths.Values)
            {
                Gizmos.color = path.IsValid ? Color.green : Color.red;

                foreach(NodeController node in path.Nodes)
                {
                    Gizmos.DrawCube(node.transform.position, Vector3.one);
                }
            }
        }
    }
}