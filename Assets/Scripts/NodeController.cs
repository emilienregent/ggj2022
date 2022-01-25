using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public GameObject Center;

    private NodeController _nodeUp;
    private NodeController _nodeRight;
    private NodeController _nodeDown;
    private NodeController _nodeLeft;

    public bool CanMoveUp { get { return (_directionMask & (int)DirectionEnum.Up) != 0; } }
    public bool CanMoveRight { get { return (_directionMask & (int)DirectionEnum.Right) != 0; } }
    public bool CanMoveDown { get { return (_directionMask & (int)DirectionEnum.Down) != 0; } }
    public bool CanMoveLeft { get { return (_directionMask & (int)DirectionEnum.Left) != 0; } }
    public NodeController NodeUp { get => _nodeUp; set => _nodeUp = value; }
    public NodeController NodeRight { get => _nodeRight; set => _nodeRight = value; }
    public NodeController NodeDown { get => _nodeDown; set => _nodeDown = value; }
    public NodeController NodeLeft { get => _nodeLeft; set => _nodeLeft = value; }

    /// <summary>
    /// A corner is a node with 2 direction but direction are not on the same axis
    /// </summary>
    public bool IsCorner { get { return _directionCount == 2 && _directionMask != (int)DirectionEnum.Horizontal && _directionMask != (int)DirectionEnum.Vertical; } }
    /// <summary>
    /// An intersection is a node with more than 2 directions
    /// </summary>
    public bool IsIntersection { get { return _directionCount > 2; } }

    [ReadOnly]
    [SerializeField]
    private int _directionMask = 0;
    private int _directionCount = 0;
    private List<DirectionEnum> _directions = new List<DirectionEnum>();

    private const string TAG_NODE = "Node";

    // Start is called before the first frame update
    void Awake()
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

        _directionCount = _directions.Count;
    }

    private void AddDirection(DirectionEnum direction)
    {
        _directions.Add(direction);
        _directionMask |= (int)direction;
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

        foreach (var direction in _directions)
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
        int randomIndex = Random.Range(0, _directions.Count);

        return _directions[randomIndex];
    }
}
