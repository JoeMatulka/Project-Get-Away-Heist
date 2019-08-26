using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private float weight;

    protected BoxCollider Collider;

    private const float MAX_SLOPE_ANGLE = 45f;

    public static float BASE_SPEED = 28f;

    protected float SPEED = BASE_SPEED;
    private const float MAX_VELOCITY = 20;
    private const float SPEED_THRESHOLD_TO_TURN = 2f;
    private float inputAccel = 0;
    public float CurrentSpeed = 0;

    private const float MAX_TURN_RADIUS_INPUT = 2000;
    private const float MIN_TURN_RADIUS_INPUT = 1500;
    private const float CORRECT_TURN_MOD = 2.5f;

    private bool wheelsOnGround = false;

    public float Grip = 70f;
    private const float MAX_GRIP = 100f;
    private float currentGrip;

    private const float DEF_DRAG = 1;
    private const float DEF_ANGULAR_DRAG = .05f;

    public float SlideSpeed;

    private bool isBraking = false;

    protected void Accelerate(float accel)
    {
        inputAccel = accel;
        CurrentSpeed = m_rigidbody.velocity.magnitude;
        if (wheelsOnGround && CurrentSpeed < MAX_VELOCITY)
        {
            m_rigidbody.AddForce(((inputAccel * SPEED) * transform.forward) * weight);

            SlideSpeed = Vector3.Dot(transform.right, m_rigidbody.velocity);

            currentGrip = Mathf.Lerp(MAX_GRIP, Grip, CurrentSpeed * .05f);
            // Apply Grip
            m_rigidbody.AddForce(transform.right * (-SlideSpeed * weight * currentGrip));
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

    protected void Turn(float turn)
    {
        if (wheelsOnGround && CurrentSpeed >= SPEED_THRESHOLD_TO_TURN)
        {
            float torque = turn;
            if (torque > 0)
            {
                torque = Mathf.Lerp(MIN_TURN_RADIUS_INPUT, MAX_TURN_RADIUS_INPUT, turn);
            }
            else if (torque < 0)
            {
                torque = Mathf.Lerp(-MIN_TURN_RADIUS_INPUT, -MAX_TURN_RADIUS_INPUT, -turn);
            }

            Vector3 currTorque = transform.InverseTransformDirection(m_rigidbody.angularVelocity);

            // Corrective turning
            if ((Mathf.Round(currTorque.y) > 0 && turn <= 0) ||
                (Mathf.Round(currTorque.y) < 0 && turn >= 0))
            {
                Brake(true);
                // Apply negative force against correct torque to straighten out car faster
                m_rigidbody.AddTorque(((currTorque.y * transform.up) * MIN_TURN_RADIUS_INPUT * CORRECT_TURN_MOD) * -1);
            }
            else
            {
                Brake(false);
            }

            // Additive turning
            if (Mathf.Round(currTorque.y) == 0 & Mathf.Abs(turn) > 0)
            {
                torque *= CORRECT_TURN_MOD;
            }

            m_rigidbody.AddTorque(torque * transform.up);
        }
    }

    protected void CheckGroundStatus()
    {
        CheckFrontBackSensors();
        CheckBottomSensor();
        if (!wheelsOnGround)
        {
            AllignToGround();
        }
    }

    private void AllignToGround()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * 1000, Color.white);
        }
    }

    private void CheckBottomSensor()
    {
        // Grounded length check
        float groundCheck = .75f;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.down), out hit, groundCheck, layerMask))
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            wheelsOnGround = true;
        }
        else
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(Vector3.down) * groundCheck, Color.white);
            wheelsOnGround = false;
        }
    }

    private void CheckFrontBackSensors()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        // Calculate ray length
        float length = Collider.size.z * 1.5f;

        // Check if vehicle is going forward or backwards
        Vector3 rayDirection = inputAccel > 0 ? Quaternion.Euler(25, 0, 0) * Vector3.forward : Quaternion.Euler(-25, 0, 0) * -Vector3.forward;

        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(rayDirection), out hit, length, layerMask))
        {
            Debug.DrawRay(transform.localPosition, transform.TransformDirection(rayDirection) * hit.distance, Color.yellow);
            // Need to determine which ray was hit
            Vector3 hitDir = inputAccel > 0 ? transform.forward : -transform.forward;
            if (Vector3.Angle(hit.normal, hitDir) - 90 <= MAX_SLOPE_ANGLE)
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
}