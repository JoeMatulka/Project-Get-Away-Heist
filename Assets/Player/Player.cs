using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
public class Player : Vehicle
{
    public PlayerControls input;

    private float accel_Input, turn_Input;

    void Start()
    {
        input = GetComponent<PlayerControls>();

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;
    }

    void Update() {
        accel_Input = input.Acceleration;
        turn_Input = input.Steering;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();

        Accelerate(accel_Input);
        Turn(turn_Input);
    }
}
