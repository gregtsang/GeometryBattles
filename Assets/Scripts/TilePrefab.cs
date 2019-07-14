using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class TilePrefab : MonoBehaviour
    {
        public BoardState boardState;
        public float fadeRate = 0.1f;
        float fadeTimer = 0.1f;
        Color color = Color.white;
        bool resourceTile = false;

        int q, r;

        void Update()
        {
            Color currColor = this.GetComponent<MeshRenderer>().material.color;
            if (!resourceTile && color != currColor)
            {
                if (fadeTimer > 0.0f)
                    fadeTimer -= Time.deltaTime;
                Color lerpedColor;
                lerpedColor = Color.Lerp(currColor, color, 1.0f - Mathf.Max(0.0f, fadeTimer / fadeRate));
                this.GetComponent<MeshRenderer>().material.color = lerpedColor;
            }
        }

        public void SetCoords(int x, int y)
        {
            q = x;
            r = y;
        }

        public void SetColor(Color color)
        {
            this.color = color;
            fadeTimer = fadeRate;
        }

        public void SetResourceTile()
        {
            resourceTile = true;
        }
    }
}