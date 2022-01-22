using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    [SerializeField]
    private Vector3 _entryDirection = Vector3.up;
    [SerializeField]
    private bool _localDirection = false;
    [SerializeField, Range(1f, 2f)] private float _triggerScale = 1.25f;

    private BoxCollider _collider = null;
    private BoxCollider _collisionCheckTriger = null;

    private void Awake() {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = false;
        _collisionCheckTriger = gameObject.AddComponent<BoxCollider>();
        _collisionCheckTriger.size = _collider.size * _triggerScale;
        _collisionCheckTriger.center = _collider.center;
        _collisionCheckTriger.isTrigger = true;
    }

    private void OnTriggerStay(Collider other) {
        if(Physics.ComputePenetration(
                _collisionCheckTriger, transform.position, transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out Vector3 collisionDirection, out float penetrationDepth
            )
         )
        {
            float dot = Vector3.Dot(_entryDirection, collisionDirection);
            if(dot < 0)
            {
                Physics.IgnoreCollision(_collider, other, false);
            } else
            {
                Physics.IgnoreCollision(_collider, other, true);
            }
        }
    }


    private void OnDrawGizmosSelected() {
        Vector3 direction;
        if(_localDirection)
        {
            direction = transform.TransformDirection(_entryDirection.normalized);
        } else
        {
            direction = _entryDirection;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -direction);
    }
}
