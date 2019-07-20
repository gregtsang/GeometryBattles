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
        public int maxHP;
        public int regenHP;
        protected int hp;
        protected int level = 1;

        public int Q { get => q; }
        public int R { get => r; }

        public void Destroy()
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

        public int GetMaxHP()
        {
            return maxHP;
        }

        public int GetHPRegen()
        {
            return regenHP;
        }
    }
}