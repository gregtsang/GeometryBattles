using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryBattles.HexAction
{
    public class HexActionModeManager : MonoBehaviour
    {
        Dictionary<string, HexActionMode> modes = new Dictionary<string, HexActionMode>();
        
        HexActionMode currentMode;
        HexActionMode prevMode;

        public Dictionary<string, HexActionMode> Modes { get => modes; set => modes = value; }

        public event EventHandler<ModeChangedEventArgs> ModeChanged;

        public void EnterMode(string modeName)
        {
            if (modes.ContainsKey(modeName))
            {
                prevMode = currentMode;
                modes[modeName].SetActionMode(true);
                currentMode = modes[modeName];
                Debug.Log($"Entereing mode {modeName}");

                var e = new ModeChangedEventArgs();
                e.newMode = currentMode;
                e.prevMode = prevMode;
                OnModeChanged(e);
            }
        }

        public void ReturnToPrevMode()
        {
            EnterMode(prevMode.ModeName);
        }

        // Start is called before the first frame update
        void Start()
        {
            bool modeSet = false;
            foreach (HexActionMode mode in GetComponentsInChildren<HexActionMode>())
            {
                modes.Add(mode.ModeName, mode);
                if (mode.InitModeOnStart)
                {
                    if (!modeSet)
                    {
                        EnterMode(mode.ModeName);
                        modeSet = true;
                    }
                    else
                    {
                        Debug.LogWarning("Multiple Hex Action Modes set to InitModeOnStart");
                    }
                }
            } 
        }

        // Event Handlers
        private void OnModeChanged(ModeChangedEventArgs e)
        {
            var handler = ModeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class ModeChangedEventArgs : EventArgs
    {
        public HexActionMode newMode { get; set; }
        public HexActionMode prevMode { get; set; }
    }
}