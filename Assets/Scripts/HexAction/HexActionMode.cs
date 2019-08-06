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

        public bool InitModeOnStart { get => initModeOnStart; set => initModeOnStart = value; }
        public string ModeName { get => modeName; set => modeName = value; }

        // private void Start()
        // {
        //     if (initModeOnStart)
        //     {
        //         SetActionMode(true);
        //     }
        // }

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