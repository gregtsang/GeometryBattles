using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public float bombRate;
        public int bombStrength;
        public int bombRadius;
        int targetQ, targetR;
        bool bombard = false;

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
                    boardState.AddNode(curr[0], curr[1], this.player, bombStrength / (1 + curr[2]));
                    if (curr[2] < bombRadius)
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
                yield return new WaitForSeconds(bombRate);
            }
        }
    }
}