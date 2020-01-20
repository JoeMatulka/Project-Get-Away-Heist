using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(BoxCollider))]
public class Money : MonoBehaviour
{
    private int amount = 1000;

    private const float LIFETIME = 5;

    private ParticleSystem moneyParticles;

    private BoxCollider collider;


    void Awake()
    {
        
    }

    void Update()
    {
        
    }
}
