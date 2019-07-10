using UnityEngine;

public class Map : MonoBehaviour
{
    public Camera gameCam;
    public GameState gameState;
    public GameObject tilePrefab;

    public int mapWidth;

    float tileWidth = 1.73205f;
    float tileLength = 2.0f;
    public float tileGap = 0.15f;
    
    void Start()
    {
        gameState.SetCap(mapWidth);
        gameCam.orthographicSize = mapWidth;
        SetGaps(tileGap);
        CreateMap();
    }

    void SetGaps(float gap)
    {
        tileWidth += tileWidth * gap;
        tileLength += tileLength * gap;
    }

    Vector3 CalcPos(Vector2Int mapPos, int numTiles)
    {
        float x = (numTiles - 1.0f) * -0.75f * tileLength + mapPos.x * 1.5f * tileLength;
        float z = (mapWidth - 1.0f - mapPos.y) * (tileWidth / 2.0f);

        return new Vector3(x, 0, z);
    }

    void CreateMap()
    {
        for (int y = 0; y < 2 * mapWidth - 1; y++)
        {
            int numTiles = mapWidth - Mathf.Abs(mapWidth - y - 1);

            for (int x = 0; x < numTiles; x++)
            {
                Vector2Int mapPos = new Vector2Int(x, y);
                Vector3 tilePos = CalcPos(mapPos, numTiles);
                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, this.transform) as GameObject;
                int q = y < mapWidth ? mapWidth - 1 - y + x : x;
                int r = y < mapWidth ? x : y - mapWidth + 1 + x;
                tile.name = "Tile" + q + "." + r;
                gameState.InitNode(tile, q, r);
                tile.GetComponent<TilePrefab>().SetCoords(q, r);
            }
        }
    }
}
