using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.BoardManager
{
    public class TileState
    {
        public TileState() {}

        public TileState(Tile tile)
        {
            this.tile = tile;
            this.owner = null;
            this.influence = 0;
            this.shield = 0;
        }

        Tile tile;
        Player owner;
        int influence;
        int shield;

        Dictionary<Player, int> buff = new Dictionary<Player, int>();

        public Tile GetTile()
        {
            return tile;
        }

        public Player GetOwner()
        {
            return owner;
        }

        public int GetInfluence()
        {
            return influence;
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;
        }

        public void Set(Player owner, int influence)
        {
            this.owner = owner;
            this.influence = influence;
        }

        public void SetColor(Player owner, int influence, int threshold, bool instant = true)
        {
            Color baseColor = tile.GetBaseColor();
            Color color = Color.Lerp(baseColor, owner ? owner.GetColor() : baseColor, Mathf.Min((float)influence / (float)threshold, 1.0f));
            if (instant)
            {
                tile.SetPrevColor(color);
                tile.SetNextColor(color);
            }
            else
            {
                if (color != tile.GetNextColor())
                {
                    tile.SetPrevColor(tile.GetMat().GetColor("_BaseColor"));
                    tile.SetNextColor(color);
                }
            }
        }

        public int GetBuff(Player player)
        {
            return buff.ContainsKey(player) ? buff[player] : 0;
        }

        public void SetBuff(Player player, int buff)
        {
            this.buff[player] = this.buff.ContainsKey(player) ? Mathf.Max(buff, this.buff[player]) : buff;
        }

        public int GetShield()
        {
            return shield;
        }

        public void SetShield(int hp)
        {
            shield = hp;
        }

        public void AddShield(int amount, int max)
        {
            shield = Mathf.Min(shield + amount, max);
        }

        public void SubShield(int amount)
        {
            shield = Mathf.Max(shield - amount, 0);
        }
    }
}