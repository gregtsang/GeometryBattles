using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class Board : MonoBehaviour
    {
        public Camera gameCam;
        public BoardState boardState;
        public GameObject tilePrefab;
        public GameObject playerPrefab;
        public Resource resource;

        public int boardWidth = 20;
        public int baseOffset = 2;
        public int numPlayers = 2;

        float tileWidth = 1.73205f;
        float tileLength = 2.0f;
        public float tileGap = 0.15f;
        
        void Start()
        {
            boardState.SetCap(boardWidth);
            resource.InitResourceTiles(boardWidth, baseOffset);
            gameCam.orthographicSize = boardWidth;
            SetGaps(tileGap);
            CreateBoard();
            CreatePlayers(numPlayers);
            boardState.ResetTimer();
        }

        void Update()
        {
            boardState.SubTimer(Time.deltaTime);
            if (boardState.GetTimer() <= 0.0f)
            {
                boardState.ResetTimer();
                boardState.CalcBuffer();
                boardState.SwapBuffer();
            }
        }

        void SetGaps(float gap)
        {
            tileWidth += tileWidth * gap;
            tileLength += tileLength * gap;
        }

        Vector3 CalcPos(Vector2Int boardPos, int numTiles)
        {
            float x = (numTiles - 1.0f) * -0.75f * tileLength + boardPos.x * 1.5f * tileLength;
            float z = (boardWidth - 1.0f - boardPos.y) * (tileWidth / 2.0f);

            return new Vector3(x, 0, z);
        }

        void CreateBoard()
        {
            for (int y = 0; y < 2 * boardWidth - 1; y++)
            {
                int numTiles = boardWidth - Mathf.Abs(boardWidth - y - 1);

                for (int x = 0; x < numTiles; x++)
                {
                    Vector2Int boardPos = new Vector2Int(x, y);
                    Vector3 tilePos = CalcPos(boardPos, numTiles);
                    GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, this.transform) as GameObject;
                    int q = y < boardWidth ? boardWidth - 1 - y + x : x;
                    int r = y < boardWidth ? x : y - boardWidth + 1 + x;
                    tile.name = "Tile" + q + "." + r;
                    tile.GetComponent<TilePrefab>().SetCoords(q, r);
                    boardState.InitNode(tile, q, r);
                    if (resource.IsResourceTile(q, r))
                        tile.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
            }
        }

        void CreatePlayers(int numPlayers)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                GameObject player = Instantiate(playerPrefab, this.transform.position, Quaternion.identity, this.transform) as GameObject;
                player.name = "Player" + (i + 1);
                player.GetComponent<PlayerPrefab>().SetColor(1.0f * i / numPlayers);
                boardState.AddPlayer(player);
            }
        }
    }
}