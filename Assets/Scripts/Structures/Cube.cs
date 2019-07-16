using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public int cost = 20;
        public float spawnRate = 1.0f;
        float spawnTimer = 1.0f;

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
            List<Vector2Int> neighbors = boardState.GetNeighbors(this.q, this.r);
        }
    }
}