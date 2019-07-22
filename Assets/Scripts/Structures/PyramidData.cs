using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class PyramidLevel
    {
        public int cost;
        public int maxHP;
        public int regen;
        public int range;
        public int strength;
    }

    public class PyramidData : MonoBehaviour
    {
        public PyramidLevel currLevel;

        public List<PyramidLevel> levels;

        void OnEnable()
        {
            currLevel = levels[0];
        }

        public PyramidLevel GetNextLevel()
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
