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
        float spreadTimer = 0.0f;

        public int infMax;
        public int infThreshold;

        int cap = -1;
        List<List<TileState>> grid;
        List<List<TileState>> buffer;
        private int grideSize;

        void Start()
        {
            StartCoroutine(MineResource());
            calculateGridSize();
        }

        private void calculateGridSize()
        {
            int count = 0;

            foreach (var row in grid) foreach (var tile in row)
                ++count;

            grideSize = count;
        }

            // Get tile @ (q, r) and prepare q and r for next call. q and r will be
            // set to -1, -1 if it's reached the end
        private TileState GetTileState(ref int q, ref int r, ref bool snakeRight)
        {
            TileState result = grid[q][r];

                // Move next column over
            r += (snakeRight ? 1 : -1);

                // If our next column is now out of bounds…
            if (grid[q].Count == r || r == -1)
            {
                    // Move down a row
                q += 1;

                    // If our row moved out of bounds
                if (grid.Count == q)
                {
                    r = -1;
                    q = -1;
                }
                    // We moved down a row successfully
                else
                {
                    snakeRight = !snakeRight;
                    r = snakeRight ? 0 : grid[q].Count - 1;
                }
            }

            return result;
        }

        private TileState PeekNextTileState(int q, int r)
        {
            if (q == -1)
            {
                return null;
            }
            else 
            {
                return grid[q][r];
            }
        }

      void Update()
        {
            spreadTimer -= Time.deltaTime;
            UpdateColors();
            SetColors();
            if (spreadTimer <= 0.0f)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    CalcBuffer();
                }
                spreadTimer = spreadRate;
            }
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
            grid = new List<List<TileState>>(n);
            buffer = new List<List<TileState>>(n);
            for (int i = 0; i < n; i++)
            {
                grid.Add(new List<TileState>(n));
                buffer.Add(new List<TileState>(n));
                for (int j = 0; j < n; j++)
                {
                    grid[i].Add(new TileState(null, null, 0));
                    buffer[i].Add(new TileState(null, null, 0));
                }
            }
            cap = n;
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

        public void InitNode(Tile node, int q, int r)
        {
            if (cap > 0 && q < cap && r < cap)
            {
                grid[q][r].SetTile(node);
                buffer[q][r].SetTile(node);
            }
        }

        public Tile GetNodeTile(int q, int r)
        {
            return grid[q][r].GetTile();
        }

        public Player GetNodeOwner(int q, int r)
        {
            return grid[q][r].GetOwner();
        }

        public int GetNodeInfluence(int q, int r)
        {
            return grid[q][r].GetInfluence();
        }

        public int GetNodeHP(int q, int r)
        {
            return grid[q][r].GetStructureHP();
        }

        public void SetNode(int q, int r, Player owner, bool target = true)
        {
            List<List<TileState>> gridbuffer = target ? grid : buffer;
            Player prevOwner = gridbuffer[q][r].GetOwner();
            int prevInfluence = gridbuffer[q][r].GetInfluence();
            
            gridbuffer[q][r].Set(owner, infThreshold);
            gridbuffer[q][r].SetColor(owner, infThreshold, infThreshold, baseTileColor);
        }

        public void SetNode(int q, int r, Player owner, int influence, bool target = true)
        {
            List<List<TileState>> gridbuffer = target ? grid : buffer;
            Player prevOwner = gridbuffer[q][r].GetOwner();
            int prevInfluence = gridbuffer[q][r].GetInfluence();
            
            gridbuffer[q][r].Set(owner, influence);
            gridbuffer[q][r].SetColor(owner, influence, infThreshold, baseTileColor);
        }

        public void AddNode(int q, int r, Player player, int value, bool target = true)
        {
            List<List<TileState>> gridbuffer = target ? grid : buffer;
            Player owner = gridbuffer[q][r].GetOwner();
            int influence = gridbuffer[q][r].GetInfluence();
            if (gridbuffer[q][r].GetStructureHP() > 0)
            {
                if (owner != player)
                    gridbuffer[q][r].SubStructureHP(value);
            }
            else
            {
                if (owner == null || owner == player)
                {
                    gridbuffer[q][r].Set(player, Mathf.Min(influence + value, infMax));
                    if (target)
                        gridbuffer[q][r].SetColor(player, Mathf.Min(influence + value, infMax), infThreshold, baseTileColor);
                }
                else
                {   
                    if (influence < value)
                    {
                        gridbuffer[q][r].Set(player, Mathf.Min(value - influence, infMax));
                        if (target)
                            gridbuffer[q][r].SetColor(player, Mathf.Min(value - influence, infMax), infThreshold, baseTileColor);
                    }
                    else if (influence > value)
                    {
                        gridbuffer[q][r].Set(owner, Mathf.Min(influence - value, infMax));
                        if (target)
                            gridbuffer[q][r].SetColor(owner, Mathf.Min(influence - value, infMax), infThreshold, baseTileColor);
                    }
                    else
                    {
                        gridbuffer[q][r].Set(null, 0);
                        if (target)
                            gridbuffer[q][r].SetColor(null, 0, infThreshold, baseTileColor);
                    }
                }
            }
        }

        public void SetNodeHP(int q, int r, int amount)
        {
            grid[q][r].SetStructureHP(amount);
            buffer[q][r].SetStructureHP(amount);
        }

        public void AddNodeHP(int q, int r, int amount, int max)
        {
            grid[q][r].AddStructureHP(amount, max);
            buffer[q][r].AddStructureHP(amount, max);
        }

        public void SetBuff(int q, int r, Player player, int buff)
        {
            grid[q][r].SetBuff(player, buff);
            buffer[q][r].SetBuff(player, buff);
        }

        public void AddBase(int q, int r, Player player)
        {
            bases[player] = new Vector2Int(q, r);
        }

        public bool IsConnectedToBase(int q, int r, Player player)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            visited.Add(new Vector2Int(q, r));
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(q, r));
            while (queue.Count > 0)
            {
                Vector2Int curr = queue.Dequeue();
                if (curr[0] == bases[player][0] && curr[1] == bases[player][1])
                    return true;
                else
                {
                    List<Vector2Int> neighbors = GetNeighbors(curr[0], curr[1]);
                    foreach (var n in neighbors)
                    {
                        if (!visited.Contains(n) && grid[n[0]][n[1]].GetOwner() == player && grid[n[0]][n[1]].GetInfluence() >= infThreshold)
                        {
                            visited.Add(n);
                            queue.Enqueue(new Vector2Int(n[0], n[1]));
                        }
                    }
                }
            }
            return false;
        }

        public bool IsOwned(int q, int r)
        {
            return grid[q][r].GetInfluence() >= infThreshold;
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
                if (grid[curr[0]][curr[1]].GetOwner() == player && grid[curr[0]][curr[1]].GetInfluence() >= infThreshold)
                    return curr[2];
                else
                {
                    List<Vector2Int> neighbors = GetNeighbors(curr[0], curr[1]);
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
            return -1;
        }

        public List<Vector2Int> GetNeighbors(int q, int r)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            bool qMin = q == 0, qMax = q == cap - 1, rMin = r == 0, rMax = r == cap - 1;
            if (!qMin)
                neighbors.Add(new Vector2Int(q - 1, r));
            if (!qMax)
                neighbors.Add(new Vector2Int(q + 1, r));
            if (!rMin)
                neighbors.Add(new Vector2Int(q, r - 1));
            if (!rMax)
                neighbors.Add(new Vector2Int(q, r + 1));
            if (!qMin && !rMax)
                neighbors.Add(new Vector2Int(q - 1, r + 1));
            if (!qMax && !rMin)
                neighbors.Add(new Vector2Int(q + 1, r - 1));
            return neighbors;
        }

        void SwapBuffer()
        {
            List<List<TileState>> temp;
            temp = grid;
            grid = buffer;
            buffer = temp;
        }

        void CalcBuffer()
        {            
            for (int i = 0; i < cap; i++)
            {
                for (int j = 0; j < cap; j++)
                {
                    buffer[i][j].Set(grid[i][j].GetOwner(), grid[i][j].GetInfluence());
                    buffer[i][j].SetStructureHP(grid[i][j].GetStructureHP());
                    List<Vector2Int> neighbors = GetNeighbors(i, j);
                    foreach (var n in neighbors)
                        if (grid[n[0]][n[1]].GetInfluence() >= infThreshold)
                            AddNode(i, j, grid[n[0]][n[1]].GetOwner(), spreadAmount + grid[n[0]][n[1]].GetBuff(grid[n[0]][n[1]].GetOwner()), false);
                }
            }
            SwapBuffer();
        }

        void SetColors()
        {
            for (int i = 0; i < cap; i++)
            {
                for (int j = 0; j < cap; j++)
                {
                    grid[i][j].SetColor(grid[i][j].GetOwner(), grid[i][j].GetInfluence(), infThreshold, baseTileColor, false);
                }
            }
        }

        void UpdateColors()
        {
            for (int i = 0; i < cap; i++)
            {
                for (int j = 0; j < cap; j++)
                {
                    Tile currTile = grid[i][j].GetTile();
                    Color nextColor = currTile.GetNextColor();
                    Color prevColor = currTile.GetPrevColor();
                    Color currColor = currTile.GetMat().GetColor("_BaseColor");
                    if (!resource.IsResourceTile(i, j) && nextColor != currColor)
                    {
                        Color lerpedColor;
                        lerpedColor = Color.Lerp(prevColor, nextColor, 1.0f - Mathf.Max(0.0f, spreadTimer / spreadRate));
                        currTile.GetMat().SetColor("_BaseColor", lerpedColor);
                    }
                }
            }
        }

        IEnumerator MineResource()
        {
            while (true)
            {
                Dictionary<Player, int> playerMining = new Dictionary<Player, int>();
                foreach (var p in players)
                    playerMining[p] = resource.miningAmount;
                HashSet<Vector2Int> resourceTiles = resource.GetResourceTiles();
                foreach (var r in resourceTiles)
                {
                    Player owner = grid[r[0]][r[1]].GetOwner();
                    int influence = grid[r[0]][r[1]].GetInfluence();
                    if (influence >= infThreshold && IsConnectedToBase(r[0], r[1], owner))
                        playerMining[owner] += resource.resourceTileAmount;
                }
                foreach (var p in players)
                    p.AddResource(playerMining[p]);

                EventManager.RaiseOnResourceUpdate();
                yield return new WaitForSeconds(resource.miningRate);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            int q = 0;
            int r = 0;
            bool snakeRight = true;

            if (PhotonNetwork.IsMasterClient && stream.IsWriting)
            {
                byte[] payload = CompressBoardState(ref q, ref r, ref snakeRight);
                Debug.Log($"Sending payload of size {payload?.Length ?? 0} bytes");
                stream.SendNext(payload);
            }
            else if (!PhotonNetwork.IsMasterClient && !stream.IsWriting)
            {
                byte[] payload = (byte[]) stream.ReceiveNext();
            }
        }


        // Precondition: q and r must be valid positions in the grid
        private byte[] CompressBoardState(ref int q, ref int r, ref bool snakeRight)
        {
                /* First bit: 1xxx_xxxx -> snaking right
                              0xxx_xxxx -> snaking left

                   Next 7 bits: q coordinate
                   Next 8 bits: r coordinate (msb unused)
                */
            byte[] gridState = new byte[grideSize * 2];
            gridState[0]  = (byte) q;
            gridState[0] |= (byte) (snakeRight ? 0b_1000_0000 : 0);
            gridState[1]  = (byte) r;    
            int bytesUsed = 2;

            while (q != -1)
            {
                TileState startTS = GetTileState(ref q, ref r, ref snakeRight);
                TileState nextTS  = PeekNextTileState(q, r);
                byte      copies  = 1;

                while (CanContinuePacking(copies, startTS, nextTS))
                {
                        // Side-Effect: Updates q and r to next tile
                    GetTileState(ref q, ref r, ref snakeRight);

                        // Get next tile w/out updating q and r
                    nextTS = PeekNextTileState(q, r);
                    ++copies;
                }

                gridState[bytesUsed++] = GetPlayerAndTilesByte(startTS, copies);
                gridState[bytesUsed++] = (byte) startTS.GetInfluence();
            }

            byte[] payload = new byte[bytesUsed];
            Array.ConstrainedCopy(gridState, 0, payload, 0, bytesUsed);
            return payload;
        }

        private bool
        CanContinuePacking(byte copies, TileState startTS, TileState nextTS)
        {
            return copies < 32
                && nextTS?.GetInfluence() == (byte) startTS.GetInfluence() 
                && nextTS?.GetOwner() == startTS.GetOwner();
        }


        private byte GetPlayerAndTilesByte(TileState startTS, byte copies)
        {
            byte   result = 0;
            Player owner  = startTS.GetOwner();

            if (owner?.Id is null)  result |= 0b111_00000;
            else if (owner.Id == 0) result |= 0b000_00000;
            else if (owner.Id == 1) result |= 0b001_00000;
            else if (owner.Id == 2) result |= 0b010_00000;
            else if (owner.Id == 3) result |= 0b011_00000;
            else if (owner.Id == 4) result |= 0b100_00000;
            else if (owner.Id == 5) result |= 0b101_00000;

                // Right 5 bits are the # of contiguous tiles matching startTS
            result |= copies;

            return result;
        }
        
    }
}
