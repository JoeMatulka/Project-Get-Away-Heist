using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Civilian : Vehicle
{
    private float speed = 0f;

    private const float TURN_SMOOTH_SPEED = 2f;

    private const float DEF_DIST_TO_STOP = 2.5f;
    private float distToStop = DEF_DIST_TO_STOP;

    public Path Path;
    public PathWayPoint Destination;
    public int CurrentDestinationIndex = 0;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    public CivilianState aiState = CivilianState.NORMAL;
    private const float FRENZIED_STATE_MOD = 2f;
    private const float FRENZIED_STATE_TURN_MOD = 10;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;

        baseStats = new Vehicle.Stats
        {
            TopSpeed = 10f,
            Acceleration = 5f,
            AccelerationCurve = 5f,
            Braking = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed = 5f,
            Steer = 10f,
            CoastingDrag = 8f,
            Grip = 1f,
            AddedGravity = 1f,
            Suspension = .2f
        };
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
            Brake(true);
            return;
        }

        if (sensor.Contact != null && aiState.Equals(CivilianState.NORMAL))
        {
            Brake(true);
            return;
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
        if (Vector3.Distance(transform.position, Destination.Position) > distToStop && !IsDisabled)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Destination.Position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TURN_SMOOTH_SPEED * Time.deltaTime);
            speed = aiState.Equals(CivilianState.FRENZIED) ? 1 : .2f;
            if (sensor.Contact != null && aiState.Equals(CivilianState.FRENZIED))
            {
                // Turn to get out of things way while frenzied
                Quaternion currentRotation = transform.rotation;

                float turnAxis = currentRotation.y;
                turnAxis += FRENZIED_STATE_TURN_MOD;

                Quaternion frenzyTargetRot = new Quaternion(currentRotation.x, turnAxis, currentRotation.z, currentRotation.w);
                transform.rotation = Quaternion.Slerp(currentRotation, frenzyTargetRot, TURN_SMOOTH_SPEED * Time.deltaTime);
            }
            Brake(false);
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
            speed = 0;
            Brake(true);
        }

        Move(speed, 0);
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
