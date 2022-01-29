using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = System.Random;

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
    public static string PELLET_TAG = "Pellet";

    private int _score;
    private int _lifesLeft;
    private float _pelletLimitBeforePop = 0.9f;

    private GameObject[] _pellets;
    private List<GameObject> _availablePellets = new List<GameObject>();
    private List<PickupController> _collectedPellets = new List<PickupController>();
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
        ChangeState(GameState.PACMAN);


        //ChangeState(GameState.PACMAN); // /!\ TODO : STATE MENU WHEN AVAILABLE /!\
        _pellets = GameObject.FindGameObjectsWithTag(PELLET_TAG);
        _availablePellets = new List<GameObject>(_pellets);
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

    public void IncreaseScore(int points, PickupController pickupObject) {

        if(CurrentState == GameState.PACMAN)
        {
            Score += points;

            if(points == (int)PickupType.Pellet)
            {
                _collectedPellets.Add(pickupObject);
                _availablePellets.Remove(pickupObject.gameObject);

                if ((totalCountPellets * _pelletLimitBeforePop) < _collectedPellets.Count)
                {
                    _availablePellets.Add(_collectedPellets[0].gameObject);
                    Random rnd = new Random();
                    int index = rnd.Next(0, _collectedPellets.Count - 30);

                    _collectedPellets[index].EnableGameObject();
                    _collectedPellets.RemoveAt(index);
                }
            }
        }
        else if(CurrentState == GameState.GHOST)
        {
            Score -= points;
            if(Score < 0)
            {
                Score = 0;
            }
        }

        OnScoreChangeAction();
        PelletCollected?.Invoke();

        if(_collectedPellets.Count == totalCountPellets)
        {
            // /!\ TODO : WIN SCREEN /!\
            ChangeState(GameState.GAMEOVER);
        }
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
