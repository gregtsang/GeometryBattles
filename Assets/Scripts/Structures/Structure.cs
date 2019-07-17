using UnityEngine;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.StructureManager
{
    public class Structure : MonoBehaviour
    {
        public BoardState boardState;
        protected int q, r;
        protected Player player;
        public int hp = 100;
        protected int level = 1;
        
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