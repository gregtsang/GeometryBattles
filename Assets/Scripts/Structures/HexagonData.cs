namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class HexagonLevel : StructureLevel
    {
        public float buildTime;
    }

    public class HexagonData : StructureData<HexagonLevel> { }
}
