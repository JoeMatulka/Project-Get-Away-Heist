public interface PlayerInput 
{
    float GetAcceleration();

    float GetSteering();

    bool isBraking();

    void Pause();

    void UseItem();
}
