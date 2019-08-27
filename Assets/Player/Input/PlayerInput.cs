using UnityEngine.Events;

public interface PlayerInput 
{
    float GetAcceleration();
    float GetSteering();
    bool isBraking();
    void Pause();
    void UseItem();
    void SetPauseEvent(UnityEvent pauseEvent);
    void SetItemEvent(UnityEvent itemEvent);
}
