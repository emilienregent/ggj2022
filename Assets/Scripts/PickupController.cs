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

    private SphereCollider _collider;

    private void Start() {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(GameManager.PACMAN_TAG))
        {
            GameManager.Instance.IncreaseScore((int)_type);

            if (_type == PickupType.Energizer)
            {
                PlayerController controller = other.gameObject.GetComponent<PlayerController>();
                controller.PlayerPowerUpBehavior.EnablePowerUp();
            }

            DisableGameObject();
        }
    }

    private void DisableGameObject() {
        this.gameObject.SetActive(false);
        _collider.enabled = false;
    }

    public void EnableGameObject() {
        this.gameObject.SetActive(true);
        _collider.enabled = true;
    }
}