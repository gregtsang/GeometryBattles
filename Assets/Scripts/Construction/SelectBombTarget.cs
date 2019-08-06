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
            photonView.RPC("RPC_SetTarget", RpcTarget.AllViaServer, tile.Q, tile.R);
        }

        [PunRPC]
        private void PRC_SetTarget(int tileQ, int tileR)
        {
            selectedPentagon.SetTarget(tileQ, tileR);
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
    }
}