using System.Collections;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public GameObject cubeScoutPrefab;

        public float spawnRate = 10.0f;

        void Start()
        {
            StartCoroutine("SpawnScout");
        }

        IEnumerator SpawnScout()
        {
            while (this.hp > 0)
            {
                GameObject scout = Instantiate(cubeScoutPrefab, this.transform.position, cubeScoutPrefab.transform.rotation, this.transform) as GameObject;
                CubeScout currScout = scout.GetComponent<CubeScout>();
                currScout.SetHome(this.q, this.r);
                currScout.SetCoords(this.q, this.r);
                currScout.SetPlayer(this.player);
                currScout.AddVisited(this.q, this.r);
                BoardEventManager.RaiseOnCreateScout(currScout);
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}