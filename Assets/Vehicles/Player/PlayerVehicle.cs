using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle: Vehicle
{
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Move(input);
    }
}
