using System;
using System.Collections;
using System.Collections.Generic;
using GeometryBattles.BoardManager;
using UnityEngine;

namespace GeometryBattles.HexAction
{    
    public class SelectAHex : MonoBehaviour
    {
        [SerializeField] SelectionAction selectionAction = null;
        [SerializeField] HexActionMode selectionActionMode = null;
        [SerializeField] HexActionModeManager hexActionModeManager = null;

        Action<Tile> registeredCallback;

        public void SelectHex(Action<Tile> callback)
        {
            selectionAction.TileSelected += OnHexSelected;
            hexActionModeManager.EnterMode(selectionActionMode.ModeName);
            registeredCallback = callback;
        }

        private void OnHexSelected(object sender, TileSelectionEventArgs e)
        {
            registeredCallback(e.tile);
            hexActionModeManager.ReturnToPrevMode();
            selectionAction.TileSelected -= OnHexSelected;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}