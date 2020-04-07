using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWayPoint : MonoBehaviour
{
    public static readonly string WAYPOINT_GAMEOBJECT_NAME = "waypoint";

    // Dictates that when a vehicle gets to this way point they stop
    public bool stop;
    public float stopDistance;

    public bool lastWayPoint = false;

    // Connected waypoints to this waypoint, used for scripted drive pathing
    public List<PathWayPoint> ConnectedWayPoints;

    void Awake() { 
        
    }
    void OnDrawGizmos()
    {
        if (stopDistance == 0) {
            stopDistance = .25f;
        }
        Gizmos.color = stop ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, stopDistance);
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }
}
