using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class Board : MonoBehaviour
    {
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
        
        List<Vector2Int> tileSet = new List<Vector2Int>();
        int fadeCount;
        float fadeDistance = 50.0f;
        float dropDistance = 200.0f;
        float dropSpeed = -1500.0f;

        void Awake()
        {   
            boardState.SetCap(boardWidth);
            //resource.InitResourceTiles(boardWidth, baseOffset);

            boardState.SetGaps();
            this.tileWidth = boardState.GetTileWidth();
            this.tileLength = boardState.GetTileLength();

            string shape = numPlayers == 2 ? "rhombus" : "hexagon";
                
            CreateBoard(shape);
            CreatePlayers(numPlayers);
            StartCoroutine(SetBoard());
        }

        Vector3 CalcPos(Vector2Int boardPos, int numTiles, int rows)
        {
            float x = (numTiles - 1.0f) * -0.75f * tileLength + boardPos.x * 1.5f * tileLength;
            float z = (rows / 2 - boardPos.y) * (tileWidth / 2.0f);

            return new Vector3(x, -fadeDistance, z);
        }

        void CreateBoard(string shape)
        {
            switch (shape)
            {
                case "rhombus":
                    CreateRhombus();
                    break;
                case "hexagon":
                    CreateHexagon();
                    break;
            }
        }

        void CreateRhombus()
        {
            GameObject tiles = new GameObject();
            tiles.name = "Tiles";
            tiles.transform.parent = this.transform;
            int rows = 2 * boardWidth - 1;
            for (int y = 0; y < rows; y++)
            {
                int numTiles = boardWidth - Mathf.Abs(boardWidth - y - 1);

                for (int x = 0; x < numTiles; x++)
                {
                    Vector2Int boardPos = new Vector2Int(x, y);
                    Vector3 scenePos = CalcPos(boardPos, numTiles, rows);
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
                    tileSet.Add(new Vector2Int(q, r));
                }
            }
        }

        void CreateHexagon()
        {
            GameObject tiles = new GameObject();
            tiles.name = "Tiles";
            tiles.transform.parent = this.transform;
            
            int rows = 4 * boardWidth - 3;
            for (int y = 0; y < rows; y++)
            {
                int numTiles;
                if (y < boardWidth) numTiles = y + 1;
                else if (y >= rows - boardWidth) numTiles = rows - y;
                else if (boardWidth % 2 == 0) numTiles = y % 2 == 1 ? boardWidth : boardWidth - 1;
                else numTiles = y % 2 == 0 ? boardWidth : boardWidth - 1;

                for (int x = 0; x < numTiles; x++)
                {
                    Vector2Int boardPos = new Vector2Int(x, y);
                    Vector3 scenePos = CalcPos(boardPos, numTiles, rows);
                    GameObject tile = Instantiate(tilePrefab, scenePos, Quaternion.identity, tiles.transform) as GameObject;
                    int q, r;
                    if (y < boardWidth)
                    {
                        q = boardWidth - 1 - y + 2 * x;
                        r = numTiles - 1 - x;
                    }
                    else if (y >= rows - boardWidth)
                    {
                        q = boardWidth - rows + y + 2 * x;
                        r = 2 * (boardWidth - 1) - x;
                    }
                    else
                    {
                        q = boardWidth - numTiles + 2 * x;
                        r = (y - boardWidth + 1) / 2 + boardWidth - 1 - x;
                    }
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
                    tileSet.Add(new Vector2Int(q, r));
                }
            }
        }

        IEnumerator SetBoard()
        {
            fadeCount = 0;
            for (int i = tileSet.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Vector2Int temp = tileSet[i];
                tileSet[i] = tileSet[j];
                tileSet[j] = temp;
            }
            int index = 0;
            while (index < tileSet.Count)
            {
                for (int i = index; i < Mathf.Min(index + tileSet.Count / boardWidth, tileSet.Count); i++)
                {
                    StartCoroutine(FadeInTile(tileSet[i][0], tileSet[i][1]));
                }
                index += tileSet.Count / boardWidth;
                yield return new WaitForSeconds(0.05f);
            }
            while (fadeCount < tileSet.Count) yield return null;
            HashSet<Vector2Int> resources = resource.GetResourceTiles();
            foreach(Vector2Int r in resources)
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
            if (numPlayers == 2)
            {
                StartCoroutine(SetBase(baseOffset, boardWidth - baseOffset - 1, 0));
                StartCoroutine(SetBase(boardWidth - baseOffset - 1, baseOffset, 1));
            }
            else if (numPlayers == 3)
            {
                Vector3Int curr = new Vector3Int(boardWidth - 1, baseOffset, 1 - boardWidth - baseOffset);
                Vector3Int center = new Vector3Int(boardWidth - 1, boardWidth - 1, 2 - 2 * boardWidth);
                StartCoroutine(SetBase(curr[0], curr[1], 0));
                for (int i = 1; i < numPlayers; i++)
                {
                    curr = Rotate120(curr, center);
                    StartCoroutine(SetBase(curr[0], curr[1], i));
                }
            }
            else if (numPlayers == 6)
            {
                Vector3Int curr = new Vector3Int(boardWidth - 1, baseOffset, 1 - boardWidth - baseOffset);
                Vector3Int center = new Vector3Int(boardWidth - 1, boardWidth - 1, 2 - 2 * boardWidth);
                StartCoroutine(SetBase(curr[0], curr[1], 0));
                for (int i = 1; i < numPlayers; i++)
                {
                    curr = Rotate60(curr, center);
                    StartCoroutine(SetBase(curr[0], curr[1], i));
                }
            }
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

        Vector3Int Rotate60(Vector3Int curr, Vector3Int center)
        {
            Vector3Int temp = curr - center;
            Vector3Int res = new Vector3Int(-temp[2], -temp[0], -temp[1]);
            res += center;
            return res;
        }

        Vector3Int Rotate120(Vector3Int curr, Vector3Int center)
        {
            Vector3Int temp = curr - center;
            Vector3Int res = new Vector3Int(temp[1], temp[2], temp[0]);
            res += center;
            return res;
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