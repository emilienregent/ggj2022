using UnityEngine;

public enum PickupType
{
    Pellet = 10,
    Energizer = 50
}

public class PickupController : MonoBehaviour
{
    [SerializeField]
    private PickupType _type = PickupType.Pellet;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pacman"))
        {
            GameManager.Instance.IncreaseScore((int)_type);

            if (_type == PickupType.Energizer)
            {
                GameManager.Instance.EnablePowerUp();
            }

            Destroy(this.gameObject);
        }
    }
}