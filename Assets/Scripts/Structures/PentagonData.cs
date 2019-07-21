﻿using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class PentagonLevel
    {
        public int cost;
        public int maxHP;
        public int regen;
        public float bombRate;
        public int bombStrength;
        public int bombRadius;
    }

    public class PentagonData : MonoBehaviour
    {
        public PentagonLevel currLevel;

        public List<PentagonLevel> levels;

        void OnEnable()
        {
            currLevel = levels[0];
        }

        public PentagonLevel GetNextLevel()
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