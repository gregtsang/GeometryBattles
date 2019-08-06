using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class CubeScout : MonoBehaviour
    {
        public Player player;

        float moveRate;
        int movesLeft = 10;
        float currRot;
        int q, r;
        int homeQ, homeR;
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        public int Q { get => q; }
        public int R { get => r; }

        void Start()
        {
            currRot = gameObject.transform.eulerAngles.y;
        }

        public void SetHome(int q, int r)
        {
            homeQ = q;
            homeR = r;
        }

        public Vector2Int GetHome()
        {
            return new Vector2Int(homeQ, homeR);
        }

        public float GetRotation()
        {
            return currRot;
        }

        public void SetRotation(float rot)
        {
            currRot = rot;
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

        public void SetMoveRate(float rate)
        {
            moveRate = rate;
        }

        public float GetMoveRate()
        {
            return moveRate;
        }

        public void DecMoves()
        {
            movesLeft--;
        }

        public int GetMoves()
        {
            return movesLeft;
        }

        public void SetMoves(int moves)
        {
            movesLeft = moves;
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
