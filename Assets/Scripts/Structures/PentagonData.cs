namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class PentagonLevel : StructureLevel
    {
        public float bombRate;
        public int bombStrength;
        public int bombRadius;
    }

    public class PentagonData : StructureData<PentagonLevel> { }
}
