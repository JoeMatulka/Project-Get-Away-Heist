using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerVehicle: Vehicle
{
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    void Update()
    {

    }
}
