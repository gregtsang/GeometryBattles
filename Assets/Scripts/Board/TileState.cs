using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class TileState
    {
        public TileState() {}

        public TileState(Tile tile, Player owner, int influence)
        {
            this.tile = tile;
            this.owner = owner;
            this.influence = influence;
        }

        Tile tile;
        Player owner;
        int influence;
        Dictionary<Player, int> buff = new Dictionary<Player, int>();

        public Tile GetTile()
        {
            return tile;
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;
        }

        public Player GetOwner()
        {
            return owner;
        }

        public int GetInfluence()
        {
            return influence;
        }

        public void Set(Player owner, int influence)
        {
            this.owner = owner;
            this.influence = influence;
        }

        public void SetColor(Player owner, int influence, int threshold, bool instant = true)
        {
            Color color = Color.HSVToRGB(owner ? owner.GetColor() : 0.0f, Mathf.Min((float)influence / (float)threshold, 1.0f), 1.0f);
            if (instant)
                tile.SetPrevColor(color);
            else
                tile.SetPrevColor(tile.GetMat().GetColor("_BaseColor"));
            tile.SetNextColor(color);
        }

        public void SetBuff(Player player, int buff)
        {
            this.buff[player] = buff;
        }

        public int GetBuff(Player player)
        {
            return buff.ContainsKey(player) ? buff[player] : 0;
        }
    }
}