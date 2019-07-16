using UnityEngine;

namespace GeometryBattles.BoardManager
{
    public class Tile : MonoBehaviour
    {
        Color prevColor = Color.white;
        Color nextColor = Color.white;
        Material mat;

        int q, r;

        public int Q { get => q; }
        public int R { get => r; }

        void Start()
        {
            mat = this.GetComponent<MeshRenderer>().material;
        }

        public void SetCoords(int x, int y)
        {
            q = x;
            r = y;
        }

        public Material GetMat()
        {
            return mat;
        }

        public Color GetPrevColor()
        {
            return prevColor;
        }

        public Color GetNextColor()
        {
            return nextColor;
        }

        public void SetPrevColor(Color color)
        {
            prevColor = color;
        }

        public void SetNextColor(Color color)
        {
            nextColor = color;
        }
    }
}