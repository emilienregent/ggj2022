using UnityEngine;

/// <summary>
/// Manage phantom general behaviour and orchestrate for decision
/// </summary>
public class PhantomController : MonoBehaviour
{
    public Transform target;

    protected PhantomMovementController _phantomMovementController;

    private void Awake()
    {
        _phantomMovementController = GetComponent<PhantomMovementController>();

        if (_phantomMovementController == null)
        {
            Debug.LogError("No PhantomMovementController found in the Phantom's components");
        }

        _phantomMovementController.intersectionReached += SetNewDirection;
    }

    private void OnDestroy()
    {
        _phantomMovementController.intersectionReached -= SetNewDirection;
    }

    private void SetNewDirection()
    {
        Vector3 destination = GetDestination();

        _phantomMovementController.SetDirectionToDestination(destination);
    }

    /// <summary>
    /// Get a position for destination
    /// Default behaviour set destination as target position
    /// </summary>
    /// <returns>The position phantom will try to reach</returns>
    protected virtual Vector3 GetDestination()
    {
        return target.position;
    }
}