using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Vehicle
{
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 750;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();

        Accelerate(Input.GetAxis("Vertical"));
        Turn(Input.GetAxis("Horizontal"));
    }
}
