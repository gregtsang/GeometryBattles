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
        protected int hp;

        public int Q { get => q; }
        public int R { get => r; }

        virtual public void Upgrade()
        {
            
        }

        virtual public void Destroy()
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

        public void SetHP(int hp)
        {
            this.hp = hp;
        }

        public int GetHP()
        {
            return hp;
        }

        public virtual int GetMaxHP()
        {
            return 0;
        }

        public virtual int GetHPRegen()
        {
            return 0;
        }

    }
}