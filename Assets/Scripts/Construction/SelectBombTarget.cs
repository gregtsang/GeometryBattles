using System;
using System.Collections;
using System.Collections.Generic;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using GeometryBattles.StructureManager;
using UnityEngine;
using Photon.Pun;

namespace GeometryBattles.HexAction
{
    public class SelectBombTarget : MonoBehaviour, IHexAction
    {
        string _displayName = "Select Target";
        public string displayName  {get => _displayName; }

        Pentagon selectedPentagon = null;

        [SerializeField] SelectAHex selectionManager = null;
        StructureStore structureStore = null;
        
        Board board;
        PhotonView photonView;

        public event EventHandler<BombSelectionEnteredEventArgs> BombSelectionEntered;
        public event EventHandler BombSelectionLeft;


        // Start is called before the first frame update
        void Start()
        {
            board = FindObjectOfType<Board>();
            structureStore = FindObjectOfType<StructureStore>();
            photonView = GetComponent<PhotonView>();
        }

        public bool canDoAction(Player player, Tile tile)
        {
            string err = "";
            return canDoAction(player, tile, ref err);
        }

        public bool canDoAction(Player player, Tile tile, ref string err)
        {
            return isViableAction(player, tile, ref err);
        }

        public void doAction(Player player, Tile tile)
        {
            
            if (canDoAction(player, tile))
            {
                Structure structure = structureStore.GetStructure(tile.Q, tile.R);
                selectedPentagon = (Pentagon) structure;

                selectionManager.SelectHex(OnHexSelection);

                var e = new BombSelectionEnteredEventArgs();
                e.tile = tile;
                OnBombSelectionEntered(e);

                return;
            }
        }

        public string GetTipText(Player player, Tile tile)
        {
            return "";
        }

        public bool isViableAction(Player player, Tile tile)
        {
            string err = "";
            return isViableAction(player, tile, ref err);
        }

        public bool isViableAction(Player player, Tile tile, ref string err)
        {
            if (TileContainsPentagon(tile))
            {
                if (board.boardState.IsOwned(tile.Q, tile.R) && 
                    board.boardState.GetNodeOwner(tile.Q, tile.R) == player)
                {
                    return true;
                }
                else
                {
                    err = "Tile not Owned";
                }
            }
            else
            {
                err = "No Pentagon here";
            }
            return false;
        }

        private void OnHexSelection(Tile tile)
        {
            Debug.Log($"tile selected: {tile.Q}, {tile.R}");
            OnBombSelectionLeft();
            photonView.RPC("RPC_SetTarget", RpcTarget.AllViaServer, selectedPentagon.Q, selectedPentagon.R, tile.Q, tile.R);
        }

        [PunRPC]
        private void RPC_SetTarget(int structureQ, int structureR, int tileQ, int tileR)
        {
            if (TileContainsPentagon(board.boardState.GetNodeTile(structureQ, structureR)))
            {
                Structure structure = structureStore.GetStructure(structureQ, structureR);
                Pentagon pentagon = (Pentagon) structure;
                pentagon.SetTarget(tileQ, tileR);
            }
        }

        private bool TileContainsPentagon(Tile tile)
        {
            if (structureStore.HasStructure(tile.Q, tile.R))
            {
                Structure structure = structureStore.GetStructure(tile.Q, tile.R);
                if (structure is Pentagon)
                {
                    return true;
                }
            }
            return false;
        }

        //Event Handlers
        private void OnBombSelectionEntered(BombSelectionEnteredEventArgs e)
        {
            var handler = BombSelectionEntered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnBombSelectionLeft()
        {
            var handler = BombSelectionLeft;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }

    public class BombSelectionEnteredEventArgs : EventArgs
    {
        public Tile tile { get; set; }
    }
}