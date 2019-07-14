using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class TilePrefab : MonoBehaviour
    {
        float fadeRate = 1.0f;
        float fadeTimer = 1.0f;
        Color prevColor = Color.white;
        Color nextColor = Color.white;
        bool resourceTile = false;

        int q, r;

        void Update()
        {
            if (!resourceTile && nextColor != prevColor)
            {
                if (fadeTimer > 0.0f)
                {
                    fadeTimer -= Time.deltaTime;
                    Color lerpedColor;
                    lerpedColor = Color.Lerp(prevColor, nextColor, 1.0f - Mathf.Max(0.0f, fadeTimer / fadeRate));
                    this.GetComponent<MeshRenderer>().material.color = lerpedColor;
                }
                else
                {
                    prevColor = nextColor;
                    this.GetComponent<MeshRenderer>().material.color = nextColor;
                }
            }
        }

        public void SetCoords(int x, int y)
        {
            q = x;
            r = y;
        }

        public void SetFadeRate(float rate)
        {
            this.fadeRate = rate;
        }

        public void SetColor(Color color, bool instant)
        {
            if (instant)
                this.prevColor = color;
            else
                this.prevColor = this.GetComponent<MeshRenderer>().material.color;
            this.nextColor = color;
            fadeTimer = fadeRate;
        }

        public void SetResourceTile()
        {
            resourceTile = true;
        }
    }
}