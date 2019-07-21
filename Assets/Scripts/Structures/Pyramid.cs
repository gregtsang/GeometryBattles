using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pyramid : Structure
    {
        PyramidData stats;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PyramidData>();
        }

        void Start()
        {
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public void Buff(int range, int strength)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(this.q, this.r));
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(q, r, 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                boardState.SetBuff(curr[0], curr[1], boardState.GetNodeOwner(this.q, this.r), strength);
                if (curr[2] + 1 < range)
                {
                    List<Vector2Int> neighbors = boardState.GetNeighbors(curr[0], curr[1]);
                    foreach (var n in neighbors)
                    {
                        if (!visited.Contains(n))
                        {
                            visited.Add(n);
                            queue.Enqueue(new Vector3Int(n[0], n[1], curr[2] + 1));
                        }
                    }
                }
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

        public void Upgrade()
        {
            stats.Upgrade();
            boardState.SetNodeHP(this.q, this.r, stats.currLevel.maxHP);
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public override void Destroy()
        {
            Buff(stats.currLevel.range, 0);
        }
    }
}