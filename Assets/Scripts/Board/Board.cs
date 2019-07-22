using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class Board : MonoBehaviour
    {
        public Camera gameCam;
        public BoardState boardState;
        public Resource resource;
        
        public GameObject tilePrefab;
        public GameObject playerPrefab;

        public int boardWidth;
        public int baseOffset;
        public int numPlayers;
        [ColorUsageAttribute(true,true)]
        public List<Color> playerColors;

        float tileWidth;
        float tileLength;
        
        void Awake()
        {
            gameCam.orthographicSize = boardWidth;
            
            boardState.SetCap(boardWidth);
            resource.InitResourceTiles(boardWidth, baseOffset);

            boardState.SetGaps();
            this.tileWidth = boardState.GetTileWidth();
            this.tileLength = boardState.GetTileLength();

            CreateBoard();
            CreatePlayers(numPlayers);

            SetBases(numPlayers);
        }

        Vector3 CalcPos(Vector2Int boardPos, int numTiles)
        {
            float x = (numTiles - 1.0f) * -0.75f * tileLength + boardPos.x * 1.5f * tileLength;
            float z = (boardWidth - 1.0f - boardPos.y) * (tileWidth / 2.0f);

            return new Vector3(x, 0, z);
        }

        void CreateBoard()
        {
            GameObject tiles = new GameObject();
            tiles.name = "Tiles";
            tiles.transform.parent = this.transform;
            for (int y = 0; y < 2 * boardWidth - 1; y++)
            {
                int numTiles = boardWidth - Mathf.Abs(boardWidth - y - 1);

                for (int x = 0; x < numTiles; x++)
                {
                    Vector2Int boardPos = new Vector2Int(x, y);
                    Vector3 scenePos = CalcPos(boardPos, numTiles);
                    GameObject tile = Instantiate(tilePrefab, scenePos, Quaternion.identity, tiles.transform) as GameObject;
                    int q = y < boardWidth ? boardWidth - 1 - y + x : x;
                    int r = y < boardWidth ? x : y - boardWidth + 1 + x;
                    tile.name = "Tile[" + q + "," + r + "]";
                    Tile currTile = tile.GetComponent<Tile>();
                    currTile.SetCoords(q, r);
                    Color randColor = Color.Lerp(boardState.baseTileColor, Color.white, Random.Range(0.0f, 0.1f));
                    currTile.SetBaseColor(randColor);
                    currTile.SetPrevColor(randColor);
                    currTile.SetNextColor(randColor);
                    boardState.InitNode(currTile, q, r);
                    if (resource.IsResourceTile(q, r))
                        tile.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white * 2);
                }
            }
        }

        void CreatePlayers(int numPlayers)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                GameObject player = Instantiate(playerPrefab, this.transform.position, Quaternion.identity, this.transform) as GameObject;
                player.name = "Player" + (i + 1);
                Player currPlayer = player.GetComponent<Player>();
                currPlayer.SetColor(playerColors[i]);
                currPlayer.SetResource(resource.startResource);
                boardState.AddPlayer(player.GetComponent<Player>());
            }
        }

        void SetBases(int numPlayers)
        {
            boardState.SetNode(baseOffset, boardWidth - baseOffset - 1, boardState.GetPlayer(0));
            boardState.AddBase(baseOffset, boardWidth - baseOffset - 1, boardState.GetPlayer(0));
            boardState.SetNode(boardWidth - baseOffset - 1, baseOffset, boardState.GetPlayer(1));
            boardState.AddBase(boardWidth - baseOffset - 1, baseOffset, boardState.GetPlayer(1));
            if (numPlayers >= 3)
            {
                boardState.SetNode(baseOffset, baseOffset, boardState.GetPlayer(2));
                boardState.AddBase(baseOffset, baseOffset, boardState.GetPlayer(2));
            }
            if (numPlayers == 4)
            {
                boardState.SetNode(boardWidth - baseOffset - 1, boardWidth - baseOffset - 1, boardState.GetPlayer(3));
                boardState.AddBase(boardWidth - baseOffset - 1, boardWidth - baseOffset - 1, boardState.GetPlayer(3));
            }
        }
    }
}