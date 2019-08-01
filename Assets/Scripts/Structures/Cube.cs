using System.Collections;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Cube : Structure
    {
        public GameObject cubeScoutPrefab;

        public CubeData stats;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<CubeData>();
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void StartEffect()
        {
            StartCoroutine("SpawnScout");
        }

        IEnumerator SpawnScout()
        {
            while (true)
            {
                GameObject scout = Instantiate(cubeScoutPrefab, this.transform.position - new Vector3(0.0f, 0.25f, 0.0f), cubeScoutPrefab.transform.rotation, this.transform) as GameObject;
                CubeScout currScout = scout.GetComponent<CubeScout>();
                currScout.SetHome(this.q, this.r);
                currScout.SetCoords(this.q, this.r);
                currScout.SetPlayer(this.player);
                currScout.SetMoveRate(stats.currLevel.moveRate);
                currScout.SetMoves(stats.currLevel.numMoves);
                currScout.AddVisited(this.q, this.r);
                EventManager.RaiseOnCreateScout(scout);
                yield return new WaitForSeconds(stats.currLevel.spawnRate);
            }
        }

        public override void Upgrade()
        {
            stats.Upgrade();
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void Destroy()
        {
            Destroy(gameObject);
        }
    }
}