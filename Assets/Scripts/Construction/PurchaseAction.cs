using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;

namespace GeometryBattles.Construction
{
    public class PurchaseAction : MonoBehaviour, IHexAction
    {
        [SerializeField] private string _displayname = "Buy Hex";
        [SerializeField] private int costPerDistance = 10;

        //Cached References
        Board board;

        public string displayName { get => _displayname; }

        private void Start()
        {
            HexActionManager.registerAction(this);
            board = FindObjectOfType<Board>();
        }

        public void doAction(Player player, Tile tile)
        {
            if (canDoAction(player, tile))
            {
                player.AddResource(-1 * GetTileCost(player, tile));                
                board.boardState.SetNode(tile.Q, tile.R, player);
            }
        }

        public bool canDoAction(Player player, Tile tile)
        {
            string errRef = "";
            return canDoAction(player, tile, ref errRef);
        }

        public bool canDoAction(Player player, Tile tile, ref string err)
        {
            bool canDo = isViableAction(player, tile, ref err);
            if (canDo)
            {
                canDo = player.GetResource() >= GetTileCost(player, tile);
                if (!canDo) err = "Tile too expensive.";
            }
            return canDo;
        }

        public bool isViableAction(Player player, Tile tile)
        {
            string errRef = "";
            return isViableAction(player, tile, ref errRef);
        }

        public bool isViableAction(Player player, Tile tile, ref string err)
        {   
            bool isViable = !board.boardState.IsOwned(tile.Q, tile.R);
            if (!isViable)
            {
                err = "Cannot buy a tile that is currently owned.";
            }
            return isViable;
        }

        private int GetTileCost(Player player, Tile tile)
        {
            int cost = board.boardState.ClosestOwned(tile.Q, tile.R, player) * costPerDistance;
            Debug.Log("Tile Cost Calculated: " + cost);
            return cost;
        }

        public string GetTipText(Player player, Tile tile)
        {
            return isViableAction(player, tile) ? GetTileCost(player, tile).ToString() : "";
        }
    }
}