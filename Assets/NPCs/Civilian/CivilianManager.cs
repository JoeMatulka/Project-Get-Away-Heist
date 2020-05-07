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

    public class CivilianSpawn
    {
        public int destinationIndex;
        public Vector3 position;

        public CivilianSpawn() { }
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
                int amountToSpawn = 1; // (path.Waypoints.Length * (int)rate);
                CivilianSpawn[] spawns = GenerateSpawns(path, amountToSpawn);

                // Spawn civilians on spawn points
                foreach (CivilianSpawn spawn in spawns)
                {
                    GameObject gameObject = Instantiate(Resources.Load("NPCs/Civilian/Civilian_car"), spawn.position, Quaternion.identity) as GameObject;
                    Civilian civilian = gameObject.GetComponent<Civilian>();
                    civilian.Path = path;
                    civilian.CurrentDestinationIndex = spawn.destinationIndex;
                    yield return null;
                }
                yield return null;
            }
            yield return 0;
        }

        private CivilianSpawn[] GenerateSpawns(Path path, int amountToSpawn)
        {
            List<CivilianSpawn> spawns = new List<CivilianSpawn>();
            for (int i = 0; i < amountToSpawn; i++)
            {
                spawns.Add(GenerateSafeSpawnOnPath(path, spawns));
            }
            return spawns.ToArray();
        }

        private CivilianSpawn GenerateSafeSpawnOnPath(Path path, List<CivilianSpawn> spawns)
        {
            CivilianSpawn spawn = new CivilianSpawn();

            // Grab random index from the array for start point
            int first = Random.Range(0, path.Waypoints.Length);

            // Determine the ending point that the spawn will be between
            int second = first == path.Waypoints.Length - 1 ? 0 : first + 1;

            // Generate Spawn
            float pos = Random.Range(0.0f, 1.0f);
            spawn.position = Vector3.Lerp(path.Waypoints[first].Position, path.Waypoints[second].Position, pos);
            spawn.destinationIndex = path.reverse ? first : second;

            // Determine if the spawn is safe
            if (spawns.Count > 0)
            {
                bool safe = true;
                foreach (CivilianSpawn civilianSpawn in spawns)
                {
                    if (Vector3.Distance(civilianSpawn.position, spawn.position) <= SAFE_DIST_TO_SPAWN)
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
