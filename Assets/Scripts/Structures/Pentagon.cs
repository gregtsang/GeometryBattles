using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pentagon : Structure
    {
        public float bombRate = 2.0f;
        public int bombStrength = 20;
        public int bombSplashDivisor = 2;
        public int bombRadius = 1;
        int targetQ, targetR;
        bool bombard = false;

        public void setTarget(int q, int r)
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
                    boardState.AddNode(curr[0], curr[1], this.player, bombStrength / (curr[2] * bombSplashDivisor));
                    if (curr[2] < bombRadius)
                    {
                        List<Vector2Int> neighbors = boardState.GetNeighbors(curr[0], curr[1]);
                        foreach (var n in neighbors)
                        {
                            if (!visited.Contains(n))
                            {
                                visited.Add(n);
                                queue.Enqueue(new Vector3Int(curr[0], curr[1], curr[2] + 1));
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(bombRate);
            }
        }
    }
}