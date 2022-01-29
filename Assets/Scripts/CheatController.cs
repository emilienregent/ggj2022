using Sirenix.OdinInspector;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    [Button]
    public void CheatKillPacMan()
    {
        GameManager.Instance.EatPacman();
    }
}
