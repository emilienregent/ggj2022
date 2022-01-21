//using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class PhantomNavigationBehaviour : MonoBehaviour
{
    public Transform target;
    public GameObject waypointTrigger;

    private NavMeshAgent _agent;
    private NavMeshPath _path;
    private Vector3[] _waypoints;
    private int _waypointIndex;
    private float _speed;

    private const float TILE_OFFSET = 0.5f;

    private void OnValidate()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        _path = new NavMeshPath();
        _speed = _agent.speed;
    }

    //[Button]
    private void CalculatePath()
    {
        SetNewPath();

        // Start with first corner set along with the new path
        Vector3 previousCorner = _waypoints[0];
        Vector3 nextCorner;

        for (int i = 1, j = 1, count = _path.corners.Length; i < count; i++)
        {
            nextCorner = _path.corners[i];

            // Skip even waypoints as not useful (pretty much duplicates of odds)
            if (i % 2 == 0)
            {
                continue;
            }

            // Last waypoint is the destination itself
            if (i == count - 1)
            {
                _waypoints[j] = nextCorner;
                continue;
            }

            previousCorner = _waypoints[j] = GetNextWaypoint(previousCorner, nextCorner);
            j++;
        }

        Debug.Log($"{_path.corners.Length} corners translated into {_waypoints.Length} waypoints");
    }

    private void SetNewPath()
    {
        // Calculate new path to destination
        _agent.CalculatePath(target.position, _path);

        // Create a new collection of half the corners (we skip even as pretty much duplicates)
        _waypoints = new Vector3[Mathf.RoundToInt(_path.corners.Length * 0.5f + 1)];

        // Set first corner as the current agent's position
        _waypoints[0] = _agent.transform.position;
    }

    /// <summary>
    /// Use previous waypoint set and the next corner in the path to define next waypoint mapped to the grid
    /// </summary>
    /// <param name="previousWaypoint">The previous waypoint set</param>
    /// <param name="nextCorner">The next corner in the path</param>
    /// <returns>Next waypoint snapped to the grid</returns>
    private Vector3 GetNextWaypoint(Vector3 previousWaypoint, Vector3 nextCorner)
    {
        Vector3 waypoint = Vector3.zero;
        float horizontalDistance = nextCorner.x - previousWaypoint.x;
        float verticalDistance = nextCorner.z - previousWaypoint.z;

        if (Mathf.Abs(horizontalDistance) > Mathf.Abs(verticalDistance))
        {
            waypoint.x = Mathf.RoundToInt(nextCorner.x) + GetOffset(horizontalDistance);
            waypoint.z = previousWaypoint.z;
        }
        else
        {
            waypoint.x = previousWaypoint.x;
            waypoint.z = Mathf.RoundToInt(nextCorner.z) + GetOffset(verticalDistance);
        }

        return waypoint;
    }

    /// <summary>
    /// Need an offset to snap the position to the grid but depends on the direction of the segment of the path
    /// </summary>
    /// <param name="distance">The distance between two points to know the direction sign</param>
    /// <returns>The offset to apply</returns>
    private float GetOffset(float distance)
    {
        // If distance between origin and destination is positive
        // we move in a positive direction, so adding positive offset
        if (distance > 0)
        {
            return TILE_OFFSET;
        }

        // otherwise we move in a negative direction
        return -TILE_OFFSET;
    }

    //[Button]
    private void GoToDestination()
    {
        SetNextDestination();
    }

    private void SetNextDestination()
    {
        if (_waypointIndex < _waypoints.Length)
        {
            Vector3 destination = _waypoints[_waypointIndex];

            PrepareWaypoint(destination);
            PrepareAgent(destination);
            
            _waypointIndex++;
        }
        else
        {
            _waypointIndex = 0;

            Debug.Log("Destination reached");
        }
    }

    private void PrepareWaypoint(Vector3 destination)
    {
        waypointTrigger.transform.position = destination + Vector3.up;
        waypointTrigger.SetActive(true);
    }

    private void PrepareAgent(Vector3 destination)
    {
        _agent.transform.LookAt(waypointTrigger.transform);
        _agent.SetDestination(destination);
        _agent.speed = _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(waypointTrigger))
        {
            waypointTrigger.SetActive(false);

            _agent.speed = 0f;

            SetNextDestination();
        }
    }

    private void OnDrawGizmos()
    {
        if (_waypoints == null)
            return;

        foreach(Vector3 corner in _waypoints)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(corner, 0.5f);
        }
    }
}
