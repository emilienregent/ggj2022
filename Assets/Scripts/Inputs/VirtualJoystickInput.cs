using UnityEngine;
public class VirtualJoystickInput : MonoBehaviour
{
    
    public DirectionEnum CurrentDirection;
    public VirtualJoystickHandler Joystick;

    void Start()
    {
        
        CurrentDirection = DirectionEnum.None;
    }

    private void Update()
    {

        if (Joystick.InputDirection.x == 0 && Joystick.InputDirection.y ==0)
        {
            CurrentDirection = DirectionEnum.None;
            return;
        }

        // Find direction based on the input
        // X > Y = Horizontal movement
        // Y > X = Vertical movememnt
        if (Mathf.Abs(Joystick.InputDirection.x) >= Mathf.Abs(Joystick.InputDirection.y))
        {

            if (Joystick.InputDirection.x > 0)
            {
                CurrentDirection = DirectionEnum.Right;
            }
            else
            {
                CurrentDirection = DirectionEnum.Left;
            }
        }
        else
        {
            if (Joystick.InputDirection.y > 0)
            {
                CurrentDirection = DirectionEnum.Up;
            }
            else
            {
                CurrentDirection = DirectionEnum.Down;
            }
        }
    }
}