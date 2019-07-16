using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class CubeScout : MonoBehaviour
    {
        public BoardState boardState;
        public Board board;

        public float moveRate = 1.0f;
        float moveTimer = 1.0f;
        public int numMoves = 10;
        int q, r;

        void Move(int q, int r)
        {
            if (q - this.q == 1 && r - this.r == -1)
                this.transform.position += new Vector3(0, 0, board.tileWidth);
            else if (q - this.q == -1 && r - this.r == 1)
                this.transform.position += new Vector3(0, 0, -board.tileWidth);
            else if (q - this.q == 1)
                this.transform.position += new Vector3(0.75f * board.tileLength, 0, board.tileWidth / 2.0f);
            else if (q - this.q == -1)
                this.transform.position += new Vector3(-0.75f * board.tileLength, 0, -board.tileWidth / 2.0f);
            else if (r - this.r == 1)
                this.transform.position += new Vector3(0.75f * board.tileLength, 0, -board.tileWidth / 2.0f);
            else
                this.transform.position += new Vector3(-0.75f * board.tileLength, 0, board.tileWidth / 2.0f);
        }

        Vector2Int NextTile()
        {
            List<Vector2Int> neighbors = boardState.GetNeighbors(q, r);
            return neighbors[Random.Range(0, neighbors.Count)];
        }
    }
}
