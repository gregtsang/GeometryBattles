using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

namespace GeometryBattles.BoardManager
{
    public class Resource : MonoBehaviour
    {
        public int resourceTilesPerSide = 5;
        public int minDistance = 3;
        public int startResource = 0;
        public int startMiningAmount = 1;
        public int resourceTileAmount = 5;
        HashSet<Vector2Int> resourceTiles = new HashSet<Vector2Int>(); // Needs to be synced

        public void InitResourceTiles(int boardWidth, int baseOffset)
        {
            if (PhotonNetwork.IsMasterClient)
            {

               SerializableVector2Int[] resourceTileLocations = new SerializableVector2Int[resourceTilesPerSide * 2];

               for (int i = 0; i < resourceTilesPerSide; i++)
               {   
                   int qRand, rRand;
                   do
                   {
                       qRand = Random.Range(baseOffset, boardWidth - baseOffset);
                       rRand = Random.Range(baseOffset, boardWidth - baseOffset);
                   } while (resourceTiles.Contains(new Vector2Int(qRand, rRand))
                           || CalcDistance(baseOffset, boardWidth - baseOffset, qRand, rRand) < minDistance 
                           || CalcDistance(boardWidth - baseOffset, baseOffset, qRand, rRand) < minDistance);
                   resourceTiles.Add(new Vector2Int(qRand, rRand));
                   resourceTiles.Add(new Vector2Int(boardWidth - 1 - qRand, boardWidth - 1 - rRand));
                   resourceTileLocations[i] = new SerializableVector2Int(qRand, rRand);
                   resourceTileLocations[resourceTilesPerSide + i] = new SerializableVector2Int(boardWidth - 1 - qRand, boardWidth - 1 - rRand);
               }

               PhotonView pv = GetComponent<PhotonView>();
               pv.RPC("RPC_SetResourceTileLocation", RpcTarget.Others, resourceTileLocations);
            }
        }

        int CalcDistance(int q1, int r1, int q2, int r2)
        {
            return (Mathf.Abs(q1 - q2) + Mathf.Abs(q1 + r1 - q2 - r2) + Mathf.Abs(r1 - r2)) / 2;
        }

        public bool IsResourceTile(int q, int r)
        {
            return resourceTiles.Contains(new Vector2Int(q, r));
        }

        public HashSet<Vector2Int> GetResourceTiles()
        {
            return resourceTiles;
        }


        [PunRPC]
        private void RPC_SetResourceTileLocation(SerializableVector2Int[] resourceTileLocations)
        {
            foreach (var pos in resourceTileLocations)
            {
                resourceTiles.Add(pos.Vector2Int);
            }
        }
    }
}