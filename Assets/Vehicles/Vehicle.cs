using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private float weight;

    protected BoxCollider Collider;

    private const float MAX_SLOPE_ANGLE = 45f;

    public static float BASE_SPEED = 1000;

    protected float SPEED = BASE_SPEED;
    private const float MAX_VELOCITY = 25;
    private const float SPEED_THRESHOLD_TO_TURN = 2f;
    private float inputAccel = 0;
    public float currentSpeed = 0;

    private const float TURN_SPEED = 3.0f;

    private bool wheelsOnGround = false;

    public float Grip = 200f;
    private const float MAX_GRIP = 100f;
    public float currentGrip;

    public float slideSpeed;

    protected void Accelerate(float accel)
    {
        inputAccel = accel;
        if (wheelsOnGround && m_rigidbody.velocity.magnitude < MAX_VELOCITY)
        {
            m_rigidbody.AddForce((((inputAccel * SPEED) * transform.forward) * weight) * Time.deltaTime);
            Vector3.ClampMagnitude(m_rigidbody.velocity, MAX_VELOCITY);

            slideSpeed = Vector3.Dot(transform.right, m_rigidbody.velocity);

            currentGrip = Mathf.Lerp(MAX_GRIP, Grip, m_rigidbody.velocity.magnitude * .05f);
            // Apply Grip
            m_rigidbody.AddForce((transform.right * (-slideSpeed * weight * currentGrip)) * Time.deltaTime);

            currentSpeed = m_rigidbody.velocity.magnitude;
        }
    }

    protected void Brake()
    {

    }

    protected void Turn(float turn)
    {
        if (wheelsOnGround && m_rigidbody.velocity.magnitude > SPEED_THRESHOLD_TO_TURN)
        {
            m_rigidbody.AddTorque((((turn * transform.up) * weight) * TURN_SPEED) * Time.deltaTime);
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
        }
        get {
            return m_rigidbody;
        }
    }
}