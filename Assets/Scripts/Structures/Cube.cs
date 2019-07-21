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
        }

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
                currScout.SetMoveRate(stats.currLevel.moveRate);
                currScout.SetMoves(stats.currLevel.numMoves);
                currScout.AddVisited(this.q, this.r);
                BoardEventManager.RaiseOnCreateScout(currScout);
                yield return new WaitForSeconds(stats.currLevel.spawnRate);
            }
        }

        public override int GetMaxHP()
        {
            return stats.currLevel.maxHP;
        }

        public override int GetHPRegen()
        {
            return stats.currLevel.regen;
        }

        public override void Upgrade()
        {
            stats.Upgrade();
            boardState.SetNodeHP(this.q, this.r, stats.currLevel.maxHP);
        }

        public override void Destroy()
        {
            Destroy(gameObject);
        }
    }
}