using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    private float amount = 10000000f;

    private const float LIFETIME = 5;

    private ParticleSystem moneyParticles;

    private BoxCollider col;


    void Awake()
    {
        moneyParticles = GetComponentInChildren<ParticleSystem>();
        col = GetComponentInChildren<BoxCollider>();
    }

    void Update()
    {
        
    }

    public float Amount {
        get {
            return amount;
        }
    }
}
