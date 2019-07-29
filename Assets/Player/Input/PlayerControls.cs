using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles the controls used for Player input
 */
public class PlayerControls : MonoBehaviour
{
    private float m_accel, m_steering;

    private PlayerInput input;

    void Start() {

    }

    void Update()
    {
        m_accel = input.getAcceleration();
        m_steering = input.getSteering();
    }

    public float Acceleration
    {
        get
        {
            return m_accel;
        }
    }

    public float Steering
    {
        get
        {
            return m_steering;
        }
    }
}
