using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.Construction
{
    public class SellAction : MonoBehaviour, IHexAction
    {
        [SerializeField] string _displayName = "Sell Tile";
        [SerializeField] private float valueFactor = 0.5f;

        //Cached References
        Board board;

        public string displayName {get => _displayName; }

        // Start is called before the first frame update
        void Start()
        {
            HexActionManager.registerAction(this);
            board = FindObjectOfType<Board>();
        }

        public bool canDoAction(PlayerPrefab player, TilePrefab tile)
        {
            return isViableAction(player, tile);
        }

        public bool canDoAction(PlayerPrefab player, TilePrefab tile, ref string err)
        {
            return isViableAction(player, tile, ref err);
        }

        public void doAction(PlayerPrefab player, TilePrefab tile)
        {
            if (canDoAction(player, tile))
            {
                player.AddResource(GetTileValue(tile));
                board.boardState.SetNode(tile.Q, tile.R, null, 0);
            }
        }

        public string GetTipText(PlayerPrefab player, TilePrefab tile)
        {
            return GetTileValue(tile).ToString();
        }

        public bool isViableAction(PlayerPrefab player, TilePrefab tile)
        {
            string err = "";
            return isViableAction(player, tile, ref err);
        }

        public bool isViableAction(PlayerPrefab player, TilePrefab tile, ref string err)
        {
            bool isViable = board.boardState.IsOwned(tile.Q, tile.R) && 
                board.boardState.GetNodeOwner(tile.Q, tile.R) == player.gameObject;
            
            if (!isViable)
            {
                err = "Cannot sell a tile that you do not own.";
            }
            return isViable;
        }

        private int GetTileValue(TilePrefab tile)
        {
            return Mathf.RoundToInt(board.boardState.GetNodeInfluence(tile.Q, tile.R) * valueFactor);
        }
    }
}