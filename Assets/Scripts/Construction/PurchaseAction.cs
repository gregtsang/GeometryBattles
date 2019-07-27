using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;
using Photon.Pun;

namespace GeometryBattles.Construction
{
    public class PurchaseAction : MonoBehaviour, IHexAction
    {
        [SerializeField] private string _displayname = "Buy Hex";
        [SerializeField] private int costPerDistance = 10;
        [SerializeField] private int maxCost = 999;

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
            PhotonView pv = GetComponent<PhotonView>();
            int pid = player.Id;
            int q = tile.Q;
            int r = tile.R;

            pv.RPC("RPC_PurchaseTile", RpcTarget.AllViaServer, player, tile);
        }

        [PunRPC]
        private void RPC_PurchaseTile(int pid, int q, int r)
        {
            Player player = board.boardState.GetPlayer(pid);
            Tile tile = board.boardState.GetNodeTile(q, r);

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
            int closest = board.boardState.ClosestOwned(tile.Q, tile.R, player);
            return closest != -1 ? Mathf.Min(maxCost, closest * costPerDistance) : maxCost;
        }

        public string GetTipText(Player player, Tile tile)
        {
            return isViableAction(player, tile) ? GetTileCost(player, tile).ToString() : "";
        }
    }
}