using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float TIME_INTERVAL = 1f;

    private int _score;

    [ReadOnly, SerializeField] private float _lastInterval;
    [ReadOnly, SerializeField] private float _elapsedTime;


    public int Score { get => _score; set => _score = value; }

    public event Action TimeIntervalElapsed;
    public event Action powerUpPhaseStarting;
    public event Action powerUpPhaseEnding;

    public float powerUpDuration = 3f;
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
        if (Time.time - _powerUpStartTime >= powerUpDuration)
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

    public void EnablePowerUp()
    {
        _powerUpStartTime = Time.time;

        powerUpPhaseStarting?.Invoke();
    }

    public void DisablePowerUp()
    {
        _powerUpStartTime = float.MaxValue;

        powerUpPhaseEnding?.Invoke();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }
}
