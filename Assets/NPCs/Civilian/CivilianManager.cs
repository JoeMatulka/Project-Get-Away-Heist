using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CivilianManager
{
    public enum CivilianSpawnRate
    {
        // Numbers associated are the multipliers for generating civilians
        NONE = 0,
        SPARSE = 1,
        NORMAL = 2,
        DENSE = 3,
    }
    public class CivilianSpawner : Singleton<CivilianSpawner>
    {
        private static int CIV_SPAWN_MOD = 2;

        public IEnumerator SpawnVehilcesOnPath(Path path, CivilianSpawnRate rate)
        {
            // Calculate the amount of civilians spawned
            int amountToSpawn = (path.Waypoints.Length * (int) rate) * CIV_SPAWN_MOD;
            int amountSpawned = 0;

            // Position Civilians across the path
            while (amountSpawned < amountToSpawn) {
                Instantiate(Resources.Load("NPCs/Civilian/Civilian_car"));
                amountSpawned++;
                yield return null;
            }
            yield return 0;
        }
    }
}
