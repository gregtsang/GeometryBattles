using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace GeometryBattles.MenuUI
{
    [RequireComponent(typeof(Button))]
    public class DisconnectButton : MonoBehaviour
    {
        private void Start()
        {
            Button button;
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate {
                OnDisconnectButtonClick();
            });
        }

        private void OnDisconnectButtonClick()
        {
            PhotonNetwork.Disconnect();
        }
    }
}