using UnityEngine;

namespace GeometryBattles.StructureManager
{
    [System.Serializable]
    public class StructureLevel
    {
        public int cost;
        public int maxHP;
        public int regen;
        public int armor;
    }

    public class StructureData<T> : MonoBehaviour, IStructureData where T : StructureLevel
    {
        protected int currIndex = 0;
        public T currLevel;
        public T[] levels;

        public int GetCost()
        {
            int totalCost = 0;
            for (int i = 0; i <= currIndex; i++)
            {
                totalCost += levels[i].cost;
            }
            return totalCost;
        }

        public int GetUpgradeCost()
        {
            return currIndex < levels.Length - 1 ? levels[currIndex + 1].cost : -1;
        }

        public void Upgrade()
        {
            if (currIndex < levels.Length - 1)
            {
                currLevel = levels[++currIndex];
            }
        }
    }

    public interface IStructureData
    {
        int GetCost();
        int GetUpgradeCost();
        void Upgrade();
    }
}
