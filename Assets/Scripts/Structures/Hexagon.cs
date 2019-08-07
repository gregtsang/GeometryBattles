using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using Photon.Pun;

namespace GeometryBattles.StructureManager
{
    public class Hexagon : Structure
    {
        public PhotonView photonView;

        public int baseMaxHP;
        public int baseRegen;
        public int baseArmor;
        public float buildTime;
        float timer;

        void Start()
        {
            hp = baseMaxHP;
            maxhp = baseMaxHP;
            regen = baseRegen;
            armor = baseArmor;
            timer = buildTime;
            //StartCoroutine(RegenHP());
        }

        void Update()
        {
            if (PhotonNetwork.IsMasterClient && CheckSpace())
            {
                photonView.RPC("RPB_SubTimer", RpcTarget.AllViaServer);
            }
            if (!boardState.IsGameOver() && timer <= 0.0f)
            {
                EventManager.RaiseOnGameOver(player.gameObject);
                boardState.EndGame();
            }
        }

        [PunRPC]
        void RPC_SubTimer()
        {
            timer -= Time.deltaTime;
        }

        public bool CheckSpace()
        {
            return this.CheckSpace(this.q, this.r, this.player);
        }

        public override bool CheckSpace(int q, int r, Player player)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            tiles.Add(new Vector2Int(q, r));
            tiles.Add(new Vector2Int(q - 1, r));
            tiles.Add(new Vector2Int(q + 1, r));
            tiles.Add(new Vector2Int(q, r - 1));
            tiles.Add(new Vector2Int(q, r + 1));
            tiles.Add(new Vector2Int(q - 1, r + 1));
            tiles.Add(new Vector2Int(q + 1, r - 1));
            tiles.Add(new Vector2Int(q - 1, r - 1));
            tiles.Add(new Vector2Int(q + 1, r + 1));
            tiles.Add(new Vector2Int(q - 1, r + 2));
            tiles.Add(new Vector2Int(q + 1, r - 2));
            tiles.Add(new Vector2Int(q - 2, r + 1));
            tiles.Add(new Vector2Int(q + 2, r - 1));
            foreach (Vector2Int t in tiles)
            {
                if (!boardState.IsValidTile(t[0], t[1]) || !boardState.IsOwned(t[0], t[1]) || boardState.GetNodeOwner(t[0], t[1]) != player)
                    return false;
            }
            return true;
        }

        public float GetProgress()
        {
            return 1.0f - Mathf.Max(0.0f, timer / buildTime);
        }
    }
}
