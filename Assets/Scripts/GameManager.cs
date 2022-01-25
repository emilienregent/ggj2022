using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string PACMAN_TAG = "Pacman";

    private const float TIME_INTERVAL = 1f;

    private int _score;
    private int _lifesLeft;

    [ReadOnly, SerializeField] private float _lastInterval;
    [ReadOnly, SerializeField] private float _elapsedTime;


    public int Score { get => _score; set => _score = value; }
    public int LifesLeft { get => _lifesLeft; set => _lifesLeft = value; }

    public event Action TimeIntervalElapsed;

    public event Action PowerUpPhaseStarting;
    public event Action PowerUpPhaseEnding;

    public event Action PacmanDying;
    public event Action PhantomDying;

    // Configuration
    public float PowerUpDuration = 3f;
    public int Lifes = 3;

    private float _powerUpStartTime = float.MaxValue;

    #region SINGLETON
    // Static singleton instance
    public static GameManager Instance { private set; get; }

    // Static singleton property
    private void Awake()
    {
        // First destroy any existing instance of it
        if (Instance != null)
        {
            Destroy(Instance);
        }

        // Then reassign a proper one
        Instance = this;

        DontDestroyOnLoad(this);

        _lastInterval = Time.time;
        _lifesLeft = Lifes;

        RegisterEvents();
    }
    #endregion

    private void RegisterEvents()
    {
        TimeIntervalElapsed += UpdatePowerUp;
    }

    private void UnregisterEvents()
    {
        TimeIntervalElapsed -= UpdatePowerUp;
    }

    private void Update()
    {
        UpdateTimerTick();
    }

    private void UpdatePowerUp()
    {
        if (Time.time - _powerUpStartTime >= PowerUpDuration)
        {
            DisablePowerUp();
        }
    }

    private void UpdateTimerTick()
    {
        if (Time.time - _lastInterval >= TIME_INTERVAL)
        {
            TimeIntervalElapsed?.Invoke();

            _lastInterval = Time.time;
        }
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

    public void EnablePowerUp()
    {
        _powerUpStartTime = Time.time;

        PowerUpPhaseStarting?.Invoke();
    }

    public void DisablePowerUp()
    {
        _powerUpStartTime = float.MaxValue;

        PowerUpPhaseEnding?.Invoke();
    }

    public void EatPacman()
    {
        RemoveLife();
        PacmanDying?.Invoke();
    }

    public void EatPhantom()
    {
        PhantomDying?.Invoke();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }
}
