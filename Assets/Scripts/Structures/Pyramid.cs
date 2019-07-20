using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pyramid : Structure
    {
        private int range;
        private int strength;

        void Start()
        {
            Buff();
        }

        public void Buff()
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(this.q, this.r));
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(q, r, 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                boardState.SetBuff(curr[0], curr[1], boardState.GetNodeOwner(this.q, this.r), this.strength);
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
    }
}