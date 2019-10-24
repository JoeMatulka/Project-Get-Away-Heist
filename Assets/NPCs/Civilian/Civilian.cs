using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Civilian : Vehicle
{
    public float Speed = .5f;

    private const float TURN_SMOOTH_SPEED = 1.5f;

    private const float DIST_TO_STOP = 2.5f;

    public Path Path;
    private PathWayPoint destination;
    public int CurrentDestinationIndex = 0;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;

        if (Path.reverse)
        {
            CurrentDestinationIndex = Path.waypoints.Length - 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGroundStatus(false);

        if (sensor.Contact == null)
        {
            MoveToDestination();
        }
    }

    private void MoveToDestination()
    {
        if (destination == null)
        {
            destination = Path.waypoints[CurrentDestinationIndex];
        }

        if (Vector3.Distance(transform.position, destination.Position) > DIST_TO_STOP)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destination.Position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);

            Accelerate(Speed, false);
        }
        else
        {
            // Wait if current destination is marked for stop
            if (!destination.stop)
            {
                if (!Path.reverse)
                {
                    IncrementDestination();
                }
                else
                {
                    DecrementDestination();
                }
            }
        }
    }

    private void IncrementDestination()
    {
        if (destination.lastWayPoint)
        {
            CurrentDestinationIndex = 0;
        }
        else
        {
            CurrentDestinationIndex++;
        }
        destination = Path.waypoints[CurrentDestinationIndex];
    }

    private void DecrementDestination()
    {
        if (CurrentDestinationIndex == 0)
        {
            CurrentDestinationIndex = Path.waypoints.Length - 1;
        }
        else
        {
            CurrentDestinationIndex--;
        }
        destination = Path.waypoints[CurrentDestinationIndex];
    }
}
