using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MobilePlayerInput : MonoBehaviour, PlayerInput
{
    public Joystick accelJoystick, steerJoystick;

    public Button itemButton;
    private UnityEvent itemEvent;

    private UnityEvent pauseEvent;

    private float accelInput, steerInput;

    void Start()
    {
        itemButton.onClick.AddListener(UseItem);
    }

    void Update()
    {
        accelInput = accelJoystick.Vertical;
        steerInput = steerJoystick.Horizontal;
    }

    public float GetAcceleration()
    {
        return accelInput;
    }

    public float GetSteering()
    {
        return steerInput;
    }

    public bool isBraking()
    {
        throw new System.NotImplementedException();
    }

    public void SetItemEvent(UnityEvent itemEvent)
    {
        this.itemEvent = itemEvent;
    }

    public void UseItem()
    {
        itemEvent.Invoke();
    }

    public void SetPauseEvent(UnityEvent pauseEvent)
    {
        this.pauseEvent = pauseEvent;
    }

    public void Pause()
    {
        pauseEvent.Invoke();
    }
}
