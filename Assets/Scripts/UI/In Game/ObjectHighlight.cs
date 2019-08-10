using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GeometryBattles.UI
{
    public class ObjectHighlight : MonoBehaviour
    {
        [SerializeField] UIManager uiManager = null;
        
        bool selected = false;
        Material selectedMaterial;
        
        private void Start()
        {
            uiManager.HexSelectionManager.HexActionMenuShown += OnSelect;
            uiManager.HexSelectionManager.HexActionMenuHide += OnDeselect;
            uiManager.HexSelectionManager.TileMouseEntered += OnMouseEntered;
            uiManager.HexSelectionManager.TileMouseExited += OnMouseExited;
        }

        private void OnMouseEntered(object sender, TileEventArgs e)
        {
            Material material = e.tile.GetComponent<Renderer>().material;
            if (material != selectedMaterial)
            {
                SetHoverColor(material);
            }
        }

        private void OnMouseExited(object sender, TileEventArgs e)
        {
            Material material = e.tile.GetComponent<Renderer>().material;
            if (material != selectedMaterial)
            {
                RemoveColor(material);
            }
        }

        private void OnSelect(object sender, TileEventArgs e)
        {
            selected = true;
            selectedMaterial = e.tile.GetComponent<Renderer>().material;
            SetSelectedColor(selectedMaterial);
        }

        private void OnDeselect(object sender, EventArgs e)
        {
            selected = false;
            RemoveColor(selectedMaterial);
        }

        private void SetSelectedColor(Material material)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", material.GetColor("_BaseColor") * 2);
        }

        private void SetHoverColor(Material material)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", material.GetColor("_BaseColor"));
        }

        private void RemoveColor(Material material)
        {
            material.DisableKeyword("_EMISSION");
        }
    }
}