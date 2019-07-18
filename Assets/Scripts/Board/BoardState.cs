using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class BoardState : MonoBehaviour
    {
        public Resource resource;
        List<Player> players = new List<Player>();
        Dictionary<Player, Vector2Int> bases = new Dictionary<Player, Vector2Int>();

        public Color baseTileColor = Color.white;
        public int spreadAmount = 1;
        public float spreadRate = 0.1f;
        float spreadTimer = 0.0f;
        
        public int infMax = 200;
        public int infThreshold = 100;

        int cap = -1;
        List<List<TileState>> grid;
        List<List<TileState>> buffer;

        void Update()
        {
            spreadTimer -= Time.deltaTime;
            CalcMining();
            UpdateColors();
            if (spreadTimer <= 0.0f)
            {
                CalcBuffer();
                spreadTimer = spreadRate;
            }
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

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public Player GetPlayer(int i)
        {
            return players[i];
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
            if (owner == null || owner == player)
                gridbuffer[q][r].Set(player, Mathf.Min(influence + value, infMax));
            else
            {   
                if (influence < value)
                    gridbuffer[q][r].Set(player, Mathf.Min(value - influence, infMax));
                else if (influence > value)
                    gridbuffer[q][r].Set(owner, Mathf.Min(influence - value, infMax));
                else
                    gridbuffer[q][r].Set(null, 0);
            }
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
                    List<Vector2Int> neighbors = GetNeighbors(i, j);
                    foreach (var n in neighbors)
                    {
                        if (grid[n[0]][n[1]].GetInfluence() >= infThreshold)
                        {
                            AddNode(i, j, grid[n[0]][n[1]].GetOwner(), spreadAmount + grid[n[0]][n[1]].GetBuff(grid[n[0]][n[1]].GetOwner()), false);
                        }
                    }
                    grid[i][j].SetColor(buffer[i][j].GetOwner(), buffer[i][j].GetInfluence(), infThreshold, baseTileColor, false);
                }
            }
            SwapBuffer();
        }

        void CalcMining()
        {
            foreach (var p in players)
                p.SetMiningAmount(resource.startMiningAmount);
            HashSet<Vector2Int> resourceTiles = resource.GetResourceTiles();
            foreach (var r in resourceTiles)
            {
                Player owner = grid[r[0]][r[1]].GetOwner();
                int influence = grid[r[0]][r[1]].GetInfluence();
                if (influence >= infThreshold && IsConnectedToBase(r[0], r[1], owner))
                    owner.AddMiningAmount(resource.resourceTileAmount);
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

        public void SetBuff(int q, int r, Player player, int buff)
        {
            grid[q][r].SetBuff(player, buff);
        }
    }
}