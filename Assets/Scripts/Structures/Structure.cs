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
        public int hp;
        protected int level = 1;
        
        public void Sell()
        {
            Destroy(gameObject);
        }

        public void SetCoords(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public void SetColor(Color color)
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
        }
    }
}