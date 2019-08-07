namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class CubeLevel : StructureLevel
    {
        public float spawnRate;
        public float moveRate;
        public int numMoves;
        public int structureDamage;
        public int tileInfluence;
    }

    public class CubeData : StructureData<CubeLevel> { }
}
