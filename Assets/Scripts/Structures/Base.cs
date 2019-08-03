namespace GeometryBattles.StructureManager
{
    public class Base : Structure
    {
        public int baseMaxHP;
        public int baseRegen;
        public int baseArmor;

        void Start()
        {
            hp = baseMaxHP;
            maxhp = baseMaxHP;
            regen = baseRegen;
            armor = baseArmor;
        }
    }
}