using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles the controls used for Player input
 */
public class PlayerControls : MonoBehaviour
{
    private float m_accel, m_steering;

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
            m_steering = Input.GetAxis("Horizontal") * 250;
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
}
