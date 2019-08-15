﻿using UnityEngine;

/**
 * Handles the controls used for Player input
 */
public class PlayerControls : MonoBehaviour
{
    private float m_accel, m_steering;

    private bool m_isBraking;

    public PlayerInput input;

    void Start() {
        // TODO fix this later
        input = GameObject.Find("Mobile Input Canvas").GetComponent<MobilePlayerInput>();
    }

    void Update()
    {
        if (!Application.isEditor)
        {
            m_accel = input.getAcceleration();
            m_steering = input.getSteering();
        }
        else {
            // TODO fix this later
            m_accel = Input.GetAxis("Vertical");
            m_steering = Input.GetAxis("Horizontal");
            m_isBraking = Input.GetKey(KeyCode.Space);
        }
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

    public bool isBraking
    {
        get {
            return m_isBraking;
        }
    }
}
