using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    public GameState gameState;

    int q, r;

    public void SetCoords(int x, int y)
    {
        q = x;
        r = y;
    }

    void OnMouseDown()
    {
        if (gameState.GetNode(q, r) == -1)
        {
            int player = gameState.GetPlayer();
            this.GetComponent<MeshRenderer>().material.color = player == 0 ? Color.red : Color.blue;
            gameState.SetNode(q, r, player);
            if (gameState.GameOver(q, r, player))
                Debug.Log("Game Over");
            else
                gameState.NextPlayer();
        }
    }
}
