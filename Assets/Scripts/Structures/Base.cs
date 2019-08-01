namespace GeometryBattles.StructureManager
{
    public class Base : Structure
    {
        public int baseMaxHP;
        public int baseRegen;
        public int baseArmor;

        void OnEnable()
        {
            hp = baseMaxHP;
            maxhp = baseMaxHP;
            regen = baseRegen;
            armor = baseArmor;
        }
    }
}