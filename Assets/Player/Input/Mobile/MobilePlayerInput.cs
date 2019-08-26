using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    public Button itemButton;

    private float accelInput, steerInput;

    void Start()
    {
        itemButton.onClick.AddListener(UseItem);
    }

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        steerInput = steerJoystick.Horizontal;
    }

    public float GetAcceleration()
    {
        return accelInput;
    }

    public float GetSteering()
    {
        return steerInput;
    }

    public bool isBraking()
    {
        throw new System.NotImplementedException();
    }

    public void UseItem()
    {
        throw new System.NotImplementedException();
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }
}
