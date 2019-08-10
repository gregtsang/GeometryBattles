using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GeometryBattles.BoardManager;

namespace GeometryBattles.UI
{
    public class ObjectHighlight : MonoBehaviour
    {
        [SerializeField] UIManager uiManager = null;
        
        Tile selectedTile;
        
        private void Start()
        {
            uiManager.HexSelectionManager.HexActionMenuShown += OnSelect;
            uiManager.HexSelectionManager.HexActionMenuHide += OnDeselect;
            uiManager.HexSelectionManager.TileMouseEntered += OnMouseEntered;
            uiManager.HexSelectionManager.TileMouseExited += OnMouseExited;
        }

        private void OnMouseEntered(object sender, TileEventArgs e)
        {
            foreach (Tile tile in GetAllTiles(e.tile, uiManager.HexSelectionManager.HoverHighlightSize))
            {
                if (tile != selectedTile)
                {
                    SetHoverColor(tile);
                }
            }
        }

        private void OnMouseExited(object sender, TileEventArgs e)
        {
            if (e.tile != selectedTile)
            {
                RemoveColor(e.tile);
            }
        }

        private void OnSelect(object sender, TileEventArgs e)
        {
            selectedTile = e.tile;
            SetSelectedColor(selectedTile);
        }

        private void OnDeselect(object sender, EventArgs e)
        {
            RemoveColor(selectedTile);
            selectedTile = null;
        }

        private void SetSelectedColor(Tile tile)
        {
            Material material = tile.GetComponent<Renderer>().material;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", material.GetColor("_BaseColor") * 2);
        }

        private void SetHoverColor(Tile tile)
        {
            Material material = tile.GetComponent<Renderer>().material;
            Color emissionColor = uiManager.HexSelectionManager.HoverHighlightColor;

            material.EnableKeyword("_EMISSION");
            if (emissionColor == Color.clear)
            {
                emissionColor = material.GetColor("_BaseColor");
            }
            material.SetColor("_EmissionColor", emissionColor);
        }

        private void RemoveColor(Tile tile)
        {
            Material material = tile.GetComponent<Renderer>().material;
            material.DisableKeyword("_EMISSION");
        }

        private List<Tile> GetAllTiles(Tile centerTile, int count)
        {
            BoardState boardState = uiManager.GetBoard().boardState;

            var tileList = new List<Tile>();
            tileList.Add(centerTile);

            if (count > 1)
            {
                foreach (Vector2Int tileVector in boardState.GetNeighbors(centerTile.Q, centerTile.R))
                {
                    tileList.Add(boardState.GetNodeTile(tileVector[0], tileVector[1]));
                }
            }
            return tileList;
        }
    }
}