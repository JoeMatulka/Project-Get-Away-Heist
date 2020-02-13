using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MoneyTruck : Vehicle
{
    private float speed = STRAIGHT_SPEED;
    private const float STRAIGHT_SPEED = 1.05f;
    private const float TURN_SPEED = .35f;
    // Multiplicative value used on calculating the turn rotation, allows for the money truck to make tighter turns at higher speeds
    private const float ROT_TORQUE_MOD = 2.25f;

    private const float DIST_TO_DESTINATION = 3.5f;

    private const float TURN_SLOWDOWN_THRESHOLD = 45f;

    public Path Path;
    public PathWayPoint destination;
    // Dot calculation threshold to determine the next waypoint being chose
    private const float DEST_FIND_DOT_THRESHOLD = .5f;
    // Used to determine if a connected waypoint is a viable option to go to next
    private const float DEF_POTENTIAL_DEST_DOT = -0.05f;
    private const float POTENTIAL_DEST_INCREMENT = .05f;
    private float nextPotentialWaypointDot = DEF_POTENTIAL_DEST_DOT;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    private const string MONEY_TRAIL_GAME_OBJ= "Money Trail";
    private GameObject moneyTrail;
    public Money SpawnedMoneyObj;
    // TODO: Change this to be dynamic
    public float amountOfMoneyLeft = 600000000f;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 2000;
    }

    void Start()
    {
        moneyTrail = new GameObject(MONEY_TRAIL_GAME_OBJ);
        // TODO: Move this out of start later and be controller by some sort of game controller
        InvokeRepeating("SpawnMoney", .25f, .25f);
    }
    
    void FixedUpdate()
    {
        CheckGroundStatus(false);

        MoveToDestination();
    }

    private void MoveToDestination()
    {
        if (destination == null && Path.Waypoints != null)
        {
            destination = FindClosestWayPoint();
        }

        if (destination != null)
        {
            if (Vector3.Distance(transform.position, destination.Position) < DIST_TO_DESTINATION)
            {
                destination = FindNextDestination();
            }

            Vector3 target = destination.Position;
            Vector3 localTarget = transform.InverseTransformPoint(target);

            float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            // Slow down for turns
            speed = Mathf.Abs(angle) > TURN_SLOWDOWN_THRESHOLD ? TURN_SPEED : STRAIGHT_SPEED;

            Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
            Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * ROT_TORQUE_MOD);
            Rigidbody.MoveRotation(Rigidbody.rotation * deltaRotation);

            Accelerate(speed, false);
        }
    }

    // Goal is to find the closest waypoint also in our facing direction
    private PathWayPoint FindClosestWayPoint()
    {
        PathWayPoint dest = null;
        foreach (PathWayPoint wp in Path.Waypoints)
        {
            if (dest == null)
            {
                dest = wp;
            }
            if (DEST_FIND_DOT_THRESHOLD > Vector3.Dot(transform.forward, (dest.Position - transform.localPosition).normalized))
            {
                // If current destination is behind truck select this one automatically to avoid back tracking
                dest = wp;
            }
            // Check to see if vehicle is facing waypoint
            float dot = Vector3.Dot(transform.forward, (wp.Position - transform.localPosition).normalized);
            if (dot > DEST_FIND_DOT_THRESHOLD)
            {
                // Check to see waypoint is closer than currently selected destination
                float curDist = Vector3.Distance(transform.position, dest.Position);

                if (curDist > Vector3.Distance(transform.position, wp.Position))
                {
                    dest = wp;
                }
            }
        }
        return dest;
    }

    private PathWayPoint FindNextDestination()
    {
        PathWayPoint dest = destination;
        List<PathWayPoint> potentialNewDestinations = new List<PathWayPoint>();
        foreach (PathWayPoint wp in destination.ConnectedWayPoints)
        {
            // Check to see if vehicle is facing connected waypoint
            float dot = Vector3.Dot(transform.forward, (wp.Position - transform.localPosition).normalized);
            if (dot >= nextPotentialWaypointDot)
            {
                // Set this back to default, for next waypoint decision
                nextPotentialWaypointDot = DEF_POTENTIAL_DEST_DOT;
                potentialNewDestinations.Add(wp);
            }
        }
        // Determine next destination from given potential ones
        if (potentialNewDestinations.Count > 1)
        {
            int index = Random.Range(0, potentialNewDestinations.Count);
            dest = potentialNewDestinations[index];
        }
        else if (potentialNewDestinations.Count == 1)
        {
            // If only one in potential just grab it
            dest = potentialNewDestinations[0];
        }
        else
        {
            // Try to find a suitable next waypoint, but with a wide DOT threshold this time
            nextPotentialWaypointDot += POTENTIAL_DEST_INCREMENT;
            FindClosestWayPoint();
        }
        return dest;
    }

    private void OnCollisionEnter(Collision col) {
        if (col.gameObject.GetComponent<Vehicle>() != null) {
            Vehicle vehicle = col.gameObject.GetComponent<Vehicle>();
            Vector3 oppositeVector = vehicle.transform.position  - transform.position;
            oppositeVector.Normalize();

            vehicle.Rigidbody.AddForce(oppositeVector * 20, ForceMode.Impulse);
        }
    }

    private void SpawnMoney()
    {
        if (SpawnedMoneyObj != null && amountOfMoneyLeft >= 0)
        {
            Money moneyObj = Instantiate(SpawnedMoneyObj, transform.position, Quaternion.identity) as Money;
            moneyObj.transform.SetParent(moneyTrail.transform);
            amountOfMoneyLeft -= moneyObj.Amount;
        }
    }
}
