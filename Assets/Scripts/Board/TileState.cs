using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class TileState
    {
        public TileState() {}

        public TileState(GameObject tile, GameObject owner, int influence)
        {
            this.tile = tile;
            this.owner = owner;
            this.influence = influence;
        }

        GameObject tile;
        GameObject owner;
        int influence;
        Dictionary<GameObject, int> buff = new Dictionary<GameObject, int>();

        public GameObject GetTile()
        {
            return this.tile;
        }

        public void SetTile(GameObject tile)
        {
            this.tile = tile;
        }

        public GameObject GetOwner()
        {
            return this.owner;
        }

        public int GetInfluence()
        {
            return this.influence;
        }

        public void Set(GameObject owner, int influence, bool instant = true, bool color = true)
        {
            this.owner = owner;
            this.influence = influence;
            if (color && owner != null)
            {
                this.tile.GetComponent<TilePrefab>().SetColor(Color.HSVToRGB(owner.GetComponent<PlayerPrefab>().GetColor(), Mathf.Min(influence / 100.0f, 1.0f), 1.0f), instant);
                if (instant)
                    this.tile.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.HSVToRGB(owner.GetComponent<PlayerPrefab>().GetColor(), Mathf.Min(influence / 100.0f, 1.0f), 1.0f));
            }
        }

        public void SetBuff(GameObject player, int buff)
        {
            this.buff[player] = buff;
        }

        public int GetBuff(GameObject player)
        {
            return this.buff.ContainsKey(player) ? buff[player] : 0;
        }
    }
}