using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DirectionEnum { Top, Right, Bottom, Left};
public class MovementController : MonoBehaviour
{

    public float Speed = 4f;
    public NodeController StartingNode;

    private DirectionEnum _currentDirection;
    private DirectionEnum _nextDirection;
    private NodeController _destinationNode;
    private NavMeshAgent _agent;

    public DirectionEnum CurrentDirection { get => _currentDirection; set => _currentDirection = value; }
    public DirectionEnum NextDirection { get => _nextDirection; set => _nextDirection = value; }
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }
    public NodeController DestinationNode { get => _destinationNode; set => _destinationNode = value; }

    // Start is called before the first frame update
    void Start()
    {
        CurrentDirection = DirectionEnum.Left;
        NextDirection = DirectionEnum.Left;

        Agent = GetComponent<NavMeshAgent>();

        SetNewDestination(StartingNode.NodeLeft);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            EvaluateNextDirection();
        }

    }

    public void EvaluateNextDirection()
    {
        // If we can go to the wanted direction, we go with it
        NodeController destination = FindNextNode(NextDirection);
        if (destination != null)
        {
            // We update the current direction since we can go in the direction we want
            CurrentDirection = NextDirection;
        }
        else
        {
            // There is no node available in the direction we wanted, so we keep going in the current direction
            destination = FindNextNode(CurrentDirection);
        }

        // If we have found a destination, we go there
        if (destination != null)
        {
            SetNewDestination(destination);
        }
    }

    public void SetNextDirection(DirectionEnum newDirection)
    {
        NextDirection = newDirection;
        EvaluateNextDirection();
    }

    private void SetNewDestination(NodeController destination)
    {
        DestinationNode = destination;
        Agent.SetDestination(destination.Center.gameObject.transform.position);
    }

    private NodeController FindNextNode(DirectionEnum direction)
    {
        NodeController destination = null;

        switch (direction)
        {
            case DirectionEnum.Top:
                if (DestinationNode.CanMoveTop == true)
                {
                    destination = DestinationNode.NodeTop;
                }
                break;

            case DirectionEnum.Right:
                if (DestinationNode.CanMoveRight == true)
                {
                    destination = DestinationNode.NodeRight;
                }
                break;

            case DirectionEnum.Bottom:
                if (DestinationNode.CanMoveBottom == true)
                {
                    destination = DestinationNode.NodeBottom;
                }
                break;

            case DirectionEnum.Left:
                if (DestinationNode.CanMoveLeft == true)
                {
                    destination = DestinationNode.NodeLeft;
                }
                break;
        }

        return destination;
    }
}
