using System.Collections.Generic;
using UnityEngine;

namespace BoardManager {

public class TileState
{
    public TileState() {}

    public TileState(int owner, int influence)
    {
        this.Owner = owner;
        this.Influence = influence;
    }

    int Owner;
    int Influence;

    public int GetOwner()
    {
        return this.Owner;
    }

    public int GetInfluence()
    {
        return this.Influence;
    }

    public void Set(int owner, int influence)
    {
        this.Owner = owner;
        this.Influence = influence;
    }
}

[CreateAssetMenu]
public class BoardState : ScriptableObject
{
    public int numPlayers = 2;
    public int spreadRate = 5;
    int cap = -1;
    List<List<TileState>> grid;
    List<List<TileState>> buffer;

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
                grid[i].Add(new TileState(-1, 0));
                buffer[i].Add(new TileState(-1, 0));
            }
        }
        cap = n;
    }

    public Vector2Int GetNode(int q, int r)
    {
        return new Vector2Int(grid[q][r].GetOwner(), grid[q][r].GetInfluence());
    }

    public void SetNode(int q, int r, int owner, int influence, int target = 0)
    {
        List<List<TileState>> gridbuffer = target == 0 ? grid : buffer;
        gridbuffer[q][r].Set(owner, influence);
    }

    public void AddNode(int q, int r, int player, int value, int target = 0)
    {
        List<List<TileState>> gridbuffer = target == 0 ? grid : buffer;
        int owner = gridbuffer[q][r].GetOwner();
        int influence = gridbuffer[q][r].GetInfluence();
        if (owner == -1 || owner == player)
            gridbuffer[q][r].Set(player, Mathf.Min(influence + value, 100));
        else
        {
            if (influence < value)
                gridbuffer[q][r].Set(player, Mathf.Min(value - influence, 100));
            else if (influence > value)
                gridbuffer[q][r].Set(owner, Mathf.Min(influence - value, 100));
            else
                gridbuffer[q][r].Set(-1, 0);
        }
    }

    public int ClosestOwned(int q, int r, int player)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(new Vector2Int(q, r));
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(new Vector3Int(q, r, 0));
        while (queue.Count > 0)
        {
            Vector3Int curr = queue.Dequeue();
            if (grid[curr[0]][curr[1]].GetOwner() == player && grid[curr[0]][curr[1]].GetInfluence() == 100)
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

    List<Vector2Int> GetNeighbors(int q, int r)
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
        for (int i = 0; i < cap - 1; i++)
        {
            for (int j = 0; j < cap - 1; j++)
            {
                buffer[i][j].Set(grid[i][j].GetOwner(), grid[i][j].GetInfluence());
                List<Vector2Int> neighbors = GetNeighbors(i, j);
                foreach (var n in neighbors)
                    if (grid[n[0]][n[1]].GetInfluence() == 100)
                        AddNode(i, j, grid[n[0]][n[1]].GetOwner(), spreadRate, 1);
            }
        }
    }
}

}