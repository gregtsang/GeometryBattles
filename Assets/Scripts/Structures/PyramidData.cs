namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class PyramidLevel : StructureLevel
    {
        public int range;
        public int strength;
    }

    public class PyramidData : StructureData<PyramidLevel> { }
}
