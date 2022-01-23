using UnityEngine;

public enum DirectionEnum
{
    None = 0,

    Up = 1 << 0, // 1
    Right = 1 << 1, // 2
    Down = 1 << 2, // 4
    Left = 1 << 3, // 8

    Horizontal = Left | Right, // 10
    Vertical = Up | Down // 5
};

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
        if(DestinationNode != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, DestinationNode.gameObject.transform.position, Speed * Time.deltaTime);
            if (transform.position.x == DestinationNode.gameObject.transform.position.x && transform.position.y == DestinationNode.gameObject.transform.position.y && transform.position.z == DestinationNode.gameObject.transform.position.z)
            {
                CurrentNode = DestinationNode;
                EvaluateNextDirection();
            }
        }
    }

    public virtual void EvaluateNextDirection()
    {
        NodeController destination;

        // If we can go to the wanted direction, we go with it
        if (TryGetNextNode(NextDirection, out destination))
        {
            // We update the current direction since we can go in the direction we want
            CurrentDirection = NextDirection;

            SetNewDestination(destination);
        }
        // There is no node available in the direction we wanted, so we keep going in the current direction
        else if (TryGetNextNode(CurrentDirection, out destination))
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

    public bool TryGetNextNode(DirectionEnum direction, out NodeController destination)
    {
        destination = null;

        if (direction == DirectionEnum.None)
        {
            return false;
        }

        switch (direction)
        {
            case DirectionEnum.Up:
                if (CurrentNode.CanMoveUp == true)
                {
                    destination = CurrentNode.NodeUp;
                    return true;
                }
                break;

            case DirectionEnum.Right:
                if (CurrentNode.CanMoveRight == true)
                {
                    destination = CurrentNode.NodeRight;
                    return true;
                }
                break;

            case DirectionEnum.Down:
                if (CurrentNode.CanMoveDown == true)
                {
                    destination = CurrentNode.NodeDown;
                    return true;
                }
                break;

            case DirectionEnum.Left:
                if (CurrentNode.CanMoveLeft == true)
                {
                    destination = CurrentNode.NodeLeft;
                    return true;
                }
                break;
        }

        return false;
    }
}
