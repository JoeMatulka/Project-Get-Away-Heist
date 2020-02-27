using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color rayColor = Color.cyan;
    private PathWayPoint[] waypoints;

    private const float WAYPOINT_DIST_THRESHOLD = 2.5f;
    private const int MAX_NUMBER_WAYPOINTS = 250;

    public bool reverse;
    void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        waypoints = GetComponentsInChildren<PathWayPoint>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 position = waypoints[i].Position;
            if (waypoints[i].ConnectedWayPoints != null)
            {
                Gizmos.color = Color.green;
                foreach (PathWayPoint wp in waypoints[i].ConnectedWayPoints)
                {
                    if (!wp.ConnectedWayPoints.Contains(waypoints[i]))
                    {
                        // Notify gizmo if connection is expected but not linked
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawLine(wp.Position, position);
                }
            }
            else
            {
                if (i > 0)
                {
                    Vector3 previous = waypoints[i - 1].Position;
                    Gizmos.DrawLine(previous, position);
                    if (waypoints[i].lastWayPoint)
                    {
                        Vector3 first = waypoints[0].Position;
                        Gizmos.DrawLine(position, first);
                    }
                }
            }
        }
    }

    public void AddWayPoint(Vector3 waypointLocation)
    {
        // Do not add if last waypoint is close to one being added
        PathWayPoint lastWaypoint;
        int children = transform.childCount;
        if (children > 0)
        {
            lastWaypoint = transform.GetChild(0).GetComponent<PathWayPoint>();
            if (Vector3.Distance(lastWaypoint.Position, waypointLocation) < WAYPOINT_DIST_THRESHOLD)
            {
                return;
            }
        }
        GameObject gameObject = new GameObject(PathWayPoint.WAYPOINT_GAMEOBJECT_NAME);
        gameObject.transform.position = waypointLocation;
        gameObject.AddComponent<PathWayPoint>();
        gameObject.transform.parent = transform;
        gameObject.transform.SetAsFirstSibling();
        if (children >= MAX_NUMBER_WAYPOINTS)
        {
            // Remove last child when maximum children are met
            Destroy(transform.GetChild(children).gameObject);
        }
    }

    public PathWayPoint[] Waypoints
    {
        get
        {
            return waypoints;
        }
    }
}
