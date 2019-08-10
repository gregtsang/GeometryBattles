using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using Photon.Pun;

namespace GeometryBattles.StructureManager
{
    public class ScoutManager : MonoBehaviour
    {
        public BoardState boardState;
        public StructureStore structureStore;
        public GameObject cubeScoutPrefab;
        public PhotonView photonView;

        Dictionary<Vector2Int, CubeScout> scoutPos = new Dictionary<Vector2Int, CubeScout>();
        HashSet<CubeScout> scouts = new HashSet<CubeScout>();

        public int moveUnownedWeight = 3;
        public int moveUnvisitedWeight = 3;
        public int moveAwayWeight = 10;

        public void SpawnScout(Cube cube, Color color)
        {
            float cubeY = cube.transform.position.y;
            Vector3 tilePos = boardState.GetNodeTile(cube.Q, cube.R).gameObject.transform.position;
            Vector3 pos = new Vector3(tilePos.x, cubeY - 0.25f, tilePos.z);
            GameObject scout = Instantiate(cubeScoutPrefab, pos, cubeScoutPrefab.transform.rotation, this.transform) as GameObject;
            scout.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color * 5);
            CubeScout currScout = scout.GetComponent<CubeScout>();
            currScout.SetHome(cube.Q, cube.R);
            currScout.SetCoords(cube.Q, cube.R);
            currScout.SetPlayer(cube.Player);
            currScout.SetMoveRate(cube.GetMoveRate());
            currScout.SetMoves(cube.GetNumMoves());
            currScout.AddVisited(cube.Q, cube.R);
            currScout.SetDamage(cube.GetDamage());
            currScout.SetInfluence(cube.GetInfluence());
            scouts.Add(currScout);
            scoutPos[new Vector2Int(currScout.Q, currScout.R)] = currScout;
            StartCoroutine(Drop(currScout, color));
        }

        IEnumerator Drop(CubeScout scout, Color color)
        {
            float timer = 0.8f;
            while (timer >= 0.0f)
            {
                timer -= Time.deltaTime;
                float scoutX = scout.gameObject.transform.position.x;
                float scoutY = scout.gameObject.transform.position.y;
                float scoutZ = scout.gameObject.transform.position.z;
                scout.gameObject.transform.position = new Vector3(scoutX, (scoutY - 0.25f) * Mathf.Max(0.0f, timer / 0.8f) + 0.25f, scoutZ);
                scout.gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(color * 5, color, 1.0f - Mathf.Max(0.0f, timer)));
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(Scout(scout));
            }
        }

        [PunRPC]
        void RPC_RemoveScout(int scoutQ, int scoutR, int q, int r)
        {
            RemoveScout(scoutPos[new Vector2Int(scoutQ, scoutR)], new Vector2Int(q, r));
        }

        void RemoveScout(CubeScout scout, Vector2Int coords)
        {
            scouts.Remove(scout);
            if (scout != null)
            {
                scout.SelfDestruct();
            }
            scoutPos.Remove(coords);
        }

        IEnumerator Scout(CubeScout scout)
        {
            while (scout != null)
            {
                Vector2Int curr = new Vector2Int(scout.Q, scout.R);
                Vector2Int next = NextTile(scout);
                if (next[0] == -1)
                {
                    photonView.RPC("RPC_RemoveScout", RpcTarget.AllViaServer, scout.Q, scout.R, curr[0], curr[1]);
                }
                else
                {
                    photonView.RPC("RPC_Jump", RpcTarget.Others, next[0], next[1], scout.Q, scout.R);
                    yield return StartCoroutine(Jump(next[0], next[1], scout));
                    if (scout != null)
                        photonView.RPC("RPC_ScoutEffect", RpcTarget.AllViaServer, next[0], next[1], scout.Q, scout.R);
                }
                yield return new WaitForSeconds(scout.GetMoveRate());
            }
        }

        Vector2Int NextTile(CubeScout scout)
        {
            List<Vector2Int> neighbors = boardState.GetNeighbors(scout.Q, scout.R);
            List<Vector2Int> valid = new List<Vector2Int>();
            foreach (var n in neighbors)
            {
                int totalWeight = 1;
                if (structureStore.HasStructure(n[0], n[1]))
                {
                    if (structureStore.GetStructure(n[0], n[1]).GetPlayer() == scout.GetPlayer())
                        totalWeight *= 0;
                    else
                        return new Vector2Int(n[0], n[1]);
                }
                else
                {
                    if (boardState.GetNodeOwner(n[0], n[1]) != scout.GetPlayer())
                        totalWeight *= moveUnownedWeight;
                    if (!scout.HasVisited(n[0], n[1]))
                        totalWeight *= moveUnvisitedWeight;
                    Vector2Int home = scout.GetHome();
                    if (CalcDistance(home[0], home[1], n[0], n[1]) > CalcDistance(home[0], home[1], scout.Q, scout.R))
                        totalWeight *= moveAwayWeight;
                }
                for (int i = 0; i < totalWeight; i++)
                    valid.Add(new Vector2Int(n[0], n[1]));
            }
            return valid.Count == 0 ? new Vector2Int(-1, -1) : valid[UnityEngine.Random.Range(0, valid.Count)];
        }

        [PunRPC]
        void RPC_Jump(int q, int r, int scoutQ, int scoutR)
        {
                StartCoroutine(Jump(q, r, scoutPos[new Vector2Int(scoutQ, scoutR)]));
        }

        IEnumerator Jump(int q, int r, CubeScout scout)
        {
            Vector3 currPos = scout.gameObject.transform.position;
            Vector3 newPos = boardState.GetNodeTile(q, r).gameObject.transform.position;
            newPos += new Vector3(0.0f, 0.25f - newPos.y, 0.0f);
            float changeX = newPos.x - currPos.x;
            float changeZ = newPos.z - currPos.z;
            float direction = changeX == 0.0f ? 0.0f : 30.0f;
            direction = (changeX > 0.0f && changeZ > 0.0f) || (changeX < 0.0f && changeZ < 0.0f) ? -direction : direction;
            int sign = (changeX > 0.0f) || (changeX == 0.0f && changeZ < 0.0f) ? -1 : 1;

            float distX = newPos.x - currPos.x;
            float distZ = newPos.z - currPos.z;
            
            Quaternion currRot = scout.gameObject.transform.rotation;
            float scoutRot = scout.GetRotation();
            scout.SetRotation(direction);
            Quaternion targetRot = Quaternion.AngleAxis(direction - scoutRot, Vector3.up);
            Quaternion newRot = targetRot * scout.gameObject.transform.rotation;
            
            Vector3 axis = direction == 0.0f ? Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.right : Quaternion.Euler(0.0f, direction, 0.0f) * Vector3.forward;
            Quaternion jumpRot = Quaternion.AngleAxis(sign * 90.0f, axis);
            Quaternion endRot = jumpRot * newRot;
            
            float rotRate = 0.1f;
            float rotTimer = rotRate;
            while (scout != null && scout.gameObject.transform.rotation != newRot)
            {
                rotTimer -= Time.deltaTime;
                scout.gameObject.transform.rotation = Quaternion.Slerp(currRot, newRot, 1.0f - Mathf.Max(rotTimer, 0.0f) / rotRate);
                yield return null;
            }

            float jumpRate = 0.35f;
            float jumpTimer = jumpRate;
            float jumpHeight = 5.0f;
            while (scout != null && scout.gameObject.transform.position != newPos)
            {
                jumpTimer -= Time.deltaTime;
                scout.gameObject.transform.rotation = Quaternion.Slerp(newRot, endRot, 1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate);
                scout.gameObject.transform.position = new Vector3(currPos.x + distX * (1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate),
                                                                  currPos.y + jumpHeight * (0.25f - Mathf.Pow((0.5f - Mathf.Max(jumpTimer, 0.0f) / jumpRate), 2.0f)),
                                                                  currPos.z + distZ * (1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate));
                yield return null;
            }
        }
        
        [PunRPC]
        void RPC_ScoutEffect(int q, int r, int scoutQ, int scoutR)
        {
            if (scoutPos[new Vector2Int(scoutQ, scoutR)] != null)
            {
                ScoutEffect(q, r, scoutPos[new Vector2Int(scoutQ, scoutR)]);
            }
        }

        void ScoutEffect(int q, int r, CubeScout scout)
        {
            Vector2Int curr = new Vector2Int(scout.Q, scout.R);
            Vector2Int next = new Vector2Int(q, r);
            if (structureStore.HasStructure(next[0], next[1]))
                boardState.AddNode(q, r, scout.GetPlayer(), scout.GetDamage());
            else
                boardState.AddNode(q, r, scout.GetPlayer(), scout.GetInfluence());
            if (scoutPos.ContainsKey(next))
            {
                RemoveScout(scout, curr);
                RemoveScout(scoutPos[next], next);
            }
            else if (structureStore.HasStructure(next[0], next[1]) && structureStore.GetStructure(next[0], next[1]).GetPlayer() != scout.GetPlayer())
            {
                RemoveScout(scout, curr);
            }
            else
            {
                scoutPos.Remove(curr);
                scout.SetCoords(next[0], next[1]);
                scoutPos[next] = scout;
            }
            scout.DecMoves();
            if (scout.GetMoves() <= 0)
            {
                RemoveScout(scout, next);
            }
        }

        int CalcDistance(int q1, int r1, int q2, int r2)
        {
            return (Mathf.Abs(q1 - q2) + Mathf.Abs(q1 + r1 - q2 - r2) + Mathf.Abs(r1 - r2)) / 2;
        }
    }
}