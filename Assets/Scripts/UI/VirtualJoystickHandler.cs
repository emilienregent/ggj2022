using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    [Header("Configuration")]
    public float JoystickVisualDistance = 50f;

    [Header("UI References")]
    public Image Joystick;
    public Image JoystickContainer;

    private Vector3 _inputDirection;
    public Vector3 InputDirection { get => _inputDirection; set => _inputDirection = value; }


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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickContainer.rectTransform, ped.position, ped.pressEventCamera, out position);

        position.x = (position.x / JoystickContainer.rectTransform.sizeDelta.x);
        position.y = (position.y / JoystickContainer.rectTransform.sizeDelta.y);

        // Pivot might be giving us an offset, adjust it here
        Vector2 p = JoystickContainer.rectTransform.pivot;
        position.x += p.x - 0.5f;
        position.y += p.y - 0.5f;

        // Clamp our values
        float x = Mathf.Clamp(position.x, -1, 1); /*(JoystickContainer.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;*/
        float y = Mathf.Clamp(position.y, -1, 1);/*(JoystickContainer.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;*/

        InputDirection = new Vector3(x, y, 0);

        Joystick.rectTransform.anchoredPosition = new Vector3(x * JoystickVisualDistance, y * JoystickVisualDistance);

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
