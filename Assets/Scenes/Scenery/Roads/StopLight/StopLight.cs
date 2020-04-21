using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLight : MonoBehaviour
{
    private const float GREEN_LIGHT_LENGTH = 20;
    private const string GO = "green";
    private const float YELLOW_LIGHT_LENGTH = 5f;
    private const string WARN = "yellow";
    private const float RED_LIGHT_LENGTH = 10f;
    private const string STOP = "red";

    private string lightColor;
    private bool lightSet;

    public PathWayPoint[] WayPoints;

    void Awake()
    {
        lightSet = false;
        switch (Random.Range(0, 3))
        {
            case 0:
                lightColor = GO;
                break;
            case 1:
                lightColor = WARN;
                break;
            case 2:
                lightColor = STOP;
                break;
            default:
                lightColor = GO;
                break;
        }
    }

    void Update()
    {
        if (!lightSet)
        {
            switch (lightColor)
            {
                case GO:
                    Go();
                    Invoke("ChangeLight", GREEN_LIGHT_LENGTH);
                    break;
                case WARN:
                    Warn();
                    Invoke("ChangeLight", YELLOW_LIGHT_LENGTH);
                    break;
                case STOP:
                    Stop();
                    Invoke("ChangeLight", RED_LIGHT_LENGTH);
                    break;
            }
        }
    }

    private void ChangeLight()
    {
        switch (lightColor)
        {
            case GO:
                lightColor = WARN;
                break;
            case WARN:
                lightColor = STOP;
                break;
            case STOP:
                lightColor = GO;
                break;
        }
        lightSet = false;
    }

    public void Go()
    {
        //TODO: Change this when there is a model
        var cubeRenderer = GetComponent<Renderer>();

        //Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", Color.green);

        lightColor = GO;
        lightSet = true;

        foreach (PathWayPoint wp in WayPoints) {
            wp.stop = false;
        }
    }

    public void Warn()
    {
        //TODO: Change this when there is a model
        var cubeRenderer = GetComponent<Renderer>();

        //Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", Color.yellow);

        lightColor = WARN;
        lightSet = true;
    }

    public void Stop()
    {
        //TODO: Change this when there is a model
        var cubeRenderer = GetComponent<Renderer>();

        //Call SetColor using the shader property name "_Color" and setting the color to red
        cubeRenderer.material.SetColor("_Color", Color.red);

        lightColor = STOP;
        lightSet = true;

        foreach (PathWayPoint wp in WayPoints)
        {
            wp.stop = true;
        }
    }
}
