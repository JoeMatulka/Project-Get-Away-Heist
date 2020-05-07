using CivilianManager;
using System.Collections;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{
    public const string MONEY_TRUCK_PATH = "Money Truck Path";

    public const string TRAFFIC_PATH_TAG = "Traffic Path";


    protected GameSceneManager() { }

    public IEnumerator SetUpGameScene()
    {
        // TODO: Create heist here

        // Spawn traffic
        GameObject[] trafficGameObjects = GameObject.FindGameObjectsWithTag(TRAFFIC_PATH_TAG);
        Path[] trafficPaths = new Path[trafficGameObjects.Length];
        for (int i = 0; i < trafficGameObjects.Length; i++)
        {
            trafficPaths[i] = trafficGameObjects[i].GetComponent<Path>();
        }
        yield return StartCoroutine(CivilianSpawner.Instance.SpawnVehilcesOnPaths(trafficPaths, CivilianSpawnRate.NORMAL));
    }
}
