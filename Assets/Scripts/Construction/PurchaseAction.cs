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

        public void doAction(PlayerPrefab player, TilePrefab tile)
        {
            if (canDoAction(player, tile))
            {
                player.AddResource(-1 * GetTileCost(player, tile));                
                board.boardState.SetNode(tile.Q, tile.R, player.gameObject);
            }
        }

        public bool canDoAction(PlayerPrefab player, TilePrefab tile)
        {
            string errRef = "";
            return canDoAction(player, tile, ref errRef);
        }

        public bool canDoAction(PlayerPrefab player, TilePrefab tile, ref string err)
        {
            bool canDo = isViableAction(player, tile, ref err);
            if (canDo)
            {
                canDo = player.GetResource() >= GetTileCost(player, tile);
                if (!canDo) err = "Tile too expensive.";
            }
            return canDo;
        }

        public bool isViableAction(PlayerPrefab player, TilePrefab tile)
        {
            string errRef = "";
            return isViableAction(player, tile, ref errRef);
        }

        public bool isViableAction(PlayerPrefab player, TilePrefab tile, ref string err)
        {   
            bool isViable = !board.boardState.IsOwned(tile.Q, tile.R);
            if (!isViable)
            {
                err = "Cannot buy a tile that is currently owned.";
            }
            return isViable;
        }

        private int GetTileCost(PlayerPrefab player, TilePrefab tile)
        {
            int cost = board.boardState.ClosestOwned(tile.Q, tile.R, player.gameObject) * costPerDistance;
            Debug.Log("Tile Cost Calculated: " + cost);
            return cost;
        }

        public string GetTipText(PlayerPrefab player, TilePrefab tile)
        {
            return isViableAction(player, tile) ? GetTileCost(player, tile).ToString() : "";
        }
    }
}