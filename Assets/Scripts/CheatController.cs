using UnityEngine;

public class CheatController : MonoBehaviour
{
    public void CheatKillPlayer()
    {
        GameManager.Instance.EatPacman();
    }
}
