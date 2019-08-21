using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    private float accelInput, steerInput;

    private float previousSteerInput;

    private const float INPUT_SMASH_THRESHOLD = .35f;
    private const float INPUT_STEER_BRAKE_THRESHOLD = .85f;

    private bool m_isBraking;

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        steerInput = steerJoystick.Horizontal;

        // Check if steering input has been smashed to enabled braking
        if (Mathf.Abs(steerInput) >= INPUT_STEER_BRAKE_THRESHOLD && 
            Mathf.Abs(steerInput - previousSteerInput) > INPUT_SMASH_THRESHOLD) {
            m_isBraking = true;
        }
        // If braking, stop braking when input reaches below turn brake threshold
        if (m_isBraking && Mathf.Abs(steerInput) <= INPUT_STEER_BRAKE_THRESHOLD) {
            m_isBraking = false;
        }
    }

    void LateUpdate() {
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
        return m_isBraking;
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }
}
