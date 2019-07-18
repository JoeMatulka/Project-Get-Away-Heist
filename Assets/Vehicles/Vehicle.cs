using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    protected BoxCollider Collider;

    protected const float SPEED = 11000;
    private const float MAX_VELOCITY = 100;
    private float inputAccel = 0;

    protected const float TURN_AXIS = 9500;

    private bool wheelsOnGround = false;

    protected void Accelerate(float accel)
    {
        inputAccel = accel;
        if (wheelsOnGround)
        {
            m_rigidbody.AddForce((inputAccel * SPEED) * transform.forward);
            Vector3.ClampMagnitude(m_rigidbody.velocity, MAX_VELOCITY);
        }
    }

    protected void Brake()
    {

    }

    protected void Turn(float turn)
    {
        if (wheelsOnGround)
        {
            m_rigidbody.AddTorque((turn * TURN_AXIS) * transform.up);
        }
    }

    protected void CheckGroundStatus()
    {
        CheckFrontBackSensors();
        CheckBottomSensor();
    }

    private void CheckBottomSensor()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            wheelsOnGround = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            wheelsOnGround = false;
        }
    }

    private void CheckFrontBackSensors()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer

        // Need to grab position at bottom of collider
        Vector3 position = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );

        // Calculate ray length
        float length = Collider.size.z * 1.5f;

        // Check if vehicle is going forward or backwards
        Vector3 rayDirection = inputAccel >= 0 ? Quaternion.Euler(25, 0, 0) * Vector3.forward : Quaternion.Euler(-25, 0, 0) * -Vector3.forward;

        if (Physics.Raycast(position, transform.TransformDirection(rayDirection), out hit, length, layerMask))
        {
            Debug.DrawRay(position, transform.TransformDirection(rayDirection) * hit.distance, Color.yellow);
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            Debug.DrawRay(position, transform.TransformDirection(rayDirection) * (length), Color.white);
        }
    }

    protected float Weight
    {
        set
        {
            if (m_rigidbody != null)
            {
                m_rigidbody.mass = value;
            }
        }
    }

    protected Rigidbody Rigidbody
    {
        set
        {
            m_rigidbody = value;
        }
    }
}