using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    private static uint NODE_ID = 1;

    public GameObject Center;

    private NodeController _nodeUp;
    private NodeController _nodeRight;
    private NodeController _nodeDown;
    private NodeController _nodeLeft;
    private List<DirectionEnum> _directions = new List<DirectionEnum>();

    public bool CanMoveUp { get { return HasDirection(DirectionEnum.Up); } }
    public bool CanMoveRight { get { return HasDirection(DirectionEnum.Right); } }
    public bool CanMoveDown { get { return HasDirection(DirectionEnum.Down); } }
    public bool CanMoveLeft { get { return HasDirection(DirectionEnum.Left); } }
    public NodeController NodeUp { get => _nodeUp; set => _nodeUp = value; }
    public NodeController NodeRight { get => _nodeRight; set => _nodeRight = value; }
    public NodeController NodeDown { get => _nodeDown; set => _nodeDown = value; }
    public NodeController NodeLeft { get => _nodeLeft; set => _nodeLeft = value; }

    public PickupController PickupItem { get; private set; }

    public List<DirectionEnum> Directions { get => _directions; private set => _directions = value; }

    /// <summary>
    /// A corner is a node with 2 direction but direction are not on the same axis
    /// </summary>
    public bool IsCorner { get { return _directionCount == 2 && _directionMask != (int)DirectionEnum.Horizontal && _directionMask != (int)DirectionEnum.Vertical; } }
    /// <summary>
    /// An intersection is a node with more than 2 directions
    /// </summary>
    public bool IsIntersection { get { return _directionCount > 2; } }
    /// <summary>
    /// A dead end is a node with only one direction
    /// </summary>
    public bool IsDeadEnd { get { return _directionCount == 1; } }

    [ReadOnly]
    [SerializeField]
    private int _directionMask = 0;
    private int _directionCount = 0;

    private MovementController _controllerPresent;

    private const string TAG_NODE = "Node";

    private void Awake()
    {
        RaycastHit hit;
        // Up Node
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == TAG_NODE)
            {
                NodeUp = hit.transform.gameObject.GetComponent<NodeController>();
                AddDirection(DirectionEnum.Up);
            }
        }
        // Right Node
        if (Physics.Raycast(transform.position, -1f * transform.TransformDirection(Vector3.left), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == TAG_NODE)
            {
                NodeRight = hit.transform.gameObject.GetComponent<NodeController>();
                AddDirection(DirectionEnum.Right);
            }
        }
        // Down Node
        if (Physics.Raycast(transform.position, -1f * transform.TransformDirection(Vector3.forward), out hit, 1f))
        {
            if (hit.transform.gameObject.tag == TAG_NODE)
            {
                NodeDown = hit.transform.gameObject.GetComponent<NodeController>();
                AddDirection(DirectionEnum.Down);
            }
        }
        // Left Node
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1f))
        {
            if(hit.transform.gameObject.tag == TAG_NODE)
            {
                NodeLeft = hit.transform.gameObject.GetComponent<NodeController>();
                AddDirection(DirectionEnum.Left);
            }
        }

        _directionCount = Directions.Count;

        SetName();
        TryGetItem();
    }

    private void SetName()
    {
        if (gameObject.name == TAG_NODE)
        {
            gameObject.name = string.Format("{0} #{1}", TAG_NODE, NODE_ID++);
        }
    }

    private void TryGetItem()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit hit, 1f))
        {
            if (hit.transform.gameObject.tag == GameManager.PELLET_TAG)
            {
                PickupItem = hit.transform.gameObject.GetComponent<PickupController>();
            }
        }
    }

    private void AddDirection(DirectionEnum direction)
    {
        Directions.Add(direction);
        _directionMask |= (int)direction;
    }

    public bool HasDirection(DirectionEnum direction)
    {
        return (_directionMask & (int)direction) != 0;
    }

    public bool HasPhantom()
    {
        return _controllerPresent != null && typeof(PhantomMovementController).IsAssignableFrom(_controllerPresent.GetType());
    }

    public void EnterNode(MovementController controller)
    {
        _controllerPresent = controller;
    }

    public void LeaveNode()
    {
        _controllerPresent = null;
    }

    /// <summary>
    /// This find the direction to get out of a corner
    /// </summary>
    /// <param name="directionIn">The direction we come from</param>
    /// <returns>The direction to leave the corner</returns>
    public DirectionEnum GetDirectionOut(DirectionEnum directionIn)
    {
        // If entering with horizontal direction we want to get out with vertical direction and vice versa
        DirectionEnum axis = DirectionEnum.Horizontal.HasFlag(directionIn) ? DirectionEnum.Vertical : DirectionEnum.Horizontal;

        foreach (var direction in Directions)
        {
            // Stop if we find a direction on matching axis
            if (axis.HasFlag(direction))
            {
                return direction;
            }
        }

        return DirectionEnum.None;
    }

    public DirectionEnum GetRandomDirection()
    {
        int randomIndex = Random.Range(0, Directions.Count);

        return Directions[randomIndex];
    }

    public bool TryGetNextNode(DirectionEnum direction, out NodeController nextNode)
    {
        nextNode = null;

        if (direction == DirectionEnum.None)
        {
            return false;
        }

        switch (direction)
        {
            case DirectionEnum.Up:
            if (CanMoveUp == true)
            {
                nextNode = NodeUp;
                return true;
            }
            break;

            case DirectionEnum.Right:
            if (CanMoveRight == true)
            {
                nextNode = NodeRight;
                return true;
            }
            break;

            case DirectionEnum.Down:
            if (CanMoveDown == true)
            {
                nextNode = NodeDown;
                return true;
            }
            break;

            case DirectionEnum.Left:
            if (CanMoveLeft == true)
            {
                nextNode = NodeLeft;
                return true;
            }
            break;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (HasPhantom())
        {
            Gizmos.DrawSphere(Center.transform.position, 1f);
        }
    }
}
