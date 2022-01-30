using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : PhantomController
{
    
    protected override void SetNewDirection()
    {
        if(GameManager.Instance.CurrentState == GameState.PACMAN)
        {
            base.SetNewDirection();
        }
    }
}
