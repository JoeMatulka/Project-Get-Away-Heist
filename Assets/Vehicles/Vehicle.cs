using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody rigidbody;

    private BoxCollider col;

    private Vector3 input;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Move(input);
    }

    private void Move(Vector3 move) {

    }
}
