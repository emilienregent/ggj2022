﻿using System.Collections.Generic;
using UnityEngine;

public struct Path
{
    public int PelletCount;
    public bool HasPhantom;
    public NodeController Start;
    public List<NodeController> Nodes;
}

public class PacManMovementController : MovementController
{
    private Dictionary<DirectionEnum, Path> _availablePaths = new Dictionary<DirectionEnum, Path>();

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

        if (preferredDirection != DirectionEnum.None)
        {
            SetNextDirection(preferredDirection);
        }
        else
        {
            SetNextDirection(CurrentNode.GetRandomDirection());
        }
    }

    private DirectionEnum GetPreferredDirection()
    {
        int count = 0;
        int maximumLength = 0;
        int maximumPellet = 0;
        int maximumWeight = -1;
        DirectionEnum preferredDirection = DirectionEnum.None;

        foreach (KeyValuePair<DirectionEnum, Path> pair in _availablePaths)
        {
            Path path = pair.Value;
            DirectionEnum direction = pair.Key;

            int directionWeight = 0;

            // Differentiate paths from their length first
            if (path.Nodes.Count > maximumLength)
            {
                count++;
                directionWeight += count;

                maximumLength = path.Nodes.Count;
            }

            // If no phantom, aim for the most pellets
            if (!path.HasPhantom)
            {
                directionWeight += _availablePaths.Count;

                if (path.PelletCount > maximumPellet)
                {
                    directionWeight += path.PelletCount * 10;

                    maximumPellet = path.PelletCount;
                }
            }
            // Ensure to chase phantom
            else if (PowerUpBehavior.IsEnabled)
            {
                directionWeight += 1000;
            }

            // Keep direction as last differientator
            if (direction == CurrentDirection)
            {
                directionWeight *= 2;
            }

            if (directionWeight > maximumWeight)
            {
                preferredDirection = direction;
                maximumWeight = directionWeight;
            }
        }

        return preferredDirection;
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
            path.HasPhantom = nextNode.HasPhantom();

            if (nextNode.PickupItem != null && nextNode.PickupItem.IsEnabled)
            {
                path.PelletCount++;
            }

            if (!path.HasPhantom)
            {
                SetNextPathNode(nextNode, direction, ref path);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance.CurrentState == GameState.GHOST)
        {
            foreach (Path path in _availablePaths.Values)
            {
                Gizmos.color = path.HasPhantom ? Color.red : Color.green;

                foreach(NodeController node in path.Nodes)
                {
                    Gizmos.DrawCube(node.transform.position, Vector3.one);
                }
            }
        }
    }
}