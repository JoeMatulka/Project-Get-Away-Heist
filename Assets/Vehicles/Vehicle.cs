using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Vehicle : MonoBehaviour
{
    protected Rigidbody rb;

    protected BoxCollider col;

    protected const float SPEED = 10000;

    protected const float TURN_AXIS = 10000;

    protected void Accelerate(float accel) {
        rb.AddRelativeForce(accel * Vector3.forward);
    }

    protected void Brake() {

    }

    protected void ApplyTorque(float turn) {
        rb.AddTorque(turn * Vector3.up);
    }

    protected void ApplyDefaultVehicleRigidbodyOptions() {
        if (rb != null) {
            rb.mass = 750;
        }
    }
}