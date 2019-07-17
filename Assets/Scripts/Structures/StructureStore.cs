using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class StructureStore : MonoBehaviour
    {
        public BoardState boardState;
        Dictionary<Vector2Int, Structure> structures = new Dictionary<Vector2Int, Structure>();

        public void AddStructure(int q, int r, GameObject structurePrefab)
        {
            Vector3 tilePos = boardState.GetNodeTile(q, r).transform.position;
            GameObject structure = Instantiate(structurePrefab, tilePos, Quaternion.identity) as GameObject;
            Structure currStructure = structure.GetComponent<Structure>();
            currStructure.boardState = this.boardState;
            currStructure.SetCoords(q, r);
            structures[new Vector2Int(q, r)] = currStructure;
        }

        public void RemoveStructure(int q, int r)
        {
            Vector2Int key = new Vector2Int(q, r);
            structures[key].Sell();
            structures.Remove(key);
        }

        public bool HasStructure(int q, int r)
        {
            return structures.ContainsKey(new Vector2Int(q, r));
        }

        public Structure GetStructure(int q, int r)
        {
            return structures[new Vector2Int(q, r)];
        }
    }
}
