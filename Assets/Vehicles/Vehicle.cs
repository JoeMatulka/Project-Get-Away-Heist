using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    protected BoxCollider Collider;

    protected float hoverHeight = .1f;

    protected const float SPEED = 11000;
    private const float MAX_VELOCITY = 100;

    protected const float TURN_AXIS = 10000;

    private bool wheelsOnGround = false;

    protected void Accelerate(float accel)
    {
        if (wheelsOnGround)
        {
            m_rigidbody.AddForce((accel * SPEED) * transform.forward);
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
        Vector3 contact = CheckFrontBackSensors();
        CheckBottomSensor(contact);
    }

    private void CheckBottomSensor(Vector3 frontBackContact)
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
            Vector3 normal = hit.normal;
            if (frontBackContact != Vector3.zero)
            {
                normal = frontBackContact;
            }
            transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
            wheelsOnGround = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            wheelsOnGround = false;
        }
    }

    private Vector3 CheckFrontBackSensors()
    {
        Vector3 contact = Vector3.zero;

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
        float direction = Vector3.Dot(transform.forward, m_rigidbody.velocity);
        Vector3 rayDirection = direction >= 0 ? Quaternion.Euler(20, 0, 0) * Vector3.forward : Quaternion.Euler(-20, 0, 0) * -Vector3.forward;

        if (Physics.Raycast(position, transform.TransformDirection(rayDirection), out hit, length, layerMask))
        {
            Debug.DrawRay(position, transform.TransformDirection(rayDirection) * hit.distance, Color.yellow);
            contact = hit.normal;
        }
        else
        {
            Debug.DrawRay(position, transform.TransformDirection(rayDirection) * (length), Color.white);
        }
        return contact;
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