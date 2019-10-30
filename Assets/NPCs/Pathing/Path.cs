using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color rayColor = Color.cyan;
    private PathWayPoint[] waypoints;

    private const float WAYPOINT_DIST_THRESHOLD = .5f;

    public bool reverse;
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

    public void AddWayPoint(Vector3 waypointLocation)
    {
        // Do not add if last waypoint is close to one being added
        PathWayPoint lastWaypoint;
        if (transform.childCount > 0)
        {
            lastWaypoint = transform.GetChild(0).GetComponent<PathWayPoint>();
            if (Vector3.Distance(lastWaypoint.Position, waypointLocation) > WAYPOINT_DIST_THRESHOLD)
            {
                return;
            }
        }
        GameObject gameObject = new GameObject(PathWayPoint.WAYPOINT_GAMEOBJECT_NAME);
        gameObject.transform.position = waypointLocation;
        gameObject.AddComponent<PathWayPoint>();
        gameObject.transform.parent = transform;
        gameObject.transform.SetAsFirstSibling();
    }

    public PathWayPoint[] Waypoints
    {
        get
        {
            return waypoints;
        }
    }
}
