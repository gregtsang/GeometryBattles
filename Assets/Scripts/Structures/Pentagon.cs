using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public PentagonData stats;

        int targetQ, targetR;
        bool target = false;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PentagonData>();
        }

        void Start()
        {
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void StartEffect()
        {
            //StartCoroutine(RegenHP());
            EventManager.RaiseOnCreatePentagon(gameObject);
        }

        public bool CheckSpace()
        {
            return this.CheckSpace(this.q, this.r, this.player);
        }

        public override bool CheckSpace(int q, int r, Player player)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            tiles.Add(new Vector2Int(q, r));
            tiles.Add(new Vector2Int(q - 1, r));
            tiles.Add(new Vector2Int(q + 1, r));
            tiles.Add(new Vector2Int(q, r - 1));
            tiles.Add(new Vector2Int(q, r + 1));
            tiles.Add(new Vector2Int(q - 1, r + 1));
            tiles.Add(new Vector2Int(q + 1, r - 1));
            foreach (Vector2Int t in tiles)
            {
                if (!boardState.IsValidTile(t[0], t[1]) || !boardState.IsOwned(t[0], t[1]) || boardState.GetNodeOwner(t[0], t[1]) != player)
                    return false;
            }
            return true;
        }

        public int GetBombRadius()
        {
            return stats.currLevel.bombRadius;
        }

        public float GetBombRate()
        {
            return stats.currLevel.bombRate;
        }

        public int GetBombStrength()
        {
            return stats.currLevel.bombStrength;
        }

        public Vector2Int GetTarget()
        {
            return new Vector2Int(targetQ, targetR);
        }

        public bool HasTarget()
        {
            return target;
        }

        public void SetTarget(int q, int r)
        {
            targetQ = q;
            targetR = r;
            target = true;
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