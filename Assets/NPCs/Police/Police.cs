using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Police : Vehicle
{
    private Player player;

    public float Speed = 1;

    public Vector3 destination;

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

        if (player != null)
        {
            FindDestination();
            MoveToDestination();
        }
    }

    private void FindDestination()
    {
        if (player.PlayerTrailPath != null &&
            player.PlayerTrailPath.Waypoints != null)
        {
            // Iterate through current player trail to navigate to player
            foreach (PathWayPoint waypoint in player.PlayerTrailPath.Waypoints)
            {
                if (destination == null)
                {
                    destination = waypoint.Position;
                }

                float curDist = Vector3.Distance(transform.position, destination);

                if (curDist > Vector3.Distance(transform.position, waypoint.Position))
                {
                    destination = waypoint.Position;
                }
            }
        }
        // Check to see if player is closest destination
        Vector3 playerPos = player.transform.position;
        if (destination == null ||
            Vector3.Distance(transform.position, destination) >
            Vector3.Distance(transform.position, playerPos))
        {
            destination = playerPos;
        }
    }

    private void MoveToDestination()
    {
        Vector3 localTarget = transform.InverseTransformPoint(destination);

        float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        Rigidbody.MoveRotation(Rigidbody.rotation * deltaRotation);

        Accelerate(Speed, false);
    }
}
