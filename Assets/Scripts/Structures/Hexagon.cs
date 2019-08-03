using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Hexagon : Structure
    {
        public int baseMaxHP;
        public int baseRegen;
        public int baseArmor;
        public float buildTime;
        float timer;

        void Start()
        {
            hp = baseMaxHP;
            maxhp = baseMaxHP;
            regen = baseRegen;
            armor = baseArmor;
            timer = buildTime;
        }

        void Update()
        {
            if (CheckSpace())
            {
                timer -= Time.deltaTime;
            }
            if (timer <= 0.0f)
            {
                EventManager.RaiseOnGameOver(player.gameObject);
            }
        }

        bool CheckSpace()
        {
            List<Vector2Int> neighbors = boardState.GetNeighbors(q, r);
            foreach (Vector2Int n in neighbors)
            {
                if (!boardState.IsOwned(n[0], n[1]) || boardState.GetNodeOwner(n[0], n[1]) != player)
                    return false;
            }
            return true;
        }

        public float GetProgress()
        {
            return 1.0f - Mathf.Max(0.0f, timer / buildTime);
        }
    }
}
