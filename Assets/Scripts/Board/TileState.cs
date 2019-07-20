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
            this.structureHP = 0;
        }

        Tile tile;
        Player owner;
        int influence;
        int structureHP;

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

        public void SetColor(Player owner, int influence, int threshold, Color baseColor, bool instant = true)
        {
            Color color = Color.Lerp(baseColor, owner ? owner.GetColor() : baseColor, Mathf.Min((float)influence / (float)threshold, 1.0f));
            if (instant)
                tile.SetPrevColor(color);
            else
                tile.SetPrevColor(tile.GetMat().GetColor("_BaseColor"));
            tile.SetNextColor(color);
        }

        public void SetBuff(Player player, int buff)
        {
            this.buff[player] = this.buff.ContainsKey(player) ? Mathf.Max(buff, this.buff[player]) : buff;
        }

        public int GetBuff(Player player)
        {
            return buff.ContainsKey(player) ? buff[player] : 0;
        }

        public void SetStructureHP(int hp)
        {
            structureHP = hp;
        }

        public void DecStructureHP(int amount)
        {
            structureHP = Mathf.Max(structureHP - amount, 0);
        }

        public void AddStructureHP(int amount, int max)
        {
            structureHP = Mathf.Min(structureHP + amount, max);
        }

        public int GetStructureHP()
        {
            return structureHP;
        }
    }
}