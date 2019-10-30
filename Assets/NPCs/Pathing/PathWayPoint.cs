using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWayPoint : MonoBehaviour
{
    public static readonly string WAYPOINT_GAMEOBJECT_NAME = "waypoint";

    // Dictates that when a vehicle gets to this way point they stop
    public bool stop;

    public bool lastWayPoint = false;

    void OnDrawGizmos()
    {
        Gizmos.color = stop ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, .25f);
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }
}
