using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    private const float TIME_INTERVAL = 1f;

    public static bool IsEnabled = false;

    [ReadOnly, SerializeField] private float _lastInterval;
    [ReadOnly, SerializeField] private float _elapsedTime;

    private float _powerUpStartTime = float.MaxValue;

    private void Awake()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    // Start is called before the first frame update
    void Start()
    {
        _lastInterval = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateTimerTick();
    }

    private void RegisterEvents()
    {
        PowerUpEvents.Instance.TimeIntervalElapsed += UpdatePowerUp;
    }

    private void UnregisterEvents()
    {
        PowerUpEvents.Instance.TimeIntervalElapsed -= UpdatePowerUp;
    }

    private void UpdatePowerUp()
    {
        if (Time.time - _powerUpStartTime >= GameManager.Instance.PowerUpDuration)
        {
            DisablePowerUp();
        }
    }

    private void UpdateTimerTick()
    {
        if (Time.time - _lastInterval >= TIME_INTERVAL)
        {
            PowerUpEvents.Instance.OnTimeIntervalElapsedAction();

            _lastInterval = Time.time;
        }
    }

    public void EnablePowerUp()
    {
        IsEnabled = true;

        _powerUpStartTime = Time.time;

        PowerUpEvents.Instance.OnPowerUpPhaseStartingAction();
    }

    public void DisablePowerUp()
    {
        IsEnabled = false;

        _powerUpStartTime = float.MaxValue;

        PowerUpEvents.Instance.OnPowerUpPhaseEndingAction();
    }
}
