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
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
        }

        public override void StartEffect()
        {
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public void Buff(int range, int strength)
        {
            for (int i = -range; i <= range; i++)
            {
                for (int j = Mathf.Max(-range, -range - i); j <= Mathf.Min(range, range - i); j++)
                {
                    if (boardState.IsValidTile(this.q + i, this.r + j))
                        boardState.SetNodeBuff(this.q + i, this.r + j, boardState.GetNodeOwner(this.q, this.r), strength);
                }
            }
        }

        public override void Upgrade()
        {
            stats.Upgrade();
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
            Buff(stats.currLevel.range, stats.currLevel.strength);
        }

        public override void Destroy()
        {
            Buff(stats.currLevel.range, 0);
            Destroy(gameObject);
        }
    }
}