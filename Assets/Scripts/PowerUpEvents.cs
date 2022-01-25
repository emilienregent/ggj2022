using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpEvents
{
    protected PowerUpEvents() { }

    private static PowerUpEvents _instance = null;

    // Singleton pattern implementation
    public static PowerUpEvents Instance
    {
        get
        {
            if (PowerUpEvents._instance == null)
            {
                PowerUpEvents._instance = new PowerUpEvents
                {

                };

            }
            return PowerUpEvents._instance;
        }
    }

    public event Action TimeIntervalElapsed;
    public void OnTimeIntervalElapsedAction()
    {
        TimeIntervalElapsed?.Invoke();
    }
    
    public event Action PowerUpPhaseStarting;
    public void OnPowerUpPhaseStartingAction()
    {
        PowerUpPhaseStarting?.Invoke();
    }

    public event Action PowerUpPhaseEnding;
    public void OnPowerUpPhaseEndingAction()
    {
        PowerUpPhaseEnding?.Invoke();
    }
}
