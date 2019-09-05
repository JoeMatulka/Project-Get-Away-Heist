using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
public class Player : Vehicle
{
    public PlayerControls input;

    public PlayerItem item;

    public PlayerGadgets gadgets;

    void Start()
    {
        gadgets = GetComponent<PlayerGadgets>();
        gadgets.player = this;

        // TODO - Fix this later
        item = gameObject.AddComponent<Jump>();
        item.Player = this;

        input = GetComponent<PlayerControls>();
        input.GetItemEvent().AddListener(UseItem);

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;
    }

    void FixedUpdate()
    {
        CheckGroundStatus(gadgets.IsParachuting);

        float accel = input.Acceleration;
        float steer = input.Steering;

        if (gadgets.IsParachuting) {
            accel /= 2;
            steer /= 1000;
        }

        Accelerate(accel, gadgets.IsParachuting);
        Turn(steer, gadgets.IsParachuting);
    }

    private void UseItem() {
        item.Use();
    }
}
