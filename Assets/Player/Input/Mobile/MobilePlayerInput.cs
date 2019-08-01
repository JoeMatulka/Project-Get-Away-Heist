using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    private float accelInput, steerInput;

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        // Multiply to apply turning radius at minimum inputs
        steerInput = Mathf.Abs(steerJoystick.Horizontal) < .5f ? steerJoystick.Horizontal * 2f : steerJoystick.Horizontal;
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

    public void Pause()
    {
        throw new System.NotImplementedException();
    }
}
