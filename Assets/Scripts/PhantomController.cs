using UnityEngine;

/// <summary>
/// Manage phantom general behaviour and orchestrate for decision
/// </summary>
public class PhantomController : MonoBehaviour
{
    public Transform target;

    PhantomMovementController _phantomMovementController;

    private void Awake()
    {
        _phantomMovementController = GetComponent<PhantomMovementController>();

        if (_phantomMovementController == null)
        {
            Debug.LogError("No PhantomMovementController found in the Phantom's components");
        }

        _phantomMovementController.IntersectionReached += SetNewDirection;
    }

    private void OnDestroy()
    {
        _phantomMovementController.IntersectionReached -= SetNewDirection;
    }

    private void SetNewDirection()
    {
        // Simple base behaviour to follow target
        _phantomMovementController.FollowTarget(target);
    }
}