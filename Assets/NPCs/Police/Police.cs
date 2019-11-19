using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Police : Vehicle
{
    public bool Disable = false;

    private Player player;

    private float speed = STRAIGHT_SPEED;
    private const float STRAIGHT_SPEED = 1;
    private const float TURN_SPEED = .75f;
    private const float CONTACT_SPEED = .5f;
    private const float TURN_SLOWDOWN_THRESHOLD = 35f;

    public PathWayPoint currentWaypoint;
    private int currentWaypointIndex = 0;
    private const float CHECK_DIST_TO_PLAYER = 50f;
    private const float DIST_TO_DESTINATION = 8f;
    public bool pursuePlayerDirectly;

    private bool needsToReverse = false;
    private Vector3 reverseFromTarget = Vector3.zero;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 5f;
    private const float CONTACT_TURN_MOD = 180;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Player.PLAYER_OBJ_NAME).GetComponent<Player>();

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
        CheckSensors();
        if (player != null && !Disable)
        {
            if (currentWaypoint == null &&
                !pursuePlayerDirectly)
            {
                FindClosestWaypoint();
            }
            else
            {
                if (!needsToReverse)
                {
                    MoveToDestination();
                }
                else
                {
                    Reverse();
                }
            }
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // TODO: Change this from plane to ground mesh, probably a tag that identifies as ground or something
        if (!collisionInfo.collider.name.Equals("Plane") && !needsToReverse) {
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                // TODO: Farm these variables out to constants
                if (Vector3.Distance(contact.point, transform.localPosition) <= 1.5f &&
                    Rigidbody.velocity.magnitude <= .3f) {
                    reverseFromTarget = contact.point;
                    needsToReverse = true;
                }
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
        CheckIfPlayerInSight(playerPos);

        if (!pursuePlayerDirectly &&
            Vector3.Distance(transform.position, currentWaypoint.Position) < DIST_TO_DESTINATION)
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

        if (sensor.Contact != null && !sensor.Contact.transform.tag.Equals(Player.PLAYER_OBJ_NAME))
        {
            float xPosOfContact = sensor.Contact.transform.position.x;
            angle += xPosOfContact > transform.localPosition.x ? -CONTACT_TURN_MOD : CONTACT_TURN_MOD;
            speed = CONTACT_SPEED;
        }

        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        Rigidbody.MoveRotation(Rigidbody.rotation * deltaRotation);

        Accelerate(speed, false);
    }

    private IEnumerator Reverse()
    {
        Accelerate(-speed, false);
        // Back up until sensors are clear
        yield return new WaitUntil(() => Vector3.Distance(reverseFromTarget, transform.localPosition) > 5);
        needsToReverse = false;
    }

    private void CheckSensors()
    {
        if (sensor.Contact != null)
        {
            if (sensor.Contact.GetComponentInParent<Civilian>() != null)
            {
                Civilian civilian = sensor.Contact.GetComponentInParent<Civilian>();
                civilian.PullOver(transform.position);
            }
        }
    }

    private void CheckIfPlayerInSight(Vector3 playerPos)
    {
        float distToPlayer = Vector3.Distance(transform.position, playerPos);
        // Only check if player is in sight if distance to player is less than threshold
        if (distToPlayer <= CHECK_DIST_TO_PLAYER)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;

            if (Physics.Raycast(transform.localPosition, playerPos - transform.localPosition, out hit, CHECK_DIST_TO_PLAYER, layerMask, QueryTriggerInteraction.Ignore) &&
                hit.transform.name.Equals(Player.PLAYER_OBJ_NAME))
            {
                Debug.DrawRay(transform.localPosition, playerPos - transform.localPosition, Color.green);
                pursuePlayerDirectly = true;
                currentWaypoint = null;
                currentWaypointIndex = 0;
            }
            else
            {
                Debug.DrawRay(transform.localPosition, playerPos - transform.localPosition, Color.red);
                pursuePlayerDirectly = false;
                FindClosestWaypoint();
            }
        }
    }
}
