using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSensor : MonoBehaviour
{
    public float rayLength = 5f;

    private Vehicle vehicle;

    private GameObject sensorContact;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (vehicle != null)
        {
            CheckFrontSensor();
        }
    }

    private void CheckFrontSensor()
    {
        Vector3 position = transform.localPosition;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(position, direction, out hit, rayLength, layerMask))
        {
            Debug.DrawRay(position, direction * hit.distance, Color.red);
            sensorContact = hit.collider.gameObject;
        }
        else
        {
            Debug.DrawRay(position, direction * rayLength, Color.green);
            sensorContact = null;
        }
    }

    public Vehicle Vehicle
    {
        set
        {
            vehicle = value;
        }
    }

    public GameObject Contact
    {
        get
        {
            return sensorContact;
        }
    }
}
