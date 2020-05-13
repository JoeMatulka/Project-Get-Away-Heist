using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSensor : MonoBehaviour
{
    public float rayLength = 5f;

    private int layerMask;

    private Vehicle vehicle;

    private GameObject sensorContact;

    public Vector3[] rayDirections;

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
        
        // Specify Ray Directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 diagRight = transform.TransformDirection(new Vector3(.5f, 0, 1));
        Vector3 diagLeft = transform.TransformDirection(new Vector3(-.5f, 0, 1));
        Vector3[] directions = new Vector3[] { forward, diagLeft, diagRight };
        rayDirections = directions;

        int layerMask = LayerMask.GetMask("Ignore Raycast");

        RaycastHit hit;

        foreach (Vector3 direction in directions) {
            if (Physics.Raycast(position, direction, out hit, rayLength, ~layerMask))
            {
                Debug.DrawRay(position, direction * hit.distance, Color.red);
                sensorContact = hit.collider.gameObject;
                break;
            }
            else
            {
                Debug.DrawRay(position, direction * rayLength, Color.green);
                sensorContact = null;
            }
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
