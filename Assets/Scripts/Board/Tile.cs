using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class Tile : MonoBehaviour
    {
        public Color prevColor = Color.white;
        public Color nextColor = Color.white;

        int q, r;

        public int Q { get => q; }
        public int R { get => r; }

        public void SetCoords(int x, int y)
        {
            q = x;
            r = y;
        }

        public void SetColor(Color color, bool instant)
        {
            if (instant)
                this.prevColor = color;
            else
                this.prevColor = this.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
            this.nextColor = color;
        }

        public Color GetPrevColor()
        {
            return prevColor;
        }

        public Color GetNextColor()
        {
            return nextColor;
        }
    }
}