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
    private NodeController _currentNode;

    public DirectionEnum CurrentDirection { get => _currentDirection; set => _currentDirection = value; }
    public DirectionEnum NextDirection { get => _nextDirection; set => _nextDirection = value; }
    public NodeController DestinationNode { get => _destinationNode; set => _destinationNode = value; }
    public NodeController CurrentNode { get => _currentNode; set => _currentNode = value; }

    // Start is called before the first frame update
    void Start()
    {
        CurrentDirection = DirectionEnum.Left;
        NextDirection = DirectionEnum.Left;

        CurrentNode = StartingNode;

        EvaluateNextDirection();
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, DestinationNode.gameObject.transform.position, Speed * Time.deltaTime);
        if (transform.position.x == DestinationNode.gameObject.transform.position.x && transform.position.y == DestinationNode.gameObject.transform.position.y && transform.position.z == DestinationNode.gameObject.transform.position.z)
        {
            CurrentNode = DestinationNode;
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
    }

    private void SetNewDestination(NodeController destination)
    {
        DestinationNode = destination;
        transform.LookAt(DestinationNode.gameObject.transform);
    }

    private NodeController FindNextNode(DirectionEnum direction)
    {
        NodeController destination = null;

        switch (direction)
        {
            case DirectionEnum.Top:
                if (CurrentNode.CanMoveTop == true)
                {
                    destination = CurrentNode.NodeTop;
                }
                break;

            case DirectionEnum.Right:
                if (CurrentNode.CanMoveRight == true)
                {
                    destination = CurrentNode.NodeRight;
                }
                break;

            case DirectionEnum.Bottom:
                if (CurrentNode.CanMoveBottom == true)
                {
                    destination = CurrentNode.NodeBottom;
                }
                break;

            case DirectionEnum.Left:
                if (CurrentNode.CanMoveLeft == true)
                {
                    destination = CurrentNode.NodeLeft;
                }
                break;
        }

        return destination;
    }
}
