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
        public GameObject projectilePrefab;
        public float projectileSpeed = 1.0f;
        public float projectileHeight = 2.0f;
        Dictionary<Vector2Int, GameObject> projectiles = new Dictionary<Vector2Int, GameObject>();

        void OnEnable()
        {
            EventManager.onCreatePentagon += AddPentagon;
            EventManager.onProjectileCollision += ProjectileCollision;
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
                    if (pentagon.IsGlowing())
                    {
                        photonView.RPC("RPC_StopGlow", RpcTarget.AllViaServer, pentagon.Q, pentagon.R);
                    }
                    yield return null;
                }
                if (!pentagon.IsGlowing())
                {
                    photonView.RPC("RPC_StartGlow", RpcTarget.AllViaServer, pentagon.Q, pentagon.R);
                }
                yield return new WaitForSeconds(pentagon.GetBombRate());
                Vector2Int target = pentagon.GetTarget();
                pentagon.Reset();
                photonView.RPC("RPC_Fire", RpcTarget.Others, target[0], target[1], pentagon.Q, pentagon.R);
                yield return StartCoroutine(FireProjectile(target[0], target[1], pentagon));
            }
        }

        [PunRPC]
        void RPC_StartGlow(int pentagonQ, int pentagonR)
        {
            Pentagon pentagon = (Pentagon)(structureStore.GetStructure(pentagonQ, pentagonR));
            pentagon.StartGlow();
        }

        [PunRPC]
        void RPC_StopGlow(int pentagonQ, int pentagonR)
        {
            Pentagon pentagon = (Pentagon)(structureStore.GetStructure(pentagonQ, pentagonR));
            pentagon.StopGlow();
        }

        [PunRPC]
        void RPC_Bombard(int q, int r, int pentagonQ, int pentagonR)
        {
            Pentagon pentagon = (Pentagon)(structureStore.GetStructure(pentagonQ, pentagonR));
            Bombard(q, r, pentagon);
        }

        void Bombard(int q, int r, Pentagon pentagon)
        {
            Vector2Int target = new Vector2Int(q, r);
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

        [PunRPC]
        void RPC_Fire(int q, int r, int pentagonQ, int pentagonR)
        {
                StartCoroutine(FireProjectile(q, r, (Pentagon)(structureStore.GetStructure(pentagonQ, pentagonR))));
        }

        IEnumerator FireProjectile(int q, int r, Pentagon pentagon)
        {
            Vector3 pos = pentagon.transform.position;
            GameObject projectile = Instantiate(projectilePrefab, pos, Quaternion.identity, pentagon.transform) as GameObject;
            projectiles[new Vector2Int(pentagon.Q, pentagon.R)] = projectile;
            projectile.GetComponent<Projectile>().Q = pentagon.Q;
            projectile.GetComponent<Projectile>().R = pentagon.R;
            projectile.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", pentagon.Player.GetColor() * 3);
            Vector3 targetPos = boardState.GetNodeTile(q, r).gameObject.transform.position;
            targetPos -= new Vector3(0.0f, targetPos.y, 0.0f);
            float dist = Vector3.Distance(pos, targetPos);
            MeshCollider collider = projectile.GetComponent<MeshCollider>();
            float currLerp = 0.0f;
            while (projectile != null && currLerp < 1.0f)
            {
                currLerp += Time.deltaTime * projectileSpeed / Mathf.Pow(Mathf.Max(1.0f, dist), 0.8f);
                Vector3 lerp = Vector3.Lerp(pos, targetPos, Mathf.Min(1.0f, currLerp));
                projectile.transform.position = lerp + new Vector3(0.0f, -(projectileHeight * dist * Mathf.Pow(currLerp - 0.5f, 2.0f)) + (projectileHeight * dist) / 4.0f, 0.0f);
                yield return null;
            }
            if (projectile != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("RPC_Bombard", RpcTarget.AllViaServer, q, r, pentagon.Q, pentagon.R);
                }
                Destroy(projectile);
            }
        }

        void ProjectileCollision(GameObject projectile)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Projectile p = projectile.GetComponent<Projectile>();
                photonView.RPC("RPC_DestroyProjectile", RpcTarget.AllViaServer, p.Q, p.R);
            }
        }

        [PunRPC]
        void RPC_DestroyProjectile(int q, int r)
        {
            DestroyProjectile(projectiles[new Vector2Int(q, r)]);
        }
        
        void DestroyProjectile(GameObject projectile)
        {
            if (projectile != null)
            {
                Destroy(projectile);
            }
        }
    }
}