using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class Player : Vehicle
{
    private PlayerInput input;
    void Start()
    {
        input = GetComponent<PlayerInput>();

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 750;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();

        Accelerate(input.Acceleration);
        Turn(input.Steering);
    }
}
