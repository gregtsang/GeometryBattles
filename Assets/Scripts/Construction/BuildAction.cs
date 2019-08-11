using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using GeometryBattles.StructureManager;
using Photon.Pun;

namespace GeometryBattles.Construction
{
    public class BuildAction : MonoBehaviour, IHexAction
    {
        [SerializeField] string _displayname = "Build ____";
        [SerializeField] StructureStore.StructureType structureType = 0;

        //Cached References
        StructureStore structureStore;
        Board board;
        PhotonView photonView;

        public string displayName { get => _displayname; }

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
                photonView.RPC("RPC_BuildStructure", RpcTarget.AllViaServer, (byte) player.Id, tile.Q, tile.R, (byte) structureType);
            }
        }

        [PunRPC]
        private void RPC_BuildStructure(byte playerID, int tileQ, int tileR, byte structureType)
        {
            Player player = board.boardState.GetPlayer((int) playerID);
            GameObject structurePrefab = structureStore.GetStructurePrefab((StructureStore.StructureType) structureType);
            if (canDoAction(player, board.boardState.GetNodeTile(tileQ, tileR)))
            {
                player.AddResource(-1 * GetStructureCost());
                structureStore.AddStructure(tileQ, tileR, structurePrefab);
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
                err = "Cannot construct on this tile";
                return false;
            }
        }

        private int GetStructureCost()
        {
            return structureStore.GetStructurePrefab(structureType).GetComponent<IStructureData>().GetCost();
        }
    }
}