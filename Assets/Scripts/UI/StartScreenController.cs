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
            GameManager.Instance.PauseGame();
            Wrapper.SetActive(true);

            // Animate Blinky
            StartCoroutine(AnimateBlinky());

        } else if(GameManager.Instance.PreviousState == GameState.START)
        {
            StopAllCoroutines();
            Wrapper.SetActive(false);
        }
    }

    private IEnumerator AnimateBlinky()
    {
        bool isLookingRight = false;

        while(true)
        {
            if(isLookingRight == true)
            {
                RotateBlinkyLeft();
                isLookingRight = false;
            } else
            {
                RotateBlinkyRight();
                isLookingRight = true;
            }
            yield return new WaitForSecondsRealtime(1);
            
        }
    }

    private void RotateBlinkyLeft()
    {
        LeanTween.rotateY(BlinkyLogo.gameObject, 0f, 0f);
    }

    private void RotateBlinkyRight()
    {
        LeanTween.rotateY(BlinkyLogo.gameObject, 180f, 0f);
    }

    public void StartGame()
    {
        GameManager.Instance.ResetGameData();
        GameManager.Instance.ResumeGame();
        GameManager.Instance.ChangeState(GameState.PACMAN);
    }


}
