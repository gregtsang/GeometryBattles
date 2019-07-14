﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.HexAction;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.UI
{
    [RequireComponent(typeof(TilePrefab))]
    public class HexActionMenu : MonoBehaviour
    {
        
        //Cached References
        UIManager uiManager;

        TilePrefab tilePrefab;
        
        // Start is called before the first frame update
        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
            tilePrefab = GetComponent<TilePrefab>();
        }
        
        private void OnMouseDown()
        {
            string errMsg = "";
            var actions = new List<IHexAction>();
            actions = HexActionManager.getViableActions(uiManager.GetActivePlayer(), tilePrefab);
            //DebugLogActions(actions);

            if (actions.Count == 1)
            {
                if (actions[0].canDoAction(uiManager.GetActivePlayer(), tilePrefab, ref errMsg))
                {
                    actions[0].doAction(uiManager.GetActivePlayer(), tilePrefab);
                }
                else
                {
                    Debug.Log(errMsg);
                }
            }

        }
        
        private void DebugLogActions(List<IHexAction> actions)
        {
            string errMsg = "";
            foreach (IHexAction action in actions)
            {
                Debug.Log(action.displayName);
                if (!action.canDoAction(uiManager.GetActivePlayer(), tilePrefab, ref errMsg))
                {
                    Debug.Log(errMsg);
                }
            }
        }

    }
}