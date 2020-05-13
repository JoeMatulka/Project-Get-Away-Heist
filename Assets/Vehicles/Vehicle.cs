using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    public struct Stats
    {
        [Header("Movement Settings")]
        [Tooltip("The maximum speed forwards")]
        public float TopSpeed;

        [Tooltip("How quickly the Kart reaches top speed.")]
        public float Acceleration;

        [Tooltip("The maximum speed backward.")]
        public float ReverseSpeed;

        [Tooltip("The rate at which the kart increases its backward speed.")]
        public float ReverseAcceleration;

        [Tooltip("How quickly the Kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float AccelerationCurve;

        [Tooltip("How quickly the Kart slows down when going in the opposite direction.")]
        public float Braking;

        [Tooltip("How quickly to slow down when neither acceleration or reverse is held.")]
        public float CoastingDrag;

        [Range(0, 1)]
        [Tooltip("The amount of side-to-side friction.")]
        public float Grip;

        [Tooltip("How quickly the Kart can turn left and right.")]
        public float Steer;

        [Tooltip("Additional gravity for when the Kart is in the air.")]
        public float AddedGravity;

        [Tooltip("How much the Kart tries to keep going forward when on bumpy terrain.")]
        [Range(0, 1)]
        public float Suspension;

        // allow for stat adding for powerups.
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
            {
                Acceleration = a.Acceleration + b.Acceleration,
                AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                Braking = a.Braking + b.Braking,
                CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                AddedGravity = a.AddedGravity + b.AddedGravity,
                Grip = a.Grip + b.Grip,
                ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
                TopSpeed = a.TopSpeed + b.TopSpeed,
                Steer = a.Steer + b.Steer,
                Suspension = a.Suspension + b.Suspension
            };
        }
    }

    private Rigidbody m_rigidbody;
    private float weight;

    protected BoxCollider Collider;

    // Used for ignoring ground allignment at certain angles
    private const float MAX_SLOPE_ANGLE = 45f;
    // Used to ignore slight changes in ground angles, prevents bumpy ride
    private const float SLOPE_DIFF_IGNORE_ANGLE = 2f;

    private const float SMOOTH_ROT_SPEED = 20f;

    private float inputAccel;

    private bool wheelsOnGround = false;
    public float MinHeightThreshold = 0.75f;
    protected float groundedHeight = float.MaxValue;

    private const float DEF_DRAG = 1;
    private const float DEF_ANGULAR_DRAG = .05f;

    private bool isBraking = false;

    public bool IsDisabled = false;

    Vehicle.Stats finalStats;

    protected Vehicle.Stats baseStats = new Vehicle.Stats
    {
        TopSpeed = 12.5f,
        Acceleration = 5f,
        AccelerationCurve = 5f,
        Braking = 10f,
        ReverseAcceleration = 5f,
        ReverseSpeed = 10f,
        Steer = 10f,
        CoastingDrag = 4f,
        Grip = .95f,
        AddedGravity = 1f,
        Suspension = .2f
    };


    protected void Move(float accel, float turn)
    {
        finalStats = baseStats;
        inputAccel = accel;

         MoveVehicle(accel, turn);
    }

    void MoveVehicle(float accelInput, float turnInput)
    {
        // manual acceleration curve coefficient scalar
        float accelerationCurveCoeff = 5;
        Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

        bool accelDirectionIsFwd = accelInput >= 0;
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // use the max speed for the direction we are going--forward or reverse.
        float maxSpeed = accelDirectionIsFwd ? finalStats.TopSpeed : finalStats.ReverseSpeed;
        float accelPower = accelDirectionIsFwd ? finalStats.Acceleration : finalStats.ReverseAcceleration;

        float accelRampT = Rigidbody.velocity.magnitude / maxSpeed;
        float multipliedAccelerationCurve = finalStats.AccelerationCurve * accelerationCurveCoeff;
        float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

        bool braking = accelDirectionIsFwd != localVelDirectionIsFwd || isBraking;

        // if we are braking (moving reverse to where we are going)
        // use the braking accleration instead
        float finalAccelPower = braking ? finalStats.Braking : accelPower;

        float finalAcceleration = finalAccelPower * accelRamp;

        // apply inputs to forward/backward
        float turningPower = turnInput * finalStats.Steer;

        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, Rigidbody.transform.up);
        Vector3 fwd = turnAngle * Rigidbody.transform.forward;

        Vector3 movement = fwd * accelInput * finalAcceleration;

        // simple suspension allows us to thrust forward even when on bumpy terrain
        fwd.y = Mathf.Lerp(fwd.y, 0, finalStats.Suspension);

        // forward movement
        float currentSpeed = Rigidbody.velocity.magnitude;
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;


        // if over max speed, cannot accelerate faster.
        if (wasOverMaxSpeed && !braking) movement *= 0;


        Vector3 adjustedVelocity = Rigidbody.velocity + movement * Time.deltaTime;

        adjustedVelocity.y = Rigidbody.velocity.y;

        //  clamp max speed if we are on ground
        if (wheelsOnGround)
        {
            if (adjustedVelocity.magnitude > maxSpeed && !wasOverMaxSpeed)
            {
                adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
            }
        }

        // coasting is when we aren't touching accelerate
        bool isCoasting = Mathf.Abs(accelInput) < .01f;

        if (isCoasting)
        {
            Vector3 restVelocity = new Vector3(0, Rigidbody.velocity.y, 0);
            adjustedVelocity = Vector3.MoveTowards(adjustedVelocity, restVelocity, Time.deltaTime * finalStats.CoastingDrag);
        }

        if (!float.IsNaN(adjustedVelocity.x) && !float.IsNaN(adjustedVelocity.y) && !float.IsNaN(adjustedVelocity.z))
        {
            Rigidbody.velocity = adjustedVelocity;
        }

        ApplyAngularSuspension();

        if (wheelsOnGround && !IsDisabled)
        {
            // manual angular velocity coefficient
            float angularVelocitySteering = .4f;
            float angularVelocitySmoothSpeed = 20f;

            // turning is reversed if we're going in reverse and pressing reverse
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd) angularVelocitySteering *= -1;
            var angularVel = Rigidbody.angularVelocity;

            // move the Y angular velocity towards our target
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);

            // apply the angular velocity
            Rigidbody.angularVelocity = angularVel;

            // rotate rigidbody's velocity as well to generate immediate velocity redirection
            // manual velocity steering coefficient
            float velocitySteering = 25f;
            // rotate our velocity based on current steer value
            Rigidbody.velocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * Rigidbody.velocity;
        }

        // apply simplified lateral ground friction
        // only apply if we are on the ground at all
        if (wheelsOnGround && !IsDisabled)
        {
            // manual grip coefficient scalar
            float gripCoeff = 30f;
            // what direction is our lateral friction in?
            // it is the direction the wheels are turned, our forward
            Vector3 latFrictionDirection = Vector3.Cross(fwd, transform.up);
            // how fast are we currently moving in our friction direction?
            float latSpeed = Vector3.Dot(Rigidbody.velocity, latFrictionDirection);
            // apply the damping
            Vector3 latFrictionDampedVelocity = Rigidbody.velocity - latFrictionDirection * latSpeed * finalStats.Grip * gripCoeff * Time.deltaTime;

            // apply the damped velocity
            Rigidbody.velocity = latFrictionDampedVelocity;
        }
    }

    void ApplyAngularSuspension()
    {
        // simple suspension dampens x and z angular velocity while on the ground
        Vector3 suspendedX = transform.right;
        Vector3 suspendedZ = transform.forward;
        suspendedX.y *= 0f;
        suspendedZ.y *= 0f;
        var sX = Vector3.Dot(Rigidbody.angularVelocity, suspendedX) * suspendedX;
        var sZ = Vector3.Dot(Rigidbody.angularVelocity, suspendedZ) * suspendedZ;
        var sXZ = sX + sZ;
        var sCoeff = 10f;

        Vector3 suspensionRotation;
        float minimumSuspension = 0.5f;
        if (wheelsOnGround || finalStats.Suspension < minimumSuspension)
        {
            suspensionRotation = sXZ * finalStats.Suspension * sCoeff * Time.deltaTime;
        }
        else
        {
            suspensionRotation = sXZ * minimumSuspension * sCoeff * Time.deltaTime;
        }

        Vector3 suspendedAngular = Rigidbody.angularVelocity - suspensionRotation;

        // apply the adjusted angularvelocity
        Rigidbody.angularVelocity = suspendedAngular;
    }

    void GroundVehicle()
    {
        if (IsWheelsOnGround)
        {
            if (groundedHeight < MinHeightThreshold)
            {
                float diff = MinHeightThreshold - groundedHeight;
                transform.position += diff * transform.up;
            }
        }
    }

    void GroundAirbourne()
    {
        // while in the air, fall faster
        if (!wheelsOnGround)
        {
            Rigidbody.velocity += Physics.gravity * Time.deltaTime * finalStats.AddedGravity;
        }
    }

    protected void Brake(bool braking)
    {
        isBraking = braking;
        if (wheelsOnGround && isBraking)
        {
            m_rigidbody.drag = 5f;
            m_rigidbody.angularDrag = 0;
        }
        else
        {
            m_rigidbody.drag = DEF_DRAG;
            m_rigidbody.angularDrag = DEF_ANGULAR_DRAG;
        }
    }

    protected void CheckGroundStatus(bool ignoreWheels)
    {
        CheckFrontBackSensors();
        CheckBottomSensor();
        if (!wheelsOnGround && !ignoreWheels)
        {
            AllignToGround();
        }
        GroundVehicle();
        GroundAirbourne();
    }

    private void AllignToGround()
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); ;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.localPosition, Vector3.down, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.localPosition, Vector3.down * hit.distance, Color.yellow);
            SetSmoothRotation(Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
        }
        else
        {
            Debug.DrawRay(transform.localPosition, Vector3.down * 1000, Color.white);
        }
    }

    private void CheckBottomSensor()
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); ;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.down), out hit, MinHeightThreshold, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            wheelsOnGround = true;
            groundedHeight = Mathf.Min(hit.distance, float.MaxValue);
        }
        else
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * MinHeightThreshold, Color.white);
            wheelsOnGround = false;
        }
    }

    private void CheckFrontBackSensors()
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); ;

        RaycastHit hit;

        // Calculate ray length
        float length = Collider.size.z * 2f;

        // Check if vehicle is going forward or backwards
        Vector3 rayDirection = inputAccel > 0 ? Quaternion.Euler(35, 0, 0) * Vector3.forward : Quaternion.Euler(-35, 0, 0) * -Vector3.forward;

        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(rayDirection), out hit, length, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(rayDirection) * hit.distance, Color.yellow);
            // Need to determine which ray was hit
            Vector3 hitDir = inputAccel > 0 ? transform.forward : -transform.forward;
            float colAngle = Vector3.Angle(hit.normal, hitDir) - 90;
            if ((colAngle - transform.rotation.x) >= SLOPE_DIFF_IGNORE_ANGLE &&
                colAngle <= MAX_SLOPE_ANGLE)
            {
                // Only allign with normals from hit if the normal is under the max slope angle
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
        }
        else
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(rayDirection) * (length), Color.white);
        }
    }

    private void SetSmoothRotation(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(SMOOTH_ROT_SPEED, (Time.deltaTime * SMOOTH_ROT_SPEED)));
    }

    protected float Weight
    {
        set
        {
            if (m_rigidbody != null)
            {
                weight = value;
                m_rigidbody.mass = weight;
            }
        }
    }

    public Rigidbody Rigidbody
    {
        set
        {
            m_rigidbody = value;
            m_rigidbody.drag = DEF_DRAG;
            m_rigidbody.angularDrag = DEF_ANGULAR_DRAG;
        }
        get
        {
            return m_rigidbody;
        }
    }

    public bool IsWheelsOnGround
    {
        get { return wheelsOnGround; }
    }

    public float AirPercent { get; private set; }
    public float GroundPercent { get; private set; }
}