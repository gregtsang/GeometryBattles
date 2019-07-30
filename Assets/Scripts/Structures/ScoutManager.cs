using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class ScoutManager : MonoBehaviour
    {
        public BoardState boardState;
        public StructureStore structureStore;
        Dictionary<Vector2Int, CubeScout> scoutPos = new Dictionary<Vector2Int, CubeScout>();
        HashSet<CubeScout> scouts = new HashSet<CubeScout>();
        List<CubeScout> destroy;

        public int moveUnownedWeight = 3;
        public int moveUnvisitedWeight = 3;
        public int moveAwayWeight = 10;

        void Start()
        {
            boardState = GameObject.FindObjectOfType<BoardState>();
            EventManager.onCreateScout += AddScout;
        }

        void AddScout(GameObject scout)
        {
            CubeScout currScout = scout.GetComponent<CubeScout>();
            scouts.Add(currScout);
            scoutPos[new Vector2Int(currScout.Q, currScout.R)] = currScout;
            StartCoroutine(Scout(currScout));
        }

        IEnumerator Scout(CubeScout scout)
        {
            yield return new WaitForSeconds(scout.GetMoveRate());
            while (scout != null)
            {
                Vector2Int curr = new Vector2Int(scout.Q, scout.R);
                Vector2Int next = NextTile(scout);
                if (next[0] == -1)
                {
                    scouts.Remove(scout);
                    scout.SelfDestruct();
                    scoutPos.Remove(curr);
                }
                else
                {
                    scout.SetMotion(true);
                    MoveScout(next[0], next[1], scout);
                    while (scout.GetMotion()) yield return null;
                    ScoutEffect(next[0], next[1], scout);
                }
                yield return new WaitForSeconds(scout.GetMoveRate());
            }
        }

        void MoveScout(int q, int r, CubeScout scout)
        {
            Vector3 pos = scout.gameObject.transform.position;
            if (q - scout.Q == 1 && r - scout.R == -1)
                StartCoroutine(Jump(scout, pos + new Vector3(0, 0, boardState.GetTileWidth()), 0.0f));
            else if (q - scout.Q == -1 && r - scout.R == 1)
                StartCoroutine(Jump(scout, pos + new Vector3(0, 0, -boardState.GetTileWidth()), 0.0f, -1));
            else if (q - scout.Q == 1)
                StartCoroutine(Jump(scout, pos + new Vector3(0.75f * boardState.GetTileLength(), 0, boardState.GetTileWidth() / 2.0f), -30.0f, -1));
            else if (q - scout.Q == -1)
                StartCoroutine(Jump(scout, pos + new Vector3(-0.75f * boardState.GetTileLength(), 0, -boardState.GetTileWidth() / 2.0f), -30.0f));
            else if (r - scout.R == 1)
                StartCoroutine(Jump(scout, pos + new Vector3(0.75f * boardState.GetTileLength(), 0, -boardState.GetTileWidth() / 2.0f), 30.0f, -1));
            else
                StartCoroutine(Jump(scout, pos + new Vector3(-0.75f * boardState.GetTileLength(), 0, boardState.GetTileWidth() / 2.0f), 30.0f));
        }

        IEnumerator Jump(CubeScout scout, Vector3 newPos, float direction, int sign = 1)
        {
            Vector3 currPos = scout.gameObject.transform.position;
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
            
            float rotRate = 0.2f;
            float rotTimer = rotRate;
            while (scout.gameObject.transform.rotation != newRot)
            {
                rotTimer -= Time.deltaTime;
                scout.gameObject.transform.rotation = Quaternion.Slerp(currRot, newRot, 1.0f - Mathf.Max(rotTimer, 0.0f) / rotRate);
                yield return null;
            }
            
            float jumpRate = 0.5f;
            float jumpTimer = jumpRate;
            float jumpHeight = 5.0f;
            while (scout.gameObject.transform.position != newPos)
            {
                jumpTimer -= Time.deltaTime;
                scout.gameObject.transform.rotation = Quaternion.Slerp(newRot, endRot, 1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate);
                scout.gameObject.transform.position = new Vector3(currPos.x + distX * (1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate),
                                                                  currPos.y + jumpHeight * (0.25f - Mathf.Pow((0.5f - Mathf.Max(jumpTimer, 0.0f) / jumpRate), 2.0f)),
                                                                  currPos.z + distZ * (1.0f - Mathf.Max(jumpTimer, 0.0f) / jumpRate));
                yield return null;
            }
            scout.SetMotion(false);
        }
        
        void ScoutEffect(int q, int r, CubeScout scout)
        {
            Vector2Int curr = new Vector2Int(scout.Q, scout.R);
            Vector2Int next = new Vector2Int(q, r);
            boardState.AddNode(q, r, scout.GetPlayer(), boardState.infMax);
            if (scoutPos.ContainsKey(next))
            {
                scouts.Remove(scout);
                scout.SelfDestruct();
                scoutPos.Remove(curr);
                scouts.Remove(scoutPos[next]);
                scoutPos[next].SelfDestruct();
                scoutPos.Remove(next);
            }
            else if (structureStore.HasStructure(next[0], next[1]) && structureStore.GetStructure(next[0], next[1]).GetPlayer() != scout.GetPlayer())
            {
                scouts.Remove(scout);
                scout.SelfDestruct();
                scoutPos.Remove(curr);
            }
            else
            {
                scoutPos.Remove(curr);
                scout.SetCoords(next[0], next[1]);
                scoutPos[next] = scout;
            }
            scout.SetTimer(scout.GetMoveRate());
            scout.DecMoves();
            if (scout.GetMoves() <= 0)
            {
                scouts.Remove(scout);
                scout.SelfDestruct();
                scoutPos.Remove(next);
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
            return valid.Count == 0 ? new Vector2Int(-1, -1) : valid[Random.Range(0, valid.Count)];
        }

        int CalcDistance(int q1, int r1, int q2, int r2)
        {
            return (Mathf.Abs(q1 - q2) + Mathf.Abs(q1 + r1 - q2 - r2) + Mathf.Abs(r1 - r2)) / 2;
        }
    }
}