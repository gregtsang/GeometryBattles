using System.Collections;
using System.Collections.Generic;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using GeometryBattles.StructureManager;
using UnityEngine;

namespace GeometryBattles.HexAction
{
    public class SelectBombTarget : MonoBehaviour, IHexAction
    {
        string _displayName = "Select Target";
        public string displayName  {get => _displayName; }

        Pentagon selectedPentagon = null;

        [SerializeField] SelectAHex selectionManager = null;
        StructureStore structureStore = null;

        public bool canDoAction(Player player, Tile tile)
        {
            return true;
        }

        public bool canDoAction(Player player, Tile tile, ref string err)
        {
            return true;
        }

        public void doAction(Player player, Tile tile)
        {
            if (structureStore.HasStructure(tile.Q, tile.R))
            {
                Structure structure = structureStore.GetStructure(tile.Q, tile.R);
                if (structure is Pentagon)
                {
                    selectedPentagon = (Pentagon) structure;

                    selectionManager.SelectHex(OnHexSelection);
                    return;
                }
            }
            Debug.Log("No Pentagon here");
        }

        public string GetTipText(Player player, Tile tile)
        {
            return "Target";
        }

        public bool isViableAction(Player player, Tile tile)
        {
            return true;
        }

        public bool isViableAction(Player player, Tile tile, ref string err)
        {
            return true;
        }

        private void OnHexSelection(Tile tile)
        {
            Debug.Log($"tile selected: {tile.Q}, {tile.R}");
            selectedPentagon.SetTarget(tile.Q, tile.R);
        }

        // Start is called before the first frame update
        void Start()
        {
            structureStore = FindObjectOfType<StructureStore>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}