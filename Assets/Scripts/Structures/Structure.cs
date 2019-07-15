using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class Structure : MonoBehaviour
    {
        public BoardState boardState;
        protected int q, r;
        protected GameObject player;
        public int hp = 100;
        protected int level;

        public void Sell()
        {
            Destroy(this);
        }

        public void SetCoords(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
    }
}