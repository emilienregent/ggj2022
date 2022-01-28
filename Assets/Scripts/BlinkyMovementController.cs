using UnityEngine;

public class BlinkyMovementController : PhantomMovementController
{
    private const float SC_PELLET_INCREASE = 0.05f;

    private int _pelletCount = 0;
    private float MaximumSpeed { get { return Speed * SC_PACMAN_NORMAL + Speed * SC_PELLET_INCREASE; } }

    protected override void SetNormalSpeed()
    {
        SetCurrentSpeed(SC_PHANTOM_NORMAL);
    }

    protected override void SetFrightenedSpeed()
    {
        SetCurrentSpeed(SC_PHANTOM_FRIGHTENED);
    }

    protected override void SetPelletSpeed()
    {
        _pelletCount++;

        CurrentSpeed = Mathf.Min(CurrentSpeed + Speed * SC_PELLET_INCREASE, MaximumSpeed);
    }

    protected override void SetCurrentSpeed(float speedCoefficient)
    {
        CurrentSpeed = Speed * speedCoefficient;

        // Reapply speed increase from pellets
        CurrentSpeed = Mathf.Min(CurrentSpeed + Speed * SC_PELLET_INCREASE * _pelletCount, MaximumSpeed);
    }

    public override void ResetMovement()
    {
        _pelletCount = 0;

        base.ResetMovement();
    }
}