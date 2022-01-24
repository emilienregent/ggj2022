using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float TIME_INTERVAL = 1f;

    private int _score;

    [ReadOnly, SerializeField] private float _time;
    [ReadOnly, SerializeField] private float _elapsedTime;

    public event Action timeIntervalElapsed;

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

        _time = Time.time;
    }
    #endregion

    private void Update()
    {
        _elapsedTime = Time.time - _time;

        if (_elapsedTime >= TIME_INTERVAL)
        {
            timeIntervalElapsed?.Invoke();
        }
    }

    public void IncreaseScore(int points) {
        _score += points;
    }
}
