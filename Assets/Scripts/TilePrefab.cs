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

    void OnMouseDown()
    {
        if (boardState.GetNode(q, r) == -1)
        {
            int player = boardState.GetPlayer();
            this.GetComponent<MeshRenderer>().material.color = player == 0 ? Color.red : Color.blue;
            boardState.SetNode(q, r, player);
            if (boardState.GameOver(q, r, player))
                Debug.Log("Game Over");
            else
                boardState.NextPlayer();
        }
    }
}

}