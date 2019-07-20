using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class CubeScout : MonoBehaviour
    {
        public Player player;

        float moveTimer = 0.0f;
        int movesLeft = 10;
        int q, r;
        int homeQ, homeR;
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        public int Q { get => q; }
        public int R { get => r; }

        public void SetHome(int q, int r)
        {
            homeQ = q;
            homeR = r;
        }

        public Vector2Int GetHome()
        {
            return new Vector2Int(homeQ, homeR);
        }

        public void SetCoords(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public void SetTimer(float time)
        {
            this.moveTimer = time;
        }

        public float GetTimer()
        {
            return moveTimer;
        }

        public void DecMoves()
        {
            movesLeft--;
        }

        public int GetMoves()
        {
            return movesLeft;
        }

        public void AddVisited(int q, int r)
        {
            visited.Add(new Vector2Int(q, r));
        }

        public bool HasVisited(int q, int r)
        {
            return visited.Contains(new Vector2Int(q, r));
        }

        public void SelfDestruct()
        {
            Destroy(gameObject);
        }
    }
}
