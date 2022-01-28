using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Vector3 InputDirection;
    public Image Joystick;
    public Image JoystickContainer;


    // Start is called before the first frame update
    void Start()
    {
        InputDirection = Vector3.zero;

        if (JoystickContainer == null)
        {
            JoystickContainer = GetComponent<Image>();
        }

        if(Joystick == null)
        {
            Joystick = transform.GetChild(0).GetComponent<Image>(); //this command is used because there is only one child in hierarchy
        }

    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        //To get InputDirection
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                (JoystickContainer.rectTransform,
                ped.position,
                ped.pressEventCamera,
                out position);

        position.x = (position.x / JoystickContainer.rectTransform.sizeDelta.x);
        position.y = (position.y / JoystickContainer.rectTransform.sizeDelta.y);

        float x = (JoystickContainer.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;
        float y = (JoystickContainer.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;

        InputDirection = new Vector3(x, y, 0);
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        //to define the area in which joystick can move around
        Joystick.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (JoystickContainer.rectTransform.sizeDelta.x / 3)
                                                               , InputDirection.y * (JoystickContainer.rectTransform.sizeDelta.y / 3));

    }

    public void OnPointerDown(PointerEventData ped)
    {

        OnDrag(ped);
    }

    public void OnPointerUp(PointerEventData ped)
    {

        InputDirection = Vector3.zero;
        Joystick.rectTransform.anchoredPosition = Vector3.zero;
    }
}
