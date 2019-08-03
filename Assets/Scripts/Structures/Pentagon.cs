using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public PentagonData stats;

        int targetQ, targetR;
        bool target = false;
        float bombTimer;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PentagonData>();
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        void Update()
        {
            if (target && CheckSpace())
            {
                bombTimer -= Time.deltaTime;
                if (bombTimer <= 0.0f)
                {
                    Bombard();
                    bombTimer = stats.currLevel.bombRate;
                }
            }
        }

        public override void StartEffect()
        {
            bombTimer = stats.currLevel.bombRate;
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

        public void SetTarget(int q, int r)
        {
            targetQ = q;
            targetR = r;
            target = true;
        }

        void Bombard()
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(targetQ, targetR));
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(targetQ, targetR, 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                boardState.AddNode(curr[0], curr[1], this.player, stats.currLevel.bombStrength / (1 + curr[2]));
                if (curr[2] < stats.currLevel.bombRadius)
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