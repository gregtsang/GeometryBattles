using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using Photon.Pun;

namespace GeometryBattles.StructureManager
{
    public class Hexagon : Structure
    {
        public PhotonView photonView;

        float buildTime;
        float timer;

        public HexagonData stats;

        void OnEnable()
        {
            stats = this.gameObject.GetComponent<HexagonData>();
        }

        void Start()
        {
            hp = stats.currLevel.maxHP;
            maxhp = stats.currLevel.maxHP;
            regen = stats.currLevel.regen;
            armor = stats.currLevel.armor;
            timer = stats.currLevel.buildTime;
            buildTime = timer;
        }

        void Update()
        {
            if (PhotonNetwork.IsMasterClient && CheckSpace())
            {
                photonView.RPC("RPC_SubTimer", RpcTarget.AllViaServer);
            }
            if (!boardState.IsGameOver() && timer <= 0.0f && PhotonNetwork.IsMasterClient)
            {
                boardState.photonView.RPC("RPC_EndGame", RpcTarget.All, (byte) player.Id);
            }
        }

        public override void StartEffect()
        {
            Color color = boardState.GetNodeOwner(q, r).GetColor();
            color.a = 0.2f;
            gameObject.GetComponent<MeshRenderer>().materials[1].SetColor("_BaseColor", color);
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", boardState.GetNodeOwner(q, r).GetColor());
            gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_Glow", 0.0f);
            gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_Level", -0.65f);
            StartCoroutine(DissolveIn());
        }

        IEnumerator DissolveIn()
        {
            float height = gameObject.GetComponent<MeshRenderer>().bounds.size.y;
            MeshRenderer dissolveRend = gameObject.GetComponent<MeshRenderer>();
            while (dissolveRend.materials[0].GetFloat("_Glow") != 1.0f)
            {
                Debug.Log(dissolveRend.materials[0].GetFloat("_Glow"));
                dissolveRend.materials[0].SetFloat("_Glow", 1.0f - Mathf.Max(timer, 0.0f) / buildTime);
                dissolveRend.materials[0].SetFloat("_Level", height - (height + 0.65f) * (Mathf.Max(timer, 0.0f) / buildTime));
                yield return null;
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
