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

        public void EnterMode(string modeName)
        {
            if (modes.ContainsKey(modeName))
            {
                prevMode = currentMode;
                modes[modeName].SetActionMode(true);
                currentMode = modes[modeName];
                Debug.Log($"Entereing mode {modeName}");
            }
        }

        public void ReturnToPrevMode()
        {
            // prevMode.SetActionMode(true);
            // HexActionMode temp = currentMode;
            // currentMode = prevMode;
            // prevMode = temp;
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

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}