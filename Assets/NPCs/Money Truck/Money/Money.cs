using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    private float amount = 10000000f;

    private const float DEGRADE_INTERVAL = 1;
    private const float DEGRADE_AMOUNT = 1000000; 

    private const float LIFETIME = 5f;

    private ParticleSystem moneyParticles;

    private BoxCollider col;


    void Awake()
    {
        moneyParticles = GetComponentInChildren<ParticleSystem>();
        col = GetComponentInChildren<BoxCollider>();
        Destroy(gameObject, LIFETIME);
    }

    void Start() {
        InvokeRepeating("DegradeAmount", 0, DEGRADE_INTERVAL);
    }

    private void DegradeAmount() {
        if (amount >= 0)
        {
            amount -= DEGRADE_AMOUNT;
        }
        else {
            amount = 0;
        }
    }

    public float Amount {
        get {
            return amount;
        }
    }
}
