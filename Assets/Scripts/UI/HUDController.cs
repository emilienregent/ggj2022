using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // Prefab References
    public GameObject LifePrefab;

    // UI References
    public Text Score;
    public GoalBannerController GoalControllerPacMan;
    public GoalBannerController GoalControllerBlinky;
    public GridLayoutGroup Lifes;

    private List<GameObject> _lifeInstances = new List<GameObject>();

    public List<GameObject> LifeInstances { get => _lifeInstances; set => _lifeInstances = value; }

    private void Awake()
    {
        GameManager.Instance.OnScoreChangeHandler += UpdateScoreDisplay;
        GameManager.Instance.OnLifeChangeHandler += UpdateLifeDisplay;
        GameManager.Instance.OnChangeStateHandler += ChangeHUDState;
    }

    private void Start()
    {
        for(int i = 0; i < GameManager.Instance.LifesLeft; i++)
        {
            // We instantiate the Life GameObject
            GameObject life = Instantiate(LifePrefab, Lifes.transform.position, Quaternion.identity, Lifes.gameObject.transform) as GameObject;
            LifeInstances.Add(life);
        }

        ChangeHUDState();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreChangeHandler -= UpdateScoreDisplay;
        GameManager.Instance.OnLifeChangeHandler -= UpdateLifeDisplay;
        GameManager.Instance.OnChangeStateHandler -= ChangeHUDState;
    }

    private void UpdateScoreDisplay()
    {
        Score.text = "Score : " + GameManager.Instance.Score.ToString("d5");
    }

    private void ChangeHUDState()
    {
        switch(GameManager.Instance.CurrentState)
        {
            case GameState.PACMAN:
                GoalControllerBlinky.gameObject.SetActive(false);
                GoalControllerPacMan.gameObject.SetActive(true);
                StartCoroutine(AnimatePacmanStateStart());
                break;

            case GameState.GHOST:
                GoalControllerPacMan.gameObject.SetActive(false);
                GoalControllerBlinky.gameObject.SetActive(true);
                StartCoroutine(AnimateGhostStateStart());
                break;
        }
        UpdateLifeDisplay();
    }

    private IEnumerator AnimatePacmanStateStart()
    {
        GameManager.Instance.PauseGame();
        GoalControllerPacMan.Goal.color = new Color32(254, 227, 15, 255);
        GoalControllerPacMan.Goal.text = "Ready ?";
        yield return new WaitForSecondsRealtime(1);
        GoalControllerPacMan.Goal.text = "Don't die !";
        yield return new WaitForSecondsRealtime(2);
        GameManager.Instance.ResumeGame();
        GoalControllerPacMan.gameObject.SetActive(false);
    }

    private IEnumerator AnimateGhostStateStart()
    {
        GameManager.Instance.PauseGame();
        GoalControllerBlinky.Goal.color = new Color32(254, 0, 0, 255);
        GoalControllerBlinky.Goal.text = "Ready ?";
        yield return new WaitForSecondsRealtime(1);
        GoalControllerBlinky.Goal.text = "Eat Pac-Man !";
        yield return new WaitForSecondsRealtime(2);
        GameManager.Instance.ResumeGame();
        GoalControllerBlinky.gameObject.SetActive(false);

    }

    private void UpdateLifeDisplay()
    {

        Color32 newColor = new Color32(255, 255, 255, 255);
        if(GameManager.Instance.CurrentState == GameState.GHOST)
        {
            newColor = new Color32(255, 255, 255, 128);
        }

        for(int i = 0; i < LifeInstances.Count; i++)
        {
            if(i >= GameManager.Instance.LifesLeft)
            {
                LifeInstances[i].SetActive(false);
            } else
            {
                LifeInstances[i].SetActive(true);
                LifeInstances[i].GetComponent<Image>().color = newColor;
            }
        }
    }
}
