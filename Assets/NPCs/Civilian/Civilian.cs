﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Civilian : Vehicle
{
    private float speed = .475f;
    private float origSpeed = .5f;

    private const float TURN_SMOOTH_SPEED = 1f;

    private const float DEF_DIST_TO_STOP = 2.5f;
    private float distToStop = DEF_DIST_TO_STOP;

    public Path Path;
    public PathWayPoint Destination;
    public int CurrentDestinationIndex = 0;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    private CivilianState aiState = CivilianState.NORMAL;
    private const float FRENZIED_STATE_MOD = 2f;
    private const float FRENZIED_STATE_TURN_MOD = 10;

    void Awake()
    {
        origSpeed = speed;

        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;
    }

    void Start()
    {
        // Rotate towards destination
        Destination = Path.Waypoints[CurrentDestinationIndex];
        transform.LookAt(Destination.transform);
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
            speed = origSpeed * FRENZIED_STATE_MOD;
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

        if (Path != null && Path.Waypoints != null)
        {
            MoveToDestination();
        }
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

    public void PullOver(Vector3 locationOfPolice)
    {
        if (aiState.Equals(CivilianState.NORMAL))
        {
            Quaternion currentRotation = transform.rotation;

            float turnAxis = currentRotation.y;
            turnAxis += locationOfPolice.x > transform.localPosition.x ? -FRENZIED_STATE_TURN_MOD : FRENZIED_STATE_TURN_MOD;

            Quaternion targetRotation = new Quaternion(currentRotation.x, turnAxis, currentRotation.z, currentRotation.w);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);

            Invoke("Halt", 2);
            Invoke("SetAiStateToNormal", 5);
        }
    }

    private void MoveToDestination()
    {
        if (Vector3.Distance(transform.position, Destination.Position) > distToStop)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Destination.Position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);

            Move(speed, 0);
        }
        else
        {
            // Wait if current destination is marked for stop
            if (!Destination.stop || aiState.Equals(CivilianState.FRENZIED))
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
        if (Destination.lastWayPoint)
        {
            CurrentDestinationIndex = 0;
        }
        else
        {
            CurrentDestinationIndex++;
        }
        Destination = Path.Waypoints[CurrentDestinationIndex];
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
        Destination = Path.Waypoints[CurrentDestinationIndex];
    }

    public void Halt()
    {
        aiState = CivilianState.HALTED;
    }

    public void SetAiStateToNormal()
    {
        aiState = CivilianState.NORMAL;
    }

    public CivilianState AIState
    {
        get
        {
            return aiState;
        }
    }
}
