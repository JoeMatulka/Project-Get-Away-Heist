using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
[RequireComponent(typeof(PlayerConfig))]
public class Player : Vehicle
{
    public PlayerControls input;

    private PlayerConfig config;

    public PlayerGadgets gadgets;

    public Path PlayerTrailPath;
    private const string PLAYER_PATH_NAME = "Player Path";
    private readonly Color PLAYER_PATH_COLOR = Color.white;

    void Start()
    {
        config = GetComponent<PlayerConfig>();

        gadgets = GetComponent<PlayerGadgets>();
        gadgets.player = this;

        input = GetComponent<PlayerControls>();
        input.GetItemEvent().AddListener(UseItem);

        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;

        GameObject playerPath = new GameObject(PLAYER_PATH_NAME);
        PlayerTrailPath = playerPath.AddComponent<Path>();
        PlayerTrailPath.rayColor = PLAYER_PATH_COLOR;
    }

    void FixedUpdate()
    {
        CheckGroundStatus(gadgets.IsParachuting);

        float accel = input.Acceleration;
        float steer = input.Steering;

        if (gadgets.IsParachuting)
        {
            accel /= 2;
            steer /= 1000;
        }

        Accelerate(accel, gadgets.IsParachuting);
        Turn(steer, gadgets.IsParachuting);

        if (!gadgets.IsParachuting)
        {
            AddToPlayerTrail();
        }
    }

    private void AddToPlayerTrail()
    {
        PlayerTrailPath.AddWayPoint(transform.position);
    }

    private void UseItem()
    {
        config.PlayerItem.Use();
    }
}
