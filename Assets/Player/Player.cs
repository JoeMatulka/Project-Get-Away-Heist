using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
public class Player : Vehicle
{
    public PlayerControls input;

    void Start()
    {
        input = GetComponent<PlayerControls>();

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();

        Accelerate(input.Acceleration);
        Turn(input.Steering);
        Brake(input.isBraking);
    }
}
