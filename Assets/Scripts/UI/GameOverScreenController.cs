using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreenController : MonoBehaviour
{

    // UI References
    public Text ScoreLabel;

    // Start is called before the first frame update
    void Start()
    {
        ScoreLabel.text = "Score : " + GameManager.Instance.Score.ToString("d5");
    }

    public void PlayAgain()
    {
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene("Main");
    }
}
