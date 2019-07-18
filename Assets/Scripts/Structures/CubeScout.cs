using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class CubeScout : MonoBehaviour
    {
        public Board board;
        public BoardState boardState;
        public Player player;

        public float moveRate = 5.0f;
        float moveTimer = 0.0f;
        public int numMoves = 10;
        int q, r;

        void Start()
        {
            board = GameObject.Find("Board").GetComponent<Board>();
            boardState = GameObject.Find("Board").GetComponent<BoardState>();
        }

        void Update()
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0.0f)
            {
                Vector2Int next = NextTile();
                Move(next[0], next[1]);
                q = next[0];
                r = next[1];
                boardState.SetNode(q, r, player);
                moveTimer = moveRate;
                numMoves--;
            }
            if (numMoves == 0)
                Destroy(gameObject);
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

        void Move(int q, int r)
        {
            if (q - this.q == 1 && r - this.r == -1)
                this.transform.position += new Vector3(0, 0, board.GetTileWidth());
            else if (q - this.q == -1 && r - this.r == 1)
                this.transform.position += new Vector3(0, 0, -board.GetTileWidth());
            else if (q - this.q == 1)
                this.transform.position += new Vector3(0.75f * board.GetTileLength(), 0, board.GetTileWidth() / 2.0f);
            else if (q - this.q == -1)
                this.transform.position += new Vector3(-0.75f * board.GetTileLength(), 0, -board.GetTileWidth() / 2.0f);
            else if (r - this.r == 1)
                this.transform.position += new Vector3(0.75f * board.GetTileLength(), 0, -board.GetTileWidth() / 2.0f);
            else
                this.transform.position += new Vector3(-0.75f * board.GetTileLength(), 0, board.GetTileWidth() / 2.0f);
        }

        Vector2Int NextTile()
        {
            List<Vector2Int> neighbors = boardState.GetNeighbors(q, r);
            return neighbors[Random.Range(0, neighbors.Count)];
        }
    }
}
