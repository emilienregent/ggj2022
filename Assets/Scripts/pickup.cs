using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField]
    private int _points = 10;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pacman"))
        {
            GameManager.Instance.IncreaseScore(_points);
            Destroy(this.gameObject);
        }
    }

}
