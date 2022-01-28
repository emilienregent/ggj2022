using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState {
    MENU,
    PACMAN,
    GHOST,
    GAMEOVER
}

public class GameManager
{
    public static string PACMAN_TAG = "Pacman";
    public static string GHOST_TAG = "Ghost";

    private int _score;
    private int _lifesLeft;
    private float _pelletLimitBeforePop = 0.9f;

    private List<PickupController> _pellets = new List<PickupController>();
    public int _totalPellets { get; private set; }

    public int Score { get => _score; set => _score = value; }
    public int LifesLeft { get => _lifesLeft; set => _lifesLeft = value; }
    public int PhantomInHouse { get; private set; }

    public event Action PacmanDying;
    public event Action PhantomDying;
    public event Action PhantomLeaving;
    public event Action PelletCollected;
    public event Action OnChangeStateHandler;

    // Configuration
    public float PowerUpDuration = 3f;
    public int Lifes = 3;

    public GameState CurrentState { get; private set; }

    #region SINGLETON
    // Static singleton instance
    private static GameManager _instance = null;

    protected GameManager()
    {
        RestartGame();
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
                _instance.ChangeState(GameState.PACMAN); // /!\ TODO : STATE MENU WHEN AVAILABLE /!\
                _instance._totalPellets = GameObject.FindGameObjectsWithTag("Pellet").Length;
            }
            return _instance;
        }
    }

    #endregion

    public void RestartGame()
    {
        _score = 0;
        _lifesLeft = Lifes;
        _pellets = new List<PickupController>();
        ChangeState(GameState.PACMAN);
    }

    public void IncreaseScore(int points, PickupController pickupObject) {

        if(CurrentState == GameState.PACMAN)
        {

            Score += points;

            if(points == (int)PickupType.Pellet)
            {
                _pellets.Add(pickupObject);

                if((_totalPellets * _pelletLimitBeforePop) < _pellets.Count)
                {
                    _pellets[0].EnableGameObject();
                    _pellets.RemoveAt(0);
                }
            }
        } else if(CurrentState == GameState.GHOST)
        {
            Score -= points;
            if(Score < 0)
            {
                Score = 0;
            }
        }

        OnScoreChangeAction();
        PelletCollected?.Invoke();
    }

    public void DecreaseScore(int points)
    {
        Score = Mathf.Max(0, Score - points);
        OnScoreChangeAction();
    }

    public event Action OnScoreChangeHandler;
    public void OnScoreChangeAction()
    {
        OnScoreChangeHandler?.Invoke();
    }

    public void AddLife()
    {
        LifesLeft++;
        OnLifeChangeAction();
    }

    public void RemoveLife()
    {
        LifesLeft--;
        OnLifeChangeAction();
    }

    public event Action OnLifeChangeHandler;
    public void OnLifeChangeAction()
    {
        OnLifeChangeHandler?.Invoke();
    }

    public void EatPacman()
    {
        PhantomInHouse = 0; // Reset count of phantoms (while be set again by phantoms)

        if(CurrentState == GameState.PACMAN)
        {
            RemoveLife();
        }

        PacmanDying?.Invoke();

        if (LifesLeft <= 0)
        {
            ChangeState(GameState.GAMEOVER);
            SceneManager.LoadScene("GameOver");
            return;
        }

        switch(CurrentState)
        {
            case GameState.PACMAN:
                ChangeState(GameState.GHOST);
                break;

            case GameState.GHOST:
                ChangeState(GameState.PACMAN);
                break;
        }
    }

    public void EatPhantom()
    {
        PhantomDying?.Invoke();
    }

    public void EnterPhantomHouse()
    {
        PhantomInHouse++;
    }

    public void LeavePhantomHouse()
    {
        if (PhantomInHouse > 0)
        {
            PhantomInHouse--;

            PhantomLeaving?.Invoke();
        }
    }

    public void ChangeState(GameState newState) {
        Debug.Log("SWITCH TO STATE " + newState.ToString());
        CurrentState = newState;
        OnChangeStateHandler?.Invoke();
    }
}
