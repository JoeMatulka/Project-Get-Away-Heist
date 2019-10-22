using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color rayColor = Color.cyan;
    public PathWayPoint[] waypoints;

    void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        waypoints = GetComponentsInChildren<PathWayPoint>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 position = waypoints[i].transform.position;
            if (i > 0)
            {
                Vector3 previous = waypoints[i - 1].transform.position;
                Gizmos.DrawLine(previous, position);
                if (waypoints[i].lastWayPoint)
                {
                    Vector3 first = waypoints[0].transform.position;
                    Gizmos.DrawLine(position, first);
                }
            }
        }
    }
}
