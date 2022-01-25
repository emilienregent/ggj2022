using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public Vector3 targetPosition;
    public NodeController target;
    public NodeController nextNode;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pacman") || other.CompareTag("Ghost"))
        {
            MovementController movementController = other.GetComponent<MovementController>();

            movementController.CurrentNode = target;
            movementController.DestinationNode = nextNode;

            other.gameObject.transform.position = target.gameObject.transform.position;
        }
    }

}
