
using UnityEngine;

/// <summary>
/// Calculates his distance from Pac-Man. If he is farther than eight tiles away, his targeting is identical to Blinky’s, using Pac-Man’s current tile as his target.
/// As soon as his distance to Pac-Man becomes less than eight tiles, Clyde’s target is set to the same tile as his fixed one in Scatter mode, just outside the bottom-left corner of the maze.
/// <see cref="https://steamcommunity.com/sharedfiles/filedetails/?id=593226813"/>
/// </summary>
public class ClydeController : PhantomController
{
    private const float DISTANCE_TRESHOLD = 8f;

    protected override Vector3 GetDestination()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > DISTANCE_TRESHOLD)
        {
            return base.GetDestination();
        }

        return _phantomMovementController.fallbackDestination.position;
    }
}