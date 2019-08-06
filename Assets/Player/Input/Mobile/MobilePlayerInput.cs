using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    private float accelInput, steerInput;

    private const float STEER_MOD = 250;

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        steerInput = steerJoystick.Horizontal;
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
