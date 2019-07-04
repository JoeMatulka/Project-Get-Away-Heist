using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Player : Vehicle
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        col = GetComponent<BoxCollider>();

        ApplyDefaultVehicleRigidbodyOptions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Accelerate(SPEED * Input.GetAxis("Vertical"));
        ApplyTorque(TURN_AXIS * Input.GetAxis("Horizontal"));
    }
}
