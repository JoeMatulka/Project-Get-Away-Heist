using UnityEngine;
using UnityEngine.Events;

/**
 * Handles the controls used for Player input
 */
public class PlayerControls : MonoBehaviour
{
    private float m_accel, m_steering;

    private bool m_isBraking;

    UnityEvent itemEvent = new UnityEvent();
    UnityEvent pauseEvent = new UnityEvent();

    public PlayerInput input;

    void Start()
    {
        // TODO fix this later
        input = GameObject.Find("Mobile Input Canvas").GetComponent<MobilePlayerInput>();
    }

    void Update()
    {
        if (!Application.isEditor)
        {
            m_accel = input.GetAcceleration();
            m_steering = input.GetSteering();
            m_isBraking = input.isBraking();


        }
        else
        {
            // TODO fix this later
            m_accel = Input.GetAxis("Vertical");
            m_steering = Input.GetAxis("Horizontal");
            m_isBraking = Input.GetKey(KeyCode.Space);
        }
    }

    public float Acceleration
    {
        get
        {
            return m_accel;
        }
    }

    public float Steering
    {
        get
        {
            return m_steering;
        }
    }

    public bool isBraking
    {
        get
        {
            return m_isBraking;
        }
    }

    public void ItemUsed() {

    }

    public void Pause() {

    }
}

