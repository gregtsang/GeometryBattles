using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.PlayerManager;
using GeometryBattles.BoardManager;

namespace GeometryBattles.HexAction
{
    public interface IHexAction
    {
        string displayName
        {
            get;
        }

        string GetTipText(Player player, Tile tile);

        void doAction(Player player, Tile tile);

        bool canDoAction(Player player, Tile tile);
        bool canDoAction(Player player, Tile tile, ref string err);

        bool isViableAction(Player player, Tile tile);
        bool isViableAction(Player player, Tile tile, ref string err);
    }
}