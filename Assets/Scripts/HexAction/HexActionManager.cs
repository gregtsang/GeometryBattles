﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.HexAction
{
    static public class HexActionManager
    {
        static List<IHexAction> hexActions = new List<IHexAction>();
        
        static public void registerAction(IHexAction action)
        {
            hexActions.Add(action);
        }

        static public List<IHexAction> getViableActions(PlayerPrefab player, TilePrefab tile)
        {
            List<IHexAction> viableActions = new List<IHexAction>();
            string errMsg = "";
            foreach (IHexAction action in hexActions)
            {
                if (action.isViableAction(player, tile, ref errMsg))
                {
                    viableActions.Add(action);
                }
            }
            return viableActions;
        }
    }
}