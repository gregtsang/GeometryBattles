using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;

namespace GeometryBattles.StructureManager
{
    public class StructureStore : MonoBehaviour
    {
        public BoardState boardState;
        Dictionary<Vector2Int, Structure> structures = new Dictionary<Vector2Int, Structure>();

        void Start()
        {
            boardState = GameObject.FindObjectOfType<BoardState>();
        }

        void Update()
        {
            List<Structure> destroy = new List<Structure>();
            foreach(var s in structures)
            {
                s.Value.SetHP(boardState.GetNodeShield(s.Key[0], s.Key[1]));
                if (s.Value.GetHP() <= 0)
                    destroy.Add(s.Value);
                else
                    boardState.AddNodeShield(s.Key[0], s.Key[1], s.Value.GetHPRegen(), s.Value.GetMaxHP());
            }
            foreach (var d in destroy)
                RemoveStructure(d.Q, d.R);
        }

        public void AddStructure(int q, int r, GameObject structurePrefab)
        {
            Tile currTile = boardState.GetNodeTile(q, r);
            Vector3 pos = currTile.transform.position + new Vector3(0.0f, structurePrefab.GetComponent<MeshRenderer>().bounds.size.y / 2.0f, 0.0f);
            GameObject structure = Instantiate(structurePrefab, pos, structurePrefab.transform.rotation) as GameObject;
            Structure currStructure = structure.GetComponent<Structure>();
            currStructure.SetColor(boardState.GetNodeOwner(q, r).GetColor());
            currStructure.boardState = this.boardState;
            currStructure.SetCoords(q, r);
            currStructure.SetPlayer(boardState.GetNodeOwner(q, r));
            boardState.AddNodeShield(q, r, currStructure.GetMaxHP(), currStructure.GetMaxHP());
            structures[new Vector2Int(q, r)] = currStructure;
            StartCoroutine(DissolveIn(currStructure));
        }

        IEnumerator DissolveIn(Structure structure)
        {
            float dissolveRate = 5.0f;
            float dissolveTimer = dissolveRate;
            while (structure.Mat.GetFloat("_Glow") != 1.0f)
            {
                dissolveTimer -= Time.deltaTime;
                structure.Mat.SetFloat("_Glow", 1.0f - Mathf.Max(dissolveTimer, 0.0f) / dissolveRate);
                structure.Mat.SetFloat("_Level", 0.5f - 1.65f * (Mathf.Max(dissolveTimer, 0.0f) / dissolveRate));
                structure.SetHP((int)(structure.GetMaxHP() * (1.0f - Mathf.Max(dissolveTimer, 0.0f) / dissolveRate)));
                yield return null;
            }
            structure.StartEffect();
        }

        public void RemoveStructure(int q, int r)
        {
            Vector2Int key = new Vector2Int(q, r);
            structures[key].Destroy();
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
