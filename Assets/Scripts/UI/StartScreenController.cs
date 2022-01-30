using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    public GameObject Wrapper;

    public Text GameTitlePart1;
    public Text GameTitlePart2;
    public Text GameTitlePart3;

    public Image BlinkyLogo;

    private void Awake()
    {
        GameManager.Instance.OnChangeStateHandler += ChangeState;
    }

    private void Start()
    {
        ChangeState();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnChangeStateHandler -= ChangeState;
    }

    private void ChangeState()
    {
        if(GameManager.Instance.CurrentState == GameState.START)
        {
            Wrapper.SetActive(true);
            GameManager.Instance.PauseGame();
        } else if(GameManager.Instance.PreviousState == GameState.START)
        {
            Wrapper.SetActive(false);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.ResumeGame();
        GameManager.Instance.ChangeState(GameState.PACMAN);
    }


}
