using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MoneyTruck : Vehicle
{
    private float speed = STRAIGHT_SPEED;
    private const float STRAIGHT_SPEED = 1.25f;
    private const float TURN_SPEED = .5f;

    private const float DIST_TO_DESTINATION = 3.5f;

    private const float TURN_SLOWDOWN_THRESHOLD = 45f;

    public Path Path;
    public PathWayPoint destination;
    // Dot calculation threshold to determine the next waypoint being chose
    private const float DEST_FIND_DOT_THRESHOLD = .5f;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 400;
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
            Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
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
            if (dot >= -0.05)
            {
                potentialNewDestinations.Add(wp);
            }
        }
        // Determine next destination from given potential ones
        if (potentialNewDestinations.Count > 1)
        {
            int index = Random.Range(0, potentialNewDestinations.Count - 1);
            Debug.Log("Grabbing random " + index + ", from " + potentialNewDestinations.Count);
            dest = potentialNewDestinations[index];
        }
        else
        {
            Debug.Log("Grabbing one");
            // If only one in potential just grab it
            dest = potentialNewDestinations[0];
        }
        return dest;
    }
}
