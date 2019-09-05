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
        CheckGroundStatus();

        Accelerate(input.Acceleration);
        Turn(input.Steering);
    }

    private void UseItem() {
        item.Use();
    }
}
