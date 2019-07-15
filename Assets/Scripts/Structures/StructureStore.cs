using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.StructureManager
{
    public class StructureStore : ScriptableObject
    {
        Dictionary<Vector2Int, GameObject> structures = new Dictionary<Vector2Int, GameObject>();

        public void AddStructure(int q, int r, GameObject structure)
        {
            structures[new Vector2Int(q, r)] = structure;
        }

        public void RemoveStructure(int q, int r)
        {
            Vector2Int key = new Vector2Int(q, r);
            structures[key].GetComponent<Structure>().Sell();
            structures.Remove(key);
        }

        public bool HasStructure(int q, int r)
        {
            return structures.ContainsKey(new Vector2Int(q, r));
        }

        public GameObject GetStructure(int q, int r)
        {
            return structures[new Vector2Int(q, r)];
        }
    }
}
