using UnityEngine;

namespace BoardManager {

public class TilePrefab : MonoBehaviour
{
    public BoardState boardState;

    int q, r;

    public void SetCoords(int x, int y)
    {
        q = x;
        r = y;
    }
}

}