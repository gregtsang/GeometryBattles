using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryBattles.BoardManager;
using GeometryBattles.PlayerManager;
using System;

namespace GeometryBattles.HexAction
{
    public class SelectionAction : MonoBehaviour, IHexAction
    {
        string _displayname = "Selection";
        public string displayName { get => _displayname; }

        public event EventHandler<TileSelectionEventArgs> TileSelected;


        public bool canDoAction(Player player, Tile tile)
        {
            return true;
        }

        public bool canDoAction(Player player, Tile tile, ref string err)
        {
            return true;
        }

        public void doAction(Player player, Tile tile)
        {
            TileSelectionEventArgs e = new TileSelectionEventArgs();
            e.tile = tile;
            OnTileSelected(e);
        }

        public string GetTipText(Player player, Tile tile)
        {
            return "X";
        }

        public bool isViableAction(Player player, Tile tile)
        {
            return true;
        }

        public bool isViableAction(Player player, Tile tile, ref string err)
        {
            return true;
        }

        private void OnTileSelected(TileSelectionEventArgs e)
        {
            EventHandler<TileSelectionEventArgs> handler = TileSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class TileSelectionEventArgs : EventArgs
    {
        public Tile tile { get; set; }
    }
}