using Sirenix.OdinInspector;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    [Button]
    public void CheatKillPlayer()
    {
        GameManager.Instance.EatPacman();
    }
}
