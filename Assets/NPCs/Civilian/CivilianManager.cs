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
        private const float SAFE_DIST_TO_SPAWN = 5f;

        public IEnumerator SpawnVehilcesOnPaths(Path[] paths, CivilianSpawnRate rate)
        {
            foreach (Path path in paths)
            {
                // Wait for Path to render, if needed
                yield return new WaitUntil(() => path.Waypoints != null);
                // Calculate the amount of civilians spawned
                int amountToSpawn = (path.Waypoints.Length * (int)rate);
                Vector3[] spawnPoints = GenerateSpawnPoints(path, amountToSpawn);

                // Spawn civilians on spawn points
                foreach (Vector3 spawnPoint in spawnPoints)
                {
                    GameObject gameObject = Instantiate(Resources.Load("NPCs/Civilian/Civilian_car"), spawnPoint, Quaternion.identity) as GameObject;
                    Civilian civilian = gameObject.GetComponent<Civilian>();
                    civilian.Path = path;
                    civilian.IsDisabled = true;
                    yield return null;
                }
                yield return null;
            }
            yield return 0;
        }

        private Vector3[] GenerateSpawnPoints(Path path, int amountToSpawn)
        {
            List<Vector3> spawns = new List<Vector3>();
            for (int i = 0; i < amountToSpawn; i++)
            {
                spawns.Add(GenerateSafeSpawnOnPath(path, spawns));
            }
            return spawns.ToArray();
        }

        private Vector3 GenerateSafeSpawnOnPath(Path path, List<Vector3> spawns)
        {
            // Grab random index from the array for start point
            int first = Random.Range(0, path.Waypoints.Length);

            // Determine the ending point that the spawn will be between
            int second = first == path.Waypoints.Length - 1 ? 0 : first + 1;

            // Generate Spawn
            float pos = Random.Range(0.0f, 1.0f);
            Vector3 spawn = Vector3.Lerp(path.Waypoints[first].Position, path.Waypoints[second].Position, pos);

            // Determine if the spawn is safe
            if (spawns.Count > 0)
            {
                bool safe = true;
                foreach (Vector3 otherSpawn in spawns)
                {
                    if (Vector3.Distance(otherSpawn, spawn) <= SAFE_DIST_TO_SPAWN)
                    {
                        safe = false;
                        break;
                    }
                }
                if (!safe)
                {
                    spawn = GenerateSafeSpawnOnPath(path, spawns);
                }
            }

            return spawn;
        }
    }
}
