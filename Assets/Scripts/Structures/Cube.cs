using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public CubeScout cubeScout;

        public int cost = 20;
        public float spawnRate = 10.0f;
        float spawnTimer = 0.0f;

        void Update()
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0.0f)
            {
                SpawnScout();
                spawnTimer = spawnRate;
            }
        }

        public void SpawnScout()
        {
            Instantiate(cubeScout, this.transform.position, Quaternion.identity, this.transform);
        }
    }
}