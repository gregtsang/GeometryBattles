using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeometryBattles.PlayerManager;

namespace GeometryBattles.UI
{
    public class ResourceDisplay : MonoBehaviour
    {
        private Text text;
        private UIManager uiManager;

        private void Start()
        {
            text = GetComponent<Text>();
            uiManager = FindObjectOfType<UIManager>();
        }

        private void Update()
        {
            text.text = "Resources: " + uiManager.GetActivePlayer()?.GetResource().ToString() ?? "0";
        }
    }
}