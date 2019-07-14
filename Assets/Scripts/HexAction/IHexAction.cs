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

        void doAction(PlayerPrefab player, TilePrefab tile);

        bool canDoAction(PlayerPrefab player, TilePrefab tile);
        bool canDoAction(PlayerPrefab player, TilePrefab tile, ref string err);

        bool isViableAction(PlayerPrefab player, TilePrefab tile);
        bool isViableAction(PlayerPrefab player, TilePrefab tile, ref string err);
    }
}