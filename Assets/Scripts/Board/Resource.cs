using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class Resource : MonoBehaviour
    {
        public int resourceTilesPerPlayer;
        public int minDistance;
        
        public int startResource;
        public int miningAmount;
        public float miningRate;
        public int resourceTileAmount;
        
        HashSet<Vector2Int> resourceTiles = new HashSet<Vector2Int>();

        public void InitResourceTiles(List<Vector2Int> bases, int baseOffset, int boardWidth, int numPlayers)
        {
            int min = baseOffset;
            int max = numPlayers == 2 ? boardWidth - baseOffset : 2 * boardWidth - 1 - baseOffset;
            for (int i = 0; i < resourceTilesPerPlayer; i++)
            {   
                int qRand, rRand;
                do
                {
                    qRand = Random.Range(min, max);
                    rRand = Random.Range(min, max);
                } while (resourceTiles.Contains(new Vector2Int(qRand, rRand)) || !IsValidResource(qRand, rRand, bases));
                Vector2Int curr = new Vector2Int(qRand, rRand);
                Vector2Int center = new Vector2Int(boardWidth - 1, boardWidth - 1);
                resourceTiles.Add(curr);
                if (numPlayers == 2)
                {
                    resourceTiles.Add(new Vector2Int(boardWidth - 1 - qRand, boardWidth - 1 - rRand));
                }
                else if (numPlayers == 3)
                {
                    for (int j = 1; j < numPlayers; j++)
                    {
                        curr = Rotate120(curr, center);
                        resourceTiles.Add(curr);
                    }
                }
                else if (numPlayers == 6)
                {
                    for (int j = 1; j < numPlayers; j++)
                    {
                        curr = Rotate60(curr, center);
                        resourceTiles.Add(curr);
                    }
                }
            }
        }

        public HashSet<Vector2Int> GetResourceTiles()
        {
            return resourceTiles;
        }

        public bool IsResourceTile(int q, int r)
        {
            return resourceTiles.Contains(new Vector2Int(q, r));
        }

        public bool IsValidResource(int q, int r, List<Vector2Int> bases)
        {
            foreach (Vector2Int b in bases)
            {
                if (CalcDistance(b[0], b[1], q, r) < minDistance)
                {
                    return false;
                }
            }
            return true;
        }

        int CalcDistance(int q1, int r1, int q2, int r2)
        {
            return (Mathf.Abs(q1 - q2) + Mathf.Abs(q1 + r1 - q2 - r2) + Mathf.Abs(r1 - r2)) / 2;
        }

        Vector2Int Rotate60(Vector2Int curr, Vector2Int center)
        {
            Vector3Int temp = new Vector3Int(curr[0] - center[0], curr[1] - center[1], -curr[0] - curr[1] + center[0] + center[1]);
            Vector2Int res = new Vector2Int(-temp[1], -temp[2]);
            res += center;
            return res;
        }

        Vector2Int Rotate120(Vector2Int curr, Vector2Int center)
        {
            Vector3Int temp = new Vector3Int(curr[0] - center[0], curr[1] - center[1], -curr[0] - curr[1] + center[0] + center[1]);
            Vector2Int res = new Vector2Int(temp[2], temp[0]);
            res += center;
            return res;
        }
    }
}