using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalBannerController : MonoBehaviour
{

    public Text Goal;

    public void StartAnimation()
    {
        ResetScale();
        gameObject.SetActive(true);
        LeanTween.scale(gameObject, Vector3.one, 0.75f).setEaseOutElastic();
    }

    public void ResetScale()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0);
        gameObject.SetActive(false);
    }

}
