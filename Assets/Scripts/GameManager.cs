using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = System.Random;

public enum GameState {
    MENU,
    PACMAN,
    GHOST,
    GAMEOVER,
    VICTORY
}

public class GameManager
{
    public static string PACMAN_TAG = "Pacman";
    public static string GHOST_TAG = "Ghost";
    public static string PELLET_TAG = "Pellet";

    private int _score;
    private int _lifesLeft;
    private float _pelletLimitBeforePop = 0.9f;

    private GameObject[] _pellets;
    private List<GameObject> _availablePellets = new List<GameObject>();
    private List<PickupController> _collectedPellets = new List<PickupController>();
    private int _collectedPelletsCounter = 0;
    public int totalCountPellets { get; private set; }

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
    public bool IsPaused = false;

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
                _instance._pellets = GameObject.FindGameObjectsWithTag(PELLET_TAG);
                _instance.totalCountPellets = _instance._pellets.Length;
                _instance._availablePellets = new List<GameObject>(_instance._pellets);
                _instance._collectedPellets = new List<PickupController>();
            }
            return _instance;
        }
    }

    #endregion

    public void RestartGame()
    {
        _score = 0;
        _lifesLeft = Lifes;
        _collectedPellets = new List<PickupController>();
        _collectedPelletsCounter = 0;
        ChangeState(GameState.PACMAN);

        //ChangeState(GameState.PACMAN); // /!\ TODO : STATE MENU WHEN AVAILABLE /!\
        _pellets = GameObject.FindGameObjectsWithTag(PELLET_TAG);
        _availablePellets = new List<GameObject>(_pellets);
    }

    public void PauseGame()
    {
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }

    public GameObject GetRandomPellet()
    {
        if(_pellets.Length == 0)
        {
            _pellets = GameObject.FindGameObjectsWithTag(PELLET_TAG);
            _availablePellets = new List<GameObject>(_pellets);
        }
        return _availablePellets[UnityEngine.Random.Range(0, _availablePellets.Count)];
    }

    public void IncreaseScore(int points) {

        Score += points;

        if(points == (int)PickupType.Pellet)
        {
            // Repop of pellets only if Pac-Man is the one eating them
            if (CurrentState == GameState.PACMAN && (totalCountPellets * _pelletLimitBeforePop) < _collectedPellets.Count)
            {
                _availablePellets.Add(_collectedPellets[0].gameObject);
                Random rnd = new Random();
                int index = rnd.Next(0, _collectedPellets.Count - 30);

                _collectedPellets[index].EnableGameObject();
                _collectedPellets.RemoveAt(index);
                _collectedPelletsCounter--;
            }
        }

        OnScoreChangeAction();
        PelletCollected?.Invoke();

        if(CurrentState == GameState.PACMAN && _collectedPelletsCounter == totalCountPellets)
        {
            ChangeState(GameState.GAMEOVER);
            SceneManager.LoadScene("GameOver");
            return;
        }
    }

    public void DecreaseScore(int points)
    {
        PelletCollected?.Invoke();
        Score = Mathf.Max(0, Score - points);
        OnScoreChangeAction();
    }

    public void CollectPellet(PickupController pickupObject)
    {
        _collectedPelletsCounter++;
        _collectedPellets.Add(pickupObject);
        _availablePellets.Remove(pickupObject.gameObject);

        CheckVictoryCondition();
    }

    public void CheckVictoryCondition()
    {
        Debug.Log("Collected Pellets = " + _collectedPelletsCounter + " / " + totalCountPellets);
        if (CurrentState == GameState.GHOST && _collectedPelletsCounter == (totalCountPellets))
        {
            ChangeState(GameState.VICTORY);
            SceneManager.LoadScene("Victory");
            return;
        }
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

        PacmanDying?.Invoke();
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
        CurrentState = newState;
        OnChangeStateHandler?.Invoke();
    }
}
