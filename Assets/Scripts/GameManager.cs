using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static string PACMAN_TAG = "Pacman";

    private int _score;
    private int _lifesLeft;

    public int Score { get => _score; set => _score = value; }
    public int LifesLeft { get => _lifesLeft; set => _lifesLeft = value; }

    public event Action PacmanDying;
    public event Action PhantomDying;

    // Configuration
    public float PowerUpDuration = 3f;
    public int Lifes = 3;

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

            }
            return _instance;
        }
    }

    #endregion

    public void RestartGame()
    {
        _score = 0;
        _lifesLeft = Lifes;
    }

    public void IncreaseScore(int points) {
        Score += points;
        OnScoreChangeAction();
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
        RemoveLife();
        PacmanDying?.Invoke();

        if(LifesLeft <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void EatPhantom()
    {
        PhantomDying?.Invoke();
    }
}
