using System.Collections;
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
        [SerializeField]
        protected int hp;
        protected int maxhp;
        protected int regen;
        protected int armor;

        Material mat;
        public Material Mat { get => mat; }

        public int Q { get => q; }
        public int R { get => r; }

        void Awake()
        {
            mat = gameObject.GetComponent<MeshRenderer>().material;
        }

        virtual public void StartEffect() {}

        virtual public void Upgrade() {}

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
            this.Mat.SetColor("_BaseColor", color);
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
            return maxhp;
        }

        public int GetHPRegen()
        {
            return regen;
        }

        public int GetArmor()
        {
            return armor;
        }

        protected IEnumerator RegenHP()
        {
            while (hp > 0)
            {
                hp = Mathf.Min(hp + regen, maxhp);
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}