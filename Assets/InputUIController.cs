using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputUIController : MonoBehaviour
{
    [TitleGroup("UI References")]
    public Image InputFeedback;

    [TitleGroup("Ressources")]
    public Sprite InputFeedbackUp;
    public Sprite InputFeedbackRight;
    public Sprite InputFeedbackDown;
    public Sprite InputFeedbackLeft;

    public void UpdateInputFeedback(DirectionEnum direction)
    {
        switch(direction)
        {
            case DirectionEnum.Up:
                InputFeedback.sprite = InputFeedbackUp;
                break;

            case DirectionEnum.Right:
                InputFeedback.sprite = InputFeedbackRight;
                break;

            case DirectionEnum.Down:
                InputFeedback.sprite = InputFeedbackDown;
                break;

            case DirectionEnum.Left:
                InputFeedback.sprite = InputFeedbackLeft;
                break;
        }
    }


}
