using System.Collections.Generic;
using UnityEngine;

namespace BoardManager {

public class Tuple3<T, U, V>
{
    public Tuple3() {}

    public Tuple3(T first, U second, V third)
    {
        this.First = first;
        this.Second = second;
        this.Third = third;
    }

    public T First { get; set; }
    public U Second { get; set; }
    public V Third { get; set; }
};

[CreateAssetMenu]
public class BoardState : ScriptableObject
{
    int cap = -1;
    List< List< Tuple3< GameObject, int, int> > > grid;

    int currPlayer = 0;
    int numPlayers = 2;

    public void SetCap(int n)
    {
        grid = new List< List< Tuple3< GameObject, int, int > > >(n);
        for (int i = 0; i < n; i++)
        {
            grid.Add(new List< Tuple3< GameObject, int, int > >(n));
            for (int j = 0; j < n; j++)
                grid[i].Add(new Tuple3< GameObject, int, int >(null, -1, 0));
        }
        cap = n;
    }

    public void InitNode(GameObject node, int q, int r)
    {
        if (cap > 0 && q < cap && r < cap)
            grid[q][r].First = node;
    }

    public int GetNode(int q, int r)
    {
        return grid[q][r].Second;
    }

    public void SetNode(int q, int r, int player)
    {
        grid[q][r].Second = player;
    }

    public int GetPlayer()
    {
        return currPlayer;
    }

    public void NextPlayer()
    {
        currPlayer = (currPlayer + 1) % numPlayers;
    }

    public bool GameOver(int q, int r, int player)
    {
        bool start = false, end = false;
        Vector2Int curr = new Vector2Int(q, r);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(curr);
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(curr);
        while (queue.Count > 0)
        {
            Vector2Int temp = queue.Dequeue();
            if (player == 0)
            {
                if (temp[0] == 0)
                    start = true;
                if (temp[0] == cap - 1)
                    end = true;
            }
            else
            {
                if (temp[1] == cap - 1)
                    start = true;
                if (temp[1] == 0)
                    end = true;
            }
            if (start && end)
                return true;
        
            List<Vector2Int> neighbors = GetNeighbors(temp[0], temp[1]);
            foreach (var n in neighbors)
            {
                if (!visited.Contains(n) && grid[n[0]][n[1]].Second == player)
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
        }

        return false;
    }

    List<Vector2Int> GetNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = Mathf.Max(x - 1, 0); i <= Mathf.Min(x + 1, cap - 1); i++)
        {
            for (int j = Mathf.Max(y - 1, 0); j <= Mathf.Min(y + 1, cap - 1); j++)
            {
                if (!(i == x && j == y) && !(i == x - 1 && j == y - 1) && !(i == x + 1 && j == y + 1))
                    neighbors.Add(new Vector2Int(i, j));
            }
        }
        return neighbors;
    }
}

}