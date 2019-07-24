using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float m_accel, m_steering;

    void Update()
    {
        m_accel = Input.GetAxis("Vertical");
        m_steering = Input.GetAxis("Horizontal");
    }

    public float Acceleration {
        get {
            return m_accel;
        }
    }

    public float Steering {
        get {
            return m_steering;
        }
    }
}
