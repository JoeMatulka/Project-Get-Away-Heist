using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Police : Vehicle
{
    private Player player;

    public float Speed;

    private Vector3 destination;

    private NPCSensor sensor;
    private const float SENSOR_LENGTH = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponentInChildren<BoxCollider>();

        sensor = gameObject.AddComponent<NPCSensor>();
        sensor.Vehicle = this;
        sensor.rayLength = SENSOR_LENGTH;

        Weight = 200;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            FindDestination();
        }
    }

    private void FindDestination() {
        // Iterate through current player trail to navigate to player
        foreach (PathWayPoint waypoint in player.PlayerTrailPath.Waypoints) {
            if (destination == null) {
                destination = waypoint.Position;
            }

            float curDist = Vector3.Distance(transform.position, destination);

            if (curDist > Vector3.Distance(transform.position, waypoint.Position)) {
                destination = waypoint.Position;
            }
        }
        // Check to see if player is closest destination
        Vector3 playerPos = player.transform.position;
        if (Vector3.Distance(transform.position, destination) >
            Vector3.Distance(transform.position, playerPos)) {
            destination = playerPos;
        }
    }
}
