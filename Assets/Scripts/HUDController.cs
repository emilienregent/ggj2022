using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    // UI References
    public Text Score;

    private void Awake()
    {
        GameManager.Instance.OnScoreChangeHandler += UpdateScoreDisplay;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreChangeHandler -= UpdateScoreDisplay;
    }

    private void UpdateScoreDisplay()
    {
        Score.text = "Score : " + GameManager.Instance.Score.ToString("d5");
    }
}
