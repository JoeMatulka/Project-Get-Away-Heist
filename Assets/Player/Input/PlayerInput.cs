﻿using UnityEngine;

public interface PlayerInput 
{
    float getAcceleration();

    float getSteering();

    bool isBraking();
}
