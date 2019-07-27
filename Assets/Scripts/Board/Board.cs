using System.Collections;
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
        public GameObject basePrefab;

        public int boardWidth;
        public int baseOffset;
        public int numPlayers;
        [ColorUsageAttribute(true,true)]
        public List<Color> playerColors;

        float tileWidth;
        float tileLength;
        
        int fadeCount;
        float fadeDistance = 50.0f;
        float dropDistance = 200.0f;
        float dropSpeed = -1500.0f;

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
            StartCoroutine(SetBoard());
        }

        Vector3 CalcPos(Vector2Int boardPos, int numTiles)
        {
            float x = (numTiles - 1.0f) * -0.75f * tileLength + boardPos.x * 1.5f * tileLength;
            float z = (boardWidth - 1.0f - boardPos.y) * (tileWidth / 2.0f);

            return new Vector3(x, -fadeDistance, z);
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
                    randColor.a = 0.0f;
                    tile.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randColor);
                    boardState.InitNode(q, r, currTile);
                }
            }
        }

        IEnumerator SetBoard()
        {
            fadeCount = 0;
            List<Vector2Int> tiles = new List<Vector2Int>();
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                    tiles.Add(new Vector2Int(i, j));
            }
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Vector2Int temp = tiles[i];
                tiles[i] = tiles[j];
                tiles[j] = temp;
            }
            int index = 0;
            while (index < tiles.Count)
            {
                for (int i = index; i < index + boardWidth; i++)
                    StartCoroutine(FadeInTile(tiles[i][0], tiles[i][1]));
                index += boardWidth;
                yield return new WaitForSeconds(0.05f);
            }
            while (fadeCount < boardWidth * boardWidth)
                yield return null;
            HashSet<Vector2Int> resources = resource.GetResourceTiles();
            foreach(var r in resources)
            {
                Tile currTile = boardState.GetNodeTile(r[0], r[1]);
                Color baseColor = currTile.GetBaseColor();
                float lerpRate = 0.2f;
                while (lerpRate > 0.0f)
                {
                    lerpRate -= Time.deltaTime;
                    Color lerpedColor = Color.Lerp(baseColor, Color.white * 2, 1.0f - (lerpRate / 0.2f));
                    currTile.gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", lerpedColor);
                    yield return null;
                }
            }
            StartCoroutine(SetBase(baseOffset, boardWidth - baseOffset - 1, 0));
            StartCoroutine(SetBase(boardWidth - baseOffset - 1, baseOffset, 1));
            StartCoroutine(SetBase(baseOffset, baseOffset, 2));
            StartCoroutine(SetBase(boardWidth - baseOffset - 1, boardWidth - baseOffset - 1, 3));
            boardState.StartGame();
        }

        IEnumerator FadeInTile(int q, int r)
        {
            Tile currTile = boardState.GetNodeTile(q, r);
            float speed = Random.Range(fadeDistance, fadeDistance * 2);
            Color currColor = currTile.GetBaseColor();
            while (currTile.transform.position.y != 0.0f)
            {
                float dist = 0.0f - currTile.transform.position.y;
                currTile.transform.Translate(0.0f, Mathf.Min(Mathf.Pow((dist / fadeDistance), 0.5f) * speed * Time.deltaTime, dist), 0.0f);
                currColor.a = Mathf.Pow(1.0f + (currTile.transform.position.y / fadeDistance), 2.0f);
                currTile.gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", currColor);
                yield return null;
            }
            fadeCount++;
        }

        IEnumerator SetBase(int q, int r, int player)
        {
            Vector3 basePos = boardState.GetNodeTile(q, r).transform.position;
            basePos += new Vector3(0.0f, dropDistance, 0.0f);
            GameObject currBase = Instantiate(basePrefab, basePos, basePrefab.transform.rotation) as GameObject;
            currBase.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", playerColors[player] * 2);
            Color trailColor = playerColors[player] * 2;
            currBase.GetComponent<TrailRenderer>().material.SetColor("_BaseColor", trailColor);
            while (currBase.transform.position.y > 0.5f)
            {
                float dist = currBase.transform.position.y - 0.5f;
                currBase.transform.Translate(0.0f, Mathf.Max(Mathf.Pow(dist / (dropDistance - 0.5f), 0.5f) * dropSpeed * Time.deltaTime, -dist), 0.0f, Space.World);
                yield return null;
            }
            boardState.SetNode(q, r, boardState.GetPlayer(player));
            boardState.AddBase(q, r, boardState.GetPlayer(player));
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
    }
}