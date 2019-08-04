using System.Collections;
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

        static public List<IHexAction> getViableActions(Player player, Tile tile)
        {
            List<IHexAction> viableActions = new List<IHexAction>();
            foreach (IHexAction action in hexActions)
            {
                if (action.isViableAction(player, tile))
                {
                    viableActions.Add(action);
                }
            }
            return viableActions;
        }
    }
}