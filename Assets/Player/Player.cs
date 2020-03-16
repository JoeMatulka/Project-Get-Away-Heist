using Heist;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerControls))]
[RequireComponent(typeof(PlayerConfig))]
public class Player : Vehicle
{
    public static string PLAYER_OBJ_NAME = "Player";

    public PlayerControls input;

    private PlayerConfig config;

    public PlayerGadgets gadgets;

    public Path PlayerTrailPath;
    private const string PLAYER_PATH_NAME = "Player Path";
    private readonly Color PLAYER_PATH_COLOR = Color.white;

    private ScoreEvent scoreEvent;

    void Awake()
    {
        name = PLAYER_OBJ_NAME;

        config = GetComponent<PlayerConfig>();
        // TODO: Change this to come from player configuration later
        config.PlayerItemId = (int) PlayerItemLibrary.JUMP;

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

        // TODO: Remove this in the future
        HeistService.Instance.CreateHeist();
    }

    void Start()
    {
        // TODO: Remove this in the future
        StartCoroutine(GameSceneManager.Instance.SetUpGameScene());

        scoreEvent = HeistService.Instance.FindCurrentHeist().AddToScore;
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

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponentInParent<Money>() != null)
        {
            Money money = col.GetComponentInParent<Money>();
            scoreEvent.Invoke(money.Amount);
            Destroy(money.gameObject);
        }
    }
}
