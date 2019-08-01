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
            EventManager.onStructureDamage += DamageStructure;
        }

        void DamageStructure(int q, int r, int amount)
        {
            Structure currStructure = structures[new Vector2Int(q, r)];
            int currHP = currStructure.GetHP() - Mathf.Max(1, amount - currStructure.GetArmor());
            if (currHP > 0)
            {
                currStructure.SetHP(currHP);
            }
            else
            {
                RemoveStructure(q, r);
            }
        }

        public void AddStructure(int q, int r, GameObject structurePrefab)
        {
            Tile currTile = boardState.GetNodeTile(q, r);
            Vector3 pos = currTile.transform.position + new Vector3(0.0f, structurePrefab.GetComponent<MeshRenderer>().bounds.size.y / 2.0f, 0.0f);
            GameObject structure = Instantiate(structurePrefab, pos, structurePrefab.transform.rotation) as GameObject;
            Structure currStructure = structure.GetComponent<Structure>();
            currStructure.SetColor(boardState.GetNodeOwner(q, r).GetColor());
            currStructure.SetCoords(q, r);
            currStructure.SetPlayer(boardState.GetNodeOwner(q, r));
            currStructure.boardState = this.boardState;
            structures[new Vector2Int(q, r)] = currStructure;
            boardState.AddStructure(q, r);
            StartCoroutine(DissolveIn(currStructure));
        }

        IEnumerator DissolveIn(Structure structure)
        {
            float dissolveRate = 5.0f;
            float dissolveTimer = dissolveRate;
            float height = structure.gameObject.GetComponent<MeshRenderer>().bounds.size.y;
            while (structure.Mat.GetFloat("_Glow") != 1.0f)
            {
                dissolveTimer -= Time.deltaTime;
                structure.Mat.SetFloat("_Glow", 1.0f - Mathf.Max(dissolveTimer, 0.0f) / dissolveRate);
                structure.Mat.SetFloat("_Level", height / 2.0f - (height + 0.65f) * (Mathf.Max(dissolveTimer, 0.0f) / dissolveRate));
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
            boardState.RemoveStructure(q, r);
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
