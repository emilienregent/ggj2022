using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pacman"))
        {
            Destroy(this.gameObject);
        }
    }

}
