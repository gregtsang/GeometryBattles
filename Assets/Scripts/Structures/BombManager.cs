using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using Photon.Pun;

namespace GeometryBattles.StructureManager
{
    public class BombManager : MonoBehaviour
    {
        public BoardState boardState;
        public StructureStore structureStore;
        public ScoutManager scoutManager;
        public PhotonView photonView;

        void OnEnable()
        {
            EventManager.onCreatePentagon += AddPentagon;
        }

        public void AddPentagon(GameObject pentagon)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(Bombardment(pentagon.GetComponent<Pentagon>()));
            }
        }

        IEnumerator Bombardment(Pentagon pentagon)
        {
            while (pentagon != null)
            {
                while (!pentagon.HasTarget() || !pentagon.CheckSpace())
                {
                    yield return null;
                }
                photonView.RPC("RPC_Bombard", RpcTarget.AllViaServer, pentagon.Q, pentagon.R);
                yield return new WaitForSeconds(pentagon.GetBombRate());
            }
        }

        [PunRPC]
        void RPC_Bombard(int q, int r)
        {
            Pentagon pentagon = (Pentagon)(structureStore.GetStructure(q, r));
            Bombard(pentagon);
        }

        void Bombard(Pentagon pentagon)
        {
            Vector2Int target = pentagon.GetTarget();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(target);
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(target[0], target[1], 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                boardState.AddNode(curr[0], curr[1], pentagon.Player, pentagon.GetBombStrength() / (1 + curr[2]));
                if (curr[2] < pentagon.GetBombRadius())
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