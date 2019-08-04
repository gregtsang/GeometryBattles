using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using GeometryBattles.StructureManager;

namespace GeometryBattles.Construction
{
    public class BuildAction : MonoBehaviour, IHexAction
    {
        [SerializeField] string _displayname = "Build ____";
        [SerializeField] GameObject structurePrefab = null;

        StructureStore structureStore;
        Board board;

        public string displayName { get => _displayname; }

        // Start is called before the first frame update
        void Start()
        {
            board = FindObjectOfType<Board>();
            structureStore = FindObjectOfType<StructureStore>();
            HexActionManager.registerAction(this);
        }

        public bool canDoAction(Player player, Tile tile)
        {
            string err = "";
            return canDoAction(player, tile, ref err);
        }

        public bool canDoAction(Player player, Tile tile, ref string err)
        {
            if (!isViableAction(player, tile, ref err))
            {
                return false;
            }
            else
            {
                if (player.GetResource() >= GetStructureCost())
                {
                    return true;
                }
                else
                {
                    err = "Cannot afford Structure";
                    return false;
                }
            }
        }

        public void doAction(Player player, Tile tile)
        {
            if (canDoAction(player, tile))
            {
                player.AddResource(-1 * GetStructureCost());
                structureStore.AddStructure(tile.Q, tile.R, structurePrefab);
            }
        }

        public string GetTipText(Player player, Tile tile)
        {
            return "-" + GetStructureCost().ToString();
        }

        public bool isViableAction(Player player, Tile tile)
        {
            string err = "";
            return isViableAction(player, tile, ref err);
        }

        public bool isViableAction(Player player, Tile tile, ref string err)
        {
            if (board.boardState.GetNodeOwner(tile.Q, tile.R) == player && 
                board.boardState.IsOwned(tile.Q, tile.R) &&
                !structureStore.HasStructure(tile.Q, tile.R))
            {
                return true;
            }
            else
            {
                err = "Cannot construct on a tile you do not own";
                return false;
            }
        }

        private int GetStructureCost()
        {
            return structurePrefab.GetComponent<IStructureData>().GetCost();
        }
    }
}