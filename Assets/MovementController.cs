using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DirectionEnum { None, Top, Right, Bottom, Left};
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
        NextDirection = DirectionEnum.None;

        Agent = GetComponent<NavMeshAgent>();

        SetNewDestination(StartingNode.NodeLeft);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Agent.pathStatus == NavMeshPathStatus.PathComplete || Agent.remainingDistance < 0.1f)
        {
            NodeController destination;

            // Get new destination
            if(NextDirection != DirectionEnum.None)
            {
                destination = FindNextNode(NextDirection);
                if(destination == null)
                {
                    destination = FindNextNode(CurrentDirection);
                }
            } else
            {
                destination = FindNextNode(CurrentDirection);
            }

            if(destination != null)
            {
                SetNewDestination(destination);

            // Fake movement input
            } else
            {
                DirectionEnum[] availableDirections = new DirectionEnum[4] { DirectionEnum.Top, DirectionEnum.Right, DirectionEnum.Bottom, DirectionEnum.Left };
                NextDirection = availableDirections[Random.Range(0, availableDirections.Length)];
            }
        }



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
