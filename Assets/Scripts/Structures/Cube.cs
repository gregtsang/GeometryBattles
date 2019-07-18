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
                GameObject scout = Instantiate(cubeScoutPrefab, this.transform.position, Quaternion.identity, this.transform) as GameObject;
                CubeScout currScout = scout.GetComponent<CubeScout>();
                currScout.SetCoords(this.q, this.r);
                currScout.SetPlayer(this.player);
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}