using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    public Button itemButton;

    private float accelInput, steerInput;

    private float previousSteerInput;

    private const float INPUT_SMASH_THRESHOLD = .35f;
    private const float INPUT_STEER_BRAKE_THRESHOLD = .85f;

    void Start()
    {
        itemButton.onClick.AddListener(UseItem);
    }

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        steerInput = steerJoystick.Horizontal;
    }

    void LateUpdate()
    {
        previousSteerInput = steerJoystick.Horizontal;
    }

    public float getAcceleration()
    {
        return accelInput;
    }

    public float getSteering()
    {
        return steerInput;
    }

    public bool isBraking()
    {
        throw new System.NotImplementedException();
    }

    public void UseItem()
    {

    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }
}
