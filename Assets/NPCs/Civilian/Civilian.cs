using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Civilian : Vehicle
{
    public float Speed = .5f;

    private const float TURN_SMOOTH_SPEED = 1f;

    private const float DIST_TO_STOP = 1f;

    public Path Path;
    private PathWayPoint destination;
    public int CurrentDestinationIndex = 0;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();

        Collider = GetComponentInChildren<BoxCollider>();

        Weight = 200;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGroundStatus(false);

        StartCoroutine(MoveToDestination());
    }

    private IEnumerator MoveToDestination() {
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
            yield return null;
        }
        else
        {
            // Wait if current destination is marked for stop
            if (!destination.stop)
            {
                IncrementDestination();
            }
        }
    }

    private void IncrementDestination() {
        CurrentDestinationIndex++;
        destination = Path.waypoints[CurrentDestinationIndex];
    }
}
