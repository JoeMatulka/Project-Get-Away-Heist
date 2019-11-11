using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Police : Vehicle
{
    public bool Disable = false;

    private Player player;

    private float speed = STRAIGHT_SPEED;
    private const float STRAIGHT_SPEED = 1;
    private const float TURN_SPEED = .75f;
    private const float TURN_SLOWDOWN_THRESHOLD = 35f;

    public PathWayPoint currentWaypoint;
    private int currentWaypointIndex = 0;
    private float distToDestination = 8f;
    public bool pursuePlayerDirectly;


    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGroundStatus(false);

        if (player != null && !Disable)
        {
            if (currentWaypoint == null &&
                !pursuePlayerDirectly)
            {
                FindClosestWaypoint();
            }
            else
            {
                MoveToDestination();
            }
        }
    }

    private void FindClosestWaypoint()
    {
        if (player.PlayerTrailPath != null &&
            player.PlayerTrailPath.Waypoints != null)
        {
            // Determine Closest Destination
            for (int i = 0; i < player.PlayerTrailPath.Waypoints.Length; i++)
            {
                if (this.currentWaypoint == null)
                {
                    currentWaypointIndex = i;
                    this.currentWaypoint = player.PlayerTrailPath.Waypoints[currentWaypointIndex];
                }

                float curDist = Vector3.Distance(transform.position, this.currentWaypoint.Position);

                if (curDist > Vector3.Distance(transform.position, player.PlayerTrailPath.Waypoints[i].Position))
                {
                    currentWaypointIndex = i;
                    this.currentWaypoint = player.PlayerTrailPath.Waypoints[currentWaypointIndex];
                }
            }
        }
    }

    private void MoveToDestination()
    {
        // Check to see if player is closest destination
        Vector3 playerPos = player.transform.position;
        if (currentWaypoint == null ||
            Vector3.Distance(transform.position, currentWaypoint.Position) >
            Vector3.Distance(transform.position, playerPos))
        {
            pursuePlayerDirectly = true;
            currentWaypoint = null;
            currentWaypointIndex = 0;
        }
        else
        {
            pursuePlayerDirectly = false;
        }

        if (!pursuePlayerDirectly &&
            Vector3.Distance(transform.position, currentWaypoint.Position) < distToDestination)
        {
            // Increment player path if available
            if (currentWaypointIndex - 1 >= 0)
            {
                currentWaypointIndex--;
                this.currentWaypoint = player.PlayerTrailPath.Waypoints[currentWaypointIndex];
            }
            else
            {
                pursuePlayerDirectly = true;
                currentWaypoint = null;
                currentWaypointIndex = 0;
            }
        }

        Vector3 target = pursuePlayerDirectly ? playerPos : currentWaypoint.Position;
        Vector3 localTarget = transform.InverseTransformPoint(target);

        float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        // Slow down for turns
        speed = Mathf.Abs(angle) > TURN_SLOWDOWN_THRESHOLD ? TURN_SPEED : STRAIGHT_SPEED;
        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        Rigidbody.MoveRotation(Rigidbody.rotation * deltaRotation);

        Accelerate(speed, false);
    }
}
