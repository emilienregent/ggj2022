using UnityEngine;

/// <summary>
/// To locate Inky’s target, we first start by selecting the position two tiles in front of Pac-Man in his current direction of travel, similar to Pinky’s targeting method. 
/// From there, imagine drawing a vector from Blinky’s position to this tile, and then doubling the length of the vector. The tile that this new, extended vector ends on will be Inky’s actual target.
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class InkyController : PhantomController
{
    [Header("Movement custom rules")]
    public Transform blinky;

    private const float DISTANCE_OFFSET = 2f;

    protected override Vector3 GetDestination()
    {
        Vector3 temporaryDestination = target.transform.position + target.transform.forward * DISTANCE_OFFSET;

        return (temporaryDestination - blinky.position) * 2f;
    }
}