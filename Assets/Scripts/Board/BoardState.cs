using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using Photon.Pun;
using System;

namespace GeometryBattles.BoardManager
{
    public class BoardState : MonoBehaviour, IPunObservable
    {
        public Resource resource;
        List<Player> players = new List<Player>();
        Dictionary<Player, Vector2Int> bases = new Dictionary<Player, Vector2Int>();

        public Color baseTileColor;
        float tileWidth = 1.73205f;
        float tileLength = 2.0f;
        public float tileGap;

        public int spreadAmount;
        public float spreadRate;
        public int fadeAmount;
        float spreadTimer = 0.0f;

        public int infMax;
        public int infThreshold;
        public int winPercent;

        public int GridCount { get => grid.Count; }

        public bool start = false;
        Dictionary<Vector2Int, TileState> grid;
        Dictionary<Vector2Int, TileState> buffer;

        private PhotonView photonView;

        void Update()
        {
            if (start)
            {
                Player winner = CheckEndGame();
                if (winner == null)
                {
                    spreadTimer -= Time.deltaTime;
                    UpdateColors();
                    if (spreadTimer <= 0.0f)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            photonView.RPC("RPC_CalcBuffer", RpcTarget.AllViaServer);
                        }
                        SetColors();
                        spreadTimer = spreadRate;
                    }
                }
                else
                {
                    EventManager.RaiseOnGameOver(winner.gameObject);
                    EndGame();
                }
            }
        }

        void OnEnable()
        {
            photonView = GetComponent<PhotonView>();
        }

        public void StartGame()
        {
            start = true;
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(MineResourceRepeat());
            }
        }

        public void EndGame()
        {
            start = false;
            if (PhotonNetwork.IsMasterClient)
            {
                StopCoroutine(MineResourceRepeat());
            }
        }

        public bool IsGameOver()
        {
            return !start;
        }

        public void SetGaps()
        {
            tileWidth += tileWidth * tileGap;
            tileLength += tileLength * tileGap;
        }

        public float GetTileWidth()
        {
            return tileWidth;
        }

        public float GetTileLength()
        {
            return tileLength;
        }

        public void SetCap(int n)
        {
            grid = new Dictionary<Vector2Int, TileState>(n * n);
            buffer = new Dictionary<Vector2Int, TileState>(n * n);
        }

        public Player GetPlayer(int i)
        {
            if (i < 0 || i >= players.Count) return null;
            return players[i];
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void InitNode(int q, int r, Tile node)
        {
            grid[new Vector2Int(q, r)] = new TileState(node);
            buffer[new Vector2Int(q, r)] = new TileState(node);
        }

        public bool ContainsNode(int q, int r)
        {
            return grid.ContainsKey(new Vector2Int(q, r));
        }

        public Tile GetNodeTile(int q, int r)
        {
            return grid[new Vector2Int(q, r)].GetTile();
        }

        public Player GetNodeOwner(int q, int r)
        {
            return grid[new Vector2Int(q, r)].GetOwner();
        }

        public int GetNodeInfluence(int q, int r)
        {
            return grid[new Vector2Int(q, r)].GetInfluence();
        }

        public void SetNode(int q, int r, Player owner, bool targetMain = true)
        {
            SetNode(q, r, owner, infThreshold, targetMain);
        }

        public void SetNode(int q, int r, Player owner, int influence, bool targetMain = true)
        {
            Dictionary<Vector2Int, TileState> gridbuffer = targetMain ? grid : buffer;
            Vector2Int coords = new Vector2Int(q, r);
            Player prevOwner = gridbuffer[coords].GetOwner();
            
            if (owner != prevOwner && prevOwner != null && prevOwner.OwnsTile(q, r))
            {
                prevOwner.RemoveTile(q, r);
            }
            if (influence >= infThreshold && !owner.OwnsTile(q, r))
            {
                owner.AddTile(q, r);
            }
            else if (influence < infThreshold && owner != null && owner.OwnsTile(q, r))
            {
                owner.RemoveTile(q, r);
            }

            gridbuffer[coords].Set(owner, influence);
            gridbuffer[coords].SetColor(owner, influence, infThreshold);
        }

        public void AddNode(int q, int r, Player player, int value, bool targetMain = true)
        {
            Dictionary<Vector2Int, TileState> gridbuffer = targetMain ? grid : buffer;
            Vector2Int coords = new Vector2Int(q, r);
            Player prevOwner = gridbuffer[coords].GetOwner();
            int prevInfluence = gridbuffer[coords].GetInfluence();

            if (gridbuffer[coords].HasStructure())
            {
                if (prevOwner != player)
                {
                    EventManager.RaiseOnStructureDamage(q, r, value);
                }
            }
            else
            {
                Player nextOwner;
                int nextInfluence;
                if (prevOwner == null || prevOwner == player)
                {
                    nextOwner = player;
                    nextInfluence = Mathf.Min(prevInfluence + value, infMax);
                }
                else
                {
                    if (prevInfluence < value)
                    {
                        nextOwner = player;
                        nextInfluence = Mathf.Min(value - prevInfluence, infMax);
                    }
                    else if (prevInfluence > value)
                    {
                        nextOwner = prevOwner;
                        nextInfluence = Mathf.Min(prevInfluence - value, infMax);
                    }
                    else
                    {
                        nextOwner = null;
                        nextInfluence = 0;
                    }
                }
                gridbuffer[coords].Set(nextOwner, nextInfluence);
                if (nextOwner != prevOwner && prevOwner != null && prevOwner.OwnsTile(q, r))
                {
                    prevOwner.RemoveTile(q, r);
                }
                if (nextInfluence >= infThreshold && !nextOwner.OwnsTile(q, r))
                {
                    nextOwner.AddTile(q, r);
                }
                else if (nextInfluence < infThreshold && nextOwner != null && nextOwner.OwnsTile(q, r))
                {
                    nextOwner.RemoveTile(q, r);
                }
                if (targetMain)
                {
                    gridbuffer[coords].SetColor(nextOwner, nextInfluence, infThreshold);
                }
            }
        }

        public void AddStructure(int q, int r)
        {
            Vector2Int coords = new Vector2Int(q, r);
            grid[coords].SetStructure(true);
            buffer[coords].SetStructure(true);
        }

        public void RemoveStructure(int q, int r)
        {
            Vector2Int coords = new Vector2Int(q, r);
            grid[coords].SetStructure(false);
            buffer[coords].SetStructure(false);
        }

        public void SetNodeBuff(int q, int r, Player player, int buff)
        {
            Vector2Int coords = new Vector2Int(q, r);
            grid[coords].SetBuff(player, buff);
            buffer[coords].SetBuff(player, buff);
        }

        public List<Vector2Int> GetBases()
        {
            List<Vector2Int> res = new List<Vector2Int>();
            foreach (Vector2Int b in bases.Values)
            {
                res.Add(b);
            }
            return res;
        }

        public void AddBase(int q, int r, Player player)
        {
            bases[player] = new Vector2Int(q, r);
        }

        public void RemoveBase(Player player)
        {
            bases.Remove(player);
            if (bases.Count == 1)
            {
                Player winner = null;
                foreach (Player p in bases.Keys)
                {
                    winner = p;
                }
                EventManager.RaiseOnGameOver(winner.gameObject);
                EndGame();
            }
        }

        public bool HasBase(Player player)
        {
            return bases.ContainsKey(player);
        }

        public bool IsConnectedToBase(int q, int r, Player player)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(q, r));
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            stack.Push(new Vector2Int(q, r));
            while (stack.Count > 0)
            {
                Vector2Int curr = stack.Pop();
                if (curr[0] == bases[player][0] && curr[1] == bases[player][1])
                {
                    return true;
                }
                else
                {
                    List<Vector2Int> neighbors = GetNeighbors(curr[0], curr[1]);
                    foreach (Vector2Int n in neighbors)
                    {
                        if (!visited.Contains(n) && grid[n].GetOwner() == player && grid[n].GetInfluence() >= infThreshold)
                        {
                            visited.Add(n);
                            stack.Push(n);
                        }
                    }
                }
            }
            return false;
        }

        public bool IsOwned(int q, int r)
        {
            return grid[new Vector2Int(q, r)].GetInfluence() >= infThreshold;
        }

        public int ClosestOwned(int q, int r, Player player)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(q, r));
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(q, r, 0));
            while (queue.Count > 0)
            {
                Vector3Int curr = queue.Dequeue();
                Vector2Int coords = new Vector2Int(curr[0], curr[1]);
                if (grid[coords].GetOwner() == player && grid[coords].GetInfluence() >= infThreshold)
                {
                    return curr[2];
                }
                else
                {
                    List<Vector2Int> neighbors = GetNeighbors(curr[0], curr[1]);
                    foreach (Vector2Int n in neighbors)
                    {
                        if (!visited.Contains(n))
                        {
                            visited.Add(n);
                            queue.Enqueue(new Vector3Int(n[0], n[1], curr[2] + 1));
                        }
                    }
                }
            }
            return -1;
        }

        public bool IsValidTile(int q, int r)
        {
            return grid.ContainsKey(new Vector2Int(q, r));
        }

        public List<Vector2Int> GetNeighbors(int q, int r)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            if (grid.ContainsKey(new Vector2Int(q - 1, r)))
                neighbors.Add(new Vector2Int(q - 1, r));
            if (grid.ContainsKey(new Vector2Int(q + 1, r)))
                neighbors.Add(new Vector2Int(q + 1, r));
            if (grid.ContainsKey(new Vector2Int(q, r - 1)))
                neighbors.Add(new Vector2Int(q, r - 1));
            if (grid.ContainsKey(new Vector2Int(q, r + 1)))
                neighbors.Add(new Vector2Int(q, r + 1));
            if (grid.ContainsKey(new Vector2Int(q - 1, r + 1)))
                neighbors.Add(new Vector2Int(q - 1, r + 1));
            if (grid.ContainsKey(new Vector2Int(q + 1, r - 1)))
                neighbors.Add(new Vector2Int(q + 1, r - 1));
            return neighbors;
        }

        void SwapBuffer()
        {
            Dictionary<Vector2Int, TileState> temp;
            temp = grid;
            grid = buffer;
            buffer = temp;
        }

        void CalcBuffer()
        {
            foreach (KeyValuePair<Vector2Int, TileState> tile in grid)
            {
                buffer[tile.Key].Set(tile.Value.GetOwner(), tile.Value.GetInfluence());
                if (tile.Value.GetOwner() != null && !HasBase(tile.Value.GetOwner()))
                {
                    AddNode(tile.Key[0], tile.Key[1], null, Mathf.Min(fadeAmount, tile.Value.GetInfluence()), false);
                }
                List<Vector2Int> neighbors = GetNeighbors(tile.Key[0], tile.Key[1]);
                foreach (Vector2Int n in neighbors)
                {
                    if (grid[n].GetInfluence() >= infThreshold && HasBase(grid[n].GetOwner()))
                    {
                        AddNode(tile.Key[0], tile.Key[1], grid[n].GetOwner(), spreadAmount + grid[n].GetBuff(grid[n].GetOwner()), false);
                    }
                }
            }
            SwapBuffer();
        }

        void SetColors()
        {
            foreach (TileState tile in grid.Values)
            {
                tile.SetColor(tile.GetOwner(), tile.GetInfluence(), infThreshold, false);
            }
        }

        void UpdateColors()
        {
            foreach (KeyValuePair<Vector2Int, TileState> tile in grid)
            {
                Tile currTile = tile.Value.GetTile();
                Color nextColor = currTile.GetNextColor();
                Color prevColor = currTile.GetPrevColor();
                Color currColor = currTile.GetMat().GetColor("_BaseColor");
                if (!resource.IsResourceTile(tile.Key[0], tile.Key[1]) && nextColor != currColor)
                {
                    Color lerpedColor;
                    lerpedColor = Color.Lerp(prevColor, nextColor, 1.0f - Mathf.Max(0.0f, spreadTimer / spreadRate));
                    currTile.GetMat().SetColor("_BaseColor", lerpedColor);
                }
                else if (resource.IsResourceTile(tile.Key[0], tile.Key[1]))
                {
                    if (tile.Value.GetInfluence() >= 100 && currColor == Color.white * 2)
                    {
                        currTile.GetMat().SetColor("_BaseColor", Color.Lerp(Color.white, tile.Value.GetOwner().GetColor(), 0.5f));
                    }
                    else if (tile.Value.GetInfluence() < 100 && currColor != Color.white * 2)
                    {
                        currTile.GetMat().SetColor("_BaseColor", Color.white * 2);
                    }
                }
            }
        }

        Player CheckEndGame()
        {
            foreach (Player p in players)
            {
                if ((float)p.GetNumTiles() / (float)grid.Count >= (float)winPercent / 100.0f)
                {
                    return p;
                }
            }
            return null;
        }

        IEnumerator MineResourceRepeat()
        {
            while (true)
            {
                photonView.RPC("RPC_MineResource", RpcTarget.AllViaServer);
                yield return new WaitForSeconds(resource.miningRate);
            }
        }

        [PunRPC]
        private void RPC_MineResource()
        {
            Dictionary<Player, int> playerMining = new Dictionary<Player, int>();
            foreach (Player p in players)
            {
                if (HasBase(p))
                {
                    playerMining[p] = resource.miningAmount;
                }
            }
            HashSet<Vector2Int> resourceTiles = resource.GetResourceTiles();
            foreach (Vector2Int r in resourceTiles)
            {
                Player owner = grid[r].GetOwner();
                int influence = grid[r].GetInfluence();
                if (influence >= infThreshold && HasBase(owner) && IsConnectedToBase(r[0], r[1], owner))
                {
                    playerMining[owner] += resource.resourceTileAmount;
                }
            }
            foreach (Player p in players)
            {
                if (HasBase(p))
                {
                    p.AddResource(playerMining[p]);
                }
            }
            EventManager.RaiseOnResourceUpdate();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // int q = 0;
            // int r = 0;
            // bool snakeRight = true;

            if (PhotonNetwork.IsMasterClient && stream.IsWriting)
            {
                // byte[] payload = CompressBoardState(ref q, ref r, ref snakeRight);
                // Debug.Log($"Sending payload of size {payload?.Length ?? 0} bytes");
                // stream.SendNext(payload);
                ;
            }
            else if (!PhotonNetwork.IsMasterClient && !stream.IsWriting)
            {
                // byte[] payload = (byte[]) stream.ReceiveNext();
                ;
            }
        }


        // Precondition: q and r must be valid positions in the grid
        // Broken w/ current usage of hash table
        // private byte[] CompressBoardState(ref int q, ref int r, ref bool snakeRight)
        // {
        //         /* First bit: 1xxx_xxxx -> snaking right
        //                       0xxx_xxxx -> snaking left

        //            Next 7 bits: q coordinate
        //            Next 8 bits: r coordinate (msb unused)
        //         */
        //     byte[] gridState = new byte[grideSize * 2];
        //     gridState[0]  = (byte) q;
        //     gridState[0] |= (byte) (snakeRight ? 0b_1000_0000 : 0);
        //     gridState[1]  = (byte) r;    
        //     int bytesUsed = 2;

        //     while (q != -1)
        //     {
        //         TileState startTS = GetTileState(ref q, ref r, ref snakeRight);
        //         TileState nextTS  = PeekNextTileState(q, r);
        //         byte      copies  = 1;

        //         while (CanContinuePacking(copies, startTS, nextTS))
        //         {
        //                 // Side-Effect: Updates q and r to next tile
        //             GetTileState(ref q, ref r, ref snakeRight);

        //                 // Get next tile w/out updating q and r
        //             nextTS = PeekNextTileState(q, r);
        //             ++copies;
        //         }

        //         gridState[bytesUsed++] = GetPlayerAndTilesByte(startTS, copies);
        //         gridState[bytesUsed++] = (byte) startTS.GetInfluence();
        //     }

        //     byte[] payload = new byte[bytesUsed];
        //     Array.ConstrainedCopy(gridState, 0, payload, 0, bytesUsed);
        //     return payload;
        // }

        // private bool
        // CanContinuePacking(byte copies, TileState startTS, TileState nextTS)
        // {
        //     return copies < 32
        //         && nextTS?.GetInfluence() == (byte) startTS.GetInfluence() 
        //         && nextTS?.GetOwner() == startTS.GetOwner();
        // }


        // private byte GetPlayerAndTilesByte(TileState startTS, byte copies)
        // {
        //     byte   result = 0;
        //     Player owner  = startTS.GetOwner();

        //     if (owner?.Id is null)  result |= 0b111_00000;
        //     else if (owner.Id == 0) result |= 0b000_00000;
        //     else if (owner.Id == 1) result |= 0b001_00000;
        //     else if (owner.Id == 2) result |= 0b010_00000;
        //     else if (owner.Id == 3) result |= 0b011_00000;
        //     else if (owner.Id == 4) result |= 0b100_00000;
        //     else if (owner.Id == 5) result |= 0b101_00000;

        //         // Right 5 bits are the # of contiguous tiles matching startTS
        //     result |= copies;

        //     return result;
        // }

        [PunRPC]
        private void RPC_CalcBuffer()
        {
                CalcBuffer();
        }
    }
}
