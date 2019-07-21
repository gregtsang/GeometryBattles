using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public PentagonData stats;

        int targetQ, targetR;
        bool bombard = false;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PentagonData>();
        }

        public void SetTarget(int q, int r)
        {
            targetQ = q;
            targetR = r;
            if (!bombard)
                StartCoroutine(Bombard());
        }

        IEnumerator Bombard()
        {
            bombard = true;
            while (true)
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
                yield return new WaitForSeconds(stats.currLevel.bombRate);
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