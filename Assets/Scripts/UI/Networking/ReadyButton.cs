using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

namespace GeometryBattles.MenuUI
{
    [RequireComponent(typeof(Button))]
    public class ReadyButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(delegate {
                FlipReadyStatus();
            });
        }

        private void FlipReadyStatus()
        {
            object currentReadyProperty;
            bool currentReadyStatus = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("ready", out currentReadyProperty))
            {
                currentReadyStatus = (bool) currentReadyProperty;
            }
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
                {"ready", !currentReadyStatus}
            });
        }
    }
}