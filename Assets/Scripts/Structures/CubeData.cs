namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class CubeLevel : StructureLevel
    {
        public float spawnRate;
        public float moveRate;
        public int numMoves;
    }

    public class CubeData : StructureData<CubeLevel> { }
}
