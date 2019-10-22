using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWayPoint : MonoBehaviour
{
    // Dictates that when a vehicle gets to this way point they stop
    public bool stop;

    public bool lastWayPoint = false;

    void OnDrawGizmos()
    {
        Gizmos.color = stop ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, .25f);
    }
}
