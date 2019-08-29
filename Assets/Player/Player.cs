using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
public class Player : Vehicle
{
    public PlayerControls input;

    public PlayerItem item;

    void Start()
    {
        //item.Player = this;
        // TODO - Fix this later
        item = new RocketBoost();

        input = GetComponent<PlayerControls>();
        input.GetItemEvent().AddListener(UseItem);

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();

        Accelerate(input.Acceleration);
        Turn(input.Steering);
    }

    private void UseItem() {
        item.Use();
    }
}
