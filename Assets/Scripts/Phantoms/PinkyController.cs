using UnityEngine;

/// <summary>
/// Pinky's targeting scheme attempts to move her to the place where Pac-Man is going, instead of where he currently is.
/// Her target tile in Chase mode is determined by looking at Pac-Man’s current position and orientation, and selecting the location four tiles straight ahead of Pac-Man.
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class PinkyController : PhantomController
{
    private const float DISTANCE_OFFSET = 4f;

    protected override Vector3 GetDestination()
    {
        return target.transform.position + target.transform.forward * DISTANCE_OFFSET;
    }
}