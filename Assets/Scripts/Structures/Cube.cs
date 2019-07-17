using System.Collections;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public GameObject cubeScoutPrefab;

        public int cost = 20;
        public float spawnRate = 10.0f;

        void Start()
        {
            StartCoroutine("SpawnScout");
        }

        IEnumerator SpawnScout()
        {
            while (this.hp > 0)
            {
                Instantiate(cubeScoutPrefab, this.transform.position, Quaternion.identity, this.transform);
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}