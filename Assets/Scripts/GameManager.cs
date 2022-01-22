using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _score;

    #region SINGLETON
    // Static singleton instance
    private static GameManager _instance;

    // Static singleton property
    public static GameManager Instance {
        // lazzy loading
        get { return _instance ?? (_instance = new GameManager()); }
        private set { _instance = value; }
    }
    #endregion

    public void IncreaseScore(int points) {
        _score += points;
    }
}
