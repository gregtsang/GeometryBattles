using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class Pyramid : Structure
    {
        PyramidData stats;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<PyramidData>();
        }

        void Start()
        {
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public void Buff(int range, int strength)
        {
            for (int i = -range; i <= range; i++)
            {
                for (int j = Mathf.Max(-range, -range - i); j <= Mathf.Min(range, range - i); j++)
                {
                    if (this.q + i >= 0 && this.r + j >= 0)
                        boardState.SetBuff(this.q + i, this.r + j, boardState.GetNodeOwner(this.q, this.r), strength);
                }
            }
        }

        public override int GetMaxHP()
        {
            return stats.currLevel.maxHP;
        }

        public override int GetHPRegen()
        {
            return stats.currLevel.regen;
        }

        public override void Upgrade()
        {
            stats.Upgrade();
            boardState.SetNodeHP(this.q, this.r, stats.currLevel.maxHP);
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public override void Destroy()
        {
            Buff(stats.currLevel.range, 0);
            Destroy(gameObject);
        }
    }
}