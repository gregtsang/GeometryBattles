using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.HexAction
{
    public class HexActionMode : MonoBehaviour
    {
        [SerializeField] string modeName = "Default";
        [SerializeField] bool initModeOnStart = false;
        [SerializeField] Color hoverColor = Color.clear;
        [SerializeField] int hoverSize = 1;

        public bool InitModeOnStart { get => initModeOnStart; set => initModeOnStart = value; }
        public string ModeName { get => modeName; set => modeName = value; }
        public Color HoverColor { get => hoverColor; set => hoverColor = value; }
        public int HoverSize { get => hoverSize; set => hoverSize = value; }

        public void SetActionMode(bool on = true)
        {
            if (on)
            {
                HexActionManager.deregisterAllActions();
                RegisterActions();
            }
            else
            {
                DerigsterActions();
            }
        }

        public void DerigsterActions()
        {
            foreach(IHexAction hexAction in GetComponentsInChildren<IHexAction>())
            {
                HexActionManager.deregisterAction(hexAction);
            }
        }

        public void RegisterActions()
        {
            foreach(IHexAction hexAction in GetComponentsInChildren<IHexAction>())
            {
                HexActionManager.registerAction(hexAction);
            }
        }
    }
}