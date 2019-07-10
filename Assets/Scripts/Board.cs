using UnityEngine;

namespace BoardManager {

public class Board : MonoBehaviour
{
    public Camera gameCam;
    public BoardState boardState;
    public GameObject tilePrefab;

    public int boardWidth = 20;

    float tileWidth = 1.73205f;
    float tileLength = 2.0f;
    public float tileGap = 0.15f;
    
    void Start()
    {
        boardState.SetCap(boardWidth);
        gameCam.orthographicSize = boardWidth;
        SetGaps(tileGap);
        CreateBoard();
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
                boardState.InitNode(tile, q, r);
                tile.GetComponent<TilePrefab>().SetCoords(q, r);
            }
        }
    }
}

}