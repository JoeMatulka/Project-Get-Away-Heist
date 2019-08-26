using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBoost : PlayerItem
{
    private const string NAME = "ROCKET BOOST";
    private const float COOLDOWN = 30;
    public RocketBoost() {
        name = NAME;
        cooldown = COOLDOWN;
    }
    protected override void Use()
    {
        throw new System.NotImplementedException();
    }
}
