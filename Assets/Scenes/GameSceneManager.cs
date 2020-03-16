using CivilianManager;
using Heist;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{
    private const string NPC_PATH_TAG = "NPCPath";

    protected GameSceneManager() { }

    public IEnumerator SetUpGameScene() {
        // TODO: Create heist here
        Path npcPath = GameObject.FindGameObjectWithTag(NPC_PATH_TAG).GetComponent<Path>();
        yield return StartCoroutine(CivilianSpawner.Instance.SpawnVehilcesOnPath(npcPath, CivilianSpawnRate.NORMAL));
    }
}
