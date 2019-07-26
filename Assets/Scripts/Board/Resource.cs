using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class Resource : MonoBehaviour
    {
        public int resourceTilesPerSide;
        public int minDistance;
        
        public int startResource;
        public int miningAmount;
        public float miningRate;
        public int resourceTileAmount;
        
        HashSet<Vector2Int> resourceTiles = new HashSet<Vector2Int>();

        public void InitResourceTiles(int boardWidth, int baseOffset)
        {
            for (int i = 0; i < resourceTilesPerSide; i++)
            {   
                int qRand, rRand;
                do
                {
                    qRand = Random.Range(baseOffset, boardWidth - baseOffset);
                    rRand = Random.Range(baseOffset, boardWidth - baseOffset);
                } while (resourceTiles.Contains(new Vector2Int(qRand, rRand))
                        || CalcDistance(baseOffset, boardWidth - baseOffset - 1, qRand, rRand) < minDistance 
                        || CalcDistance(boardWidth - baseOffset - 1, baseOffset, qRand, rRand) < minDistance);
                resourceTiles.Add(new Vector2Int(qRand, rRand));
                resourceTiles.Add(new Vector2Int(boardWidth - 1 - qRand, boardWidth - 1 - rRand));
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

        int CalcDistance(int q1, int r1, int q2, int r2)
        {
            return (Mathf.Abs(q1 - q2) + Mathf.Abs(q1 + r1 - q2 - r2) + Mathf.Abs(r1 - r2)) / 2;
        }
    }
}