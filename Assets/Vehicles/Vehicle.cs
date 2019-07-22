using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    protected BoxCollider Collider;

    private const float MAX_SLOPE_ANGLE = 45f;

    public static float BASE_SPEED = 11000;

    protected float SPEED = BASE_SPEED;
    private const float MAX_VELOCITY = 50;
    private float inputAccel = 0;

    protected const float TURN_AXIS = 8000;
    private const float MAX_TURN_RADIUS = 1;

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
            Vector3.ClampMagnitude(m_rigidbody.angularVelocity, MAX_TURN_RADIUS);
        }
    }

    protected void CheckGroundStatus()
    {
        CheckFrontBackSensors();
        CheckBottomSensor();
        if (!wheelsOnGround)
        {
            AllignToGround();
        }
    }

    private void AllignToGround()
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
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
        }
    }

    private void CheckBottomSensor()
    {
        // Grounded length check
        float groundCheck = .75f;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, groundCheck, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            wheelsOnGround = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundCheck, Color.white);
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

        // Calculate ray length
        float length = Collider.size.z * 1.5f;

        // Check if vehicle is going forward or backwards
        Vector3 rayDirection = inputAccel > 0 ? Quaternion.Euler(25, 0, 0) * Vector3.forward : Quaternion.Euler(-25, 0, 0) * -Vector3.forward;

        if (Physics.Raycast(transform.position, transform.TransformDirection(rayDirection), out hit, length, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(rayDirection) * hit.distance, Color.yellow);
            if (Vector3.Angle(hit.normal, transform.forward) - 90 <= MAX_SLOPE_ANGLE)
            {
                // Only allign with normals from hit if the normal is under the max slope angle
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(rayDirection) * (length), Color.white);
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

    public Rigidbody Rigidbody
    {
        set
        {
            m_rigidbody = value;
        }
        get {
            return m_rigidbody;
        }
    }
}