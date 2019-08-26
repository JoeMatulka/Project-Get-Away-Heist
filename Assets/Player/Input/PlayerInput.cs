public interface PlayerInput 
{
    float getAcceleration();

    float getSteering();

    bool isBraking();

    void Pause();

    void UseItem();
}
