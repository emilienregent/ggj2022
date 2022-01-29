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
    public Text Goal;
    public GridLayoutGroup Lifes;

    private List<GameObject> _lifeInstances = new List<GameObject>();

    public List<GameObject> LifeInstances { get => _lifeInstances; set => _lifeInstances = value; }

    private void Start()
    {
        for(int i = 0; i < GameManager.Instance.LifesLeft; i++)
        {
            // We instantiate the Life GameObject
            GameObject life = Instantiate(LifePrefab, Lifes.transform.position, Quaternion.identity, Lifes.gameObject.transform) as GameObject;
            LifeInstances.Add(life);
        }
        GameManager.Instance.OnScoreChangeHandler += UpdateScoreDisplay;
        GameManager.Instance.OnLifeChangeHandler += UpdateLifeDisplay;
        GameManager.Instance.OnChangeStateHandler += ChangeHUDState;
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
                Goal.text = "Don't die !";
                Goal.color = new Color32(254, 227, 15, 255);
                break;

            case GameState.GHOST:
                Goal.text = "Eat Pac-Man !";
                Goal.color = new Color32(254, 0, 0, 255);
                break;
        }
        UpdateLifeDisplay();
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
