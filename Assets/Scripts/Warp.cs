using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public Vector3 targetPosition;


    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pacman") || other.CompareTag("Ghost"))
        {
            other.gameObject.transform.position = targetPosition;
        }
    }

}
