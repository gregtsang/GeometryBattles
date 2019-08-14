using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GeometryBattles.BoardManager;

namespace GeometryBattles.UI
{
    public class HexSelectionManager : MonoBehaviour
    {
        private GameObject buttonGroup;
        [SerializeField] GameObject hexActionCanvasPrefab = null;
        [SerializeField] GameObject hexActionOptionPrefab = null;

        int hoverHighlightSize = 1;
        Color hoverHighlightColor = Color.clear;

        public int HoverHighlightSize { get => hoverHighlightSize; set { hoverHighlightSize = value; OnSelectionSettingsChanged(); } }
        public Color HoverHighlightColor { get => hoverHighlightColor; set => hoverHighlightColor = value; }

        public event EventHandler<TileEventArgs> HexActionMenuShown;
        public event EventHandler HexActionMenuHide;

        public event EventHandler<TileEventArgs> TileMouseEntered;
        public event EventHandler<TileEventArgs> TileMouseExited;
        public event EventHandler SelectionSettingsChanged;

        private void Awake()
        {
            CreateHexActionMenuPrefab();
        }
        
        public void ShowHexActionMenu(Tile tile)
        {
            buttonGroup.transform.position = Input.mousePosition;
            buttonGroup.SetActive(true);
            TileEventArgs e = new TileEventArgs();
            e.tile = tile;
            OnHexActionMenuShown(e);
        }

        public void HideHexActionMenu()
        {
            buttonGroup.SetActive(false);
            OnHexActionMenuHide();
        }

        public void InitializeHexActionMenu()
        {
            foreach (Transform child in buttonGroup.gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void MouseEnteredTile(Tile tile)
        {
            TileEventArgs e = new TileEventArgs();
            e.tile = tile;
            OnTileMouseEntered(e);
        }

        public void MouseExitedTile(Tile tile)
        {
            TileEventArgs e = new TileEventArgs();
            e.tile = tile;
            OnTileMouseExited(e);
        }

        public Button AddActionToHexActionMenu(string actionText)
        {
            GameObject newButton = Instantiate(hexActionOptionPrefab);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = actionText;
            newButton.transform.SetParent(buttonGroup.gameObject.transform);
            return newButton.GetComponent<Button>();
        }
        
        private void CreateHexActionMenuPrefab()
        {
            GameObject newCanvas = Instantiate(hexActionCanvasPrefab);
            newCanvas.transform.SetParent(gameObject.transform);
            buttonGroup = newCanvas.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
            HideHexActionMenu();
        }

        private void OnGUI()
        {
            if (Input.GetMouseButtonDown(1))
            {
                HideHexActionMenu();
            }
        }

        //Event Handlers
        private void OnHexActionMenuShown(TileEventArgs e)
        {
            EventHandler<TileEventArgs> handler = HexActionMenuShown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnHexActionMenuHide()
        {
            EventHandler handler = HexActionMenuHide;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void OnTileMouseEntered(TileEventArgs e)
        {
            EventHandler<TileEventArgs> handler = TileMouseEntered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnTileMouseExited(TileEventArgs e)
        {
            EventHandler<TileEventArgs> handler = TileMouseExited;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnSelectionSettingsChanged()
        {
            var handler = SelectionSettingsChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }

    public class TileEventArgs : EventArgs
    {
        public Tile tile { get; set; }
    }
}