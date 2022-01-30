using UnityEngine;

public class BlinkyMovementController : PhantomMovementController
{
    private const float SC_PELLET_INCREASE = 0.05f;

    public NodeController AlternativeStartingNode;

    private int _pelletCount = 0;
    private float MaximumSpeed { get { return Speed * SC_PACMAN_NORMAL + Speed * SC_PELLET_INCREASE; } }

    protected override void SetStartingNode()
    {
        if (GameManager.Instance.CurrentState == GameState.GHOST)
        {
            Vector3 spawnPoint = AlternativeStartingNode.transform.position;

            spawnPoint.y = transform.position.y;

            transform.position = spawnPoint;

            CurrentNode = AlternativeStartingNode;

            CurrentSpeed = Mathf.Min(CurrentSpeed + Speed * SC_PELLET_INCREASE, MaximumSpeed);
        }
        else
        {
            base.SetStartingNode();
        }
    }

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

        if(GameManager.Instance.CurrentState == GameState.GHOST)
        {
            _defaultDirection = DirectionEnum.None;
            DestinationNode = AlternativeStartingNode;
        } else
        {
            _defaultDirection = DirectionEnum.Left;
        }

        base.ResetMovement();
    }

    protected override void SetNextDirectionInCorner()
    {
        if (GameManager.Instance.CurrentState == GameState.PACMAN)
        {
            base.SetNextDirectionInCorner();
        }
    }

    public override void ReverseDirection()
    {
        if (GameManager.Instance.CurrentState == GameState.PACMAN)
        {
            base.ReverseDirection();
        }
    }

    public override void SetRandomDirection()
    {
        if (GameManager.Instance.CurrentState == GameState.PACMAN)
        {
            base.SetRandomDirection();
        }
    }

}