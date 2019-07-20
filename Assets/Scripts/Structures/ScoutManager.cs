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

        public float moveRate = 2.0f;
        public int numMoves = 10;
        public int moveUnownedWeight = 3;
        public int moveUnvisitedWeight = 3;
        public int moveAwayWeight = 10;

        void Start()
        {
            ScoutEventManager.onCreate += AddScout;
        }

        void Update()
        {
            UpdateScouts();
        }

        void AddScout(CubeScout scout)
        {
            scouts.Add(scout);
            scoutPos[new Vector2Int(scout.Q, scout.R)] = scout;
        }

        void UpdateScouts()
        {
            destroy = new List<CubeScout>();
            foreach(var s in scouts)
            {
                if (s == null)
                    continue;
                s.SetTimer(s.GetTimer() - Time.deltaTime);
                if (s.GetTimer() <= 0.0f)
                {
                    Vector2Int curr = new Vector2Int(s.Q, s.R);
                    Vector2Int next = NextTile(s);

                    if (next[0] == -1)
                    {
                        destroy.Add(s);
                        s.SelfDestruct();
                        scoutPos.Remove(curr);
                    }
                    else
                    {
                        MoveScout(next[0], next[1], s);
                        boardState.AddNode(next[0], next[1], s.GetPlayer(), 100);
                        if (scoutPos.ContainsKey(next))
                        {
                            destroy.Add(s);
                            s.SelfDestruct();
                            scoutPos.Remove(curr);
                            destroy.Add(scoutPos[next]);
                            scoutPos[next].SelfDestruct();
                            scoutPos.Remove(next);
                        }
                        else if (structureStore.HasStructure(next[0], next[1]) && structureStore.GetStructure(next[0], next[1]).GetPlayer() != s.GetPlayer())
                        {
                            destroy.Add(s);
                            s.SelfDestruct();
                            scoutPos.Remove(curr);
                            // Damage Structure
                        }
                        else
                        {
                            scoutPos.Remove(curr);
                            s.SetCoords(next[0], next[1]);
                            scoutPos[next] = s;
                        }
                        s.SetTimer(moveRate);
                        s.DecMoves();
                        if (s.GetMoves() <= 0)
                        {
                            destroy.Add(s);
                            s.SelfDestruct();
                            scoutPos.Remove(next);
                        }
                    }
                }
            }
            foreach(var d in destroy)
                scouts.Remove(d);
        }

        void MoveScout(int q, int r, CubeScout scout)
        {
            if (q - scout.Q == 1 && r - scout.R == -1)
                scout.gameObject.transform.position += new Vector3(0, 0, boardState.tileWidth);
            else if (q - scout.Q == -1 && r - scout.R == 1)
                scout.gameObject.transform.position += new Vector3(0, 0, -boardState.tileWidth);
            else if (q - scout.Q == 1)
                scout.gameObject.transform.position += new Vector3(0.75f * boardState.tileLength, 0, boardState.tileWidth / 2.0f);
            else if (q - scout.Q == -1)
                scout.gameObject.transform.position += new Vector3(-0.75f * boardState.tileLength, 0, -boardState.tileWidth / 2.0f);
            else if (r - scout.R == 1)
                scout.gameObject.transform.position += new Vector3(0.75f * boardState.tileLength, 0, -boardState.tileWidth / 2.0f);
            else
                scout.gameObject.transform.position += new Vector3(-0.75f * boardState.tileLength, 0, boardState.tileWidth / 2.0f);
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