using System.Collections.Generic;

namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class CubeLevel
    {
        public int cost;
        public int maxHP;
        public int regen;
        public int armor;
        public float spawnRate;
        public float moveRate;
        public int numMoves;
    }

    public class CubeData : StructureData
    {
        public CubeLevel currLevel;

        public List<CubeLevel> levels;

        void OnEnable()
        {
            currLevel = levels[0];
        }

        public override int GetCost()
        {
            if (levels.IndexOf(currLevel) < 0)
            {
                return levels[0].cost;
            }
            else
            {
                int totalCost = 0;
                for (int i = 0; i <= levels.IndexOf(currLevel); i++)
                {
                    totalCost += levels[i].cost;
                }
                return totalCost;
            }
        }

        public CubeLevel GetNextLevel()
        {
            int currLevelIndex = levels.IndexOf(currLevel);
            return currLevelIndex < levels.Count - 1 ? levels[currLevelIndex + 1] : null;
        }

        public int GetUpgradeCost()
        {
            int currLevelIndex = levels.IndexOf(currLevel);
            return currLevelIndex < levels.Count - 1 ? levels[currLevelIndex + 1].cost : -1;
        }

        public void Upgrade()
        {
            int currLevelIndex = levels.IndexOf(currLevel);
            if (currLevelIndex < levels.Count - 1)
                currLevel = levels[currLevelIndex + 1];
        }
    }
}
