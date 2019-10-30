using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Civilian : Vehicle
{
    public float Speed = .5f;
    private float origSpeed = .5f;

    private const float TURN_SMOOTH_SPEED = 1.5f;

    private const float DEF_DIST_TO_STOP = 2.5f;
    private float distToStop = DEF_DIST_TO_STOP;

    public Path Path;
    private PathWayPoint destination;
    public int CurrentDestinationIndex = 0;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    private CivilianState aiState = CivilianState.NORMAL;
    private const float FRENZIED_STATE_MOD = 2f;
    private const float FRENZIED_STATE_TURN_MOD = 10;

    void Start()
    {
        origSpeed = Speed;

        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;

        if (Path.reverse)
        {
            CurrentDestinationIndex = Path.Waypoints.Length - 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGroundStatus(false);

        if (aiState.Equals(CivilianState.HALTED))
        {
            return;
        }

        if (sensor.Contact != null && aiState.Equals(CivilianState.NORMAL))
        {
            return;
        }

        if (aiState.Equals(CivilianState.FRENZIED))
        {
            Speed = origSpeed * FRENZIED_STATE_MOD;
            distToStop = DEF_DIST_TO_STOP * FRENZIED_STATE_MOD;

            if (sensor.Contact != null)
            {
                // Turn to get out of things way while frenzied
                Quaternion currentRotation = transform.rotation;

                float turnAxis = currentRotation.y;
                turnAxis += FRENZIED_STATE_TURN_MOD;

                Quaternion targetRotation = new Quaternion(currentRotation.x, turnAxis, currentRotation.z, currentRotation.w);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);
            }
        }

        MoveToDestination();
    }

    void OnCollisionEnter(Collision collider)
    {
        if (aiState.Equals(CivilianState.NORMAL))
        {
            if (collider.gameObject.GetComponent<Vehicle>() != null)
            {
                ;
                if (Random.Range(0, 5) == 0)
                {
                    aiState = CivilianState.FRENZIED;
                }
                else
                {
                    aiState = CivilianState.HALTED;
                }
            }
        }
    }

    private void MoveToDestination()
    {
        if (destination == null)
        {
            destination = Path.Waypoints[CurrentDestinationIndex];
        }

        if (Vector3.Distance(transform.position, destination.Position) > distToStop)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destination.Position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);

            Accelerate(Speed, false);
        }
        else
        {
            // Wait if current destination is marked for stop
            if (!destination.stop || aiState.Equals(CivilianState.FRENZIED))
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
        destination = Path.Waypoints[CurrentDestinationIndex];
    }

    private void DecrementDestination()
    {
        if (CurrentDestinationIndex == 0)
        {
            CurrentDestinationIndex = Path.Waypoints.Length - 1;
        }
        else
        {
            CurrentDestinationIndex--;
        }
        destination = Path.Waypoints[CurrentDestinationIndex];
    }

    public CivilianState AIState
    {
        get
        {
            return aiState;
        }
    }
}
