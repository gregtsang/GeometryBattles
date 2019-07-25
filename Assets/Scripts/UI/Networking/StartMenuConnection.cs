using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GeometryBattles.Networking;
using Photon.Pun;
using Photon.Realtime;

namespace GeometryBattles.MenuUI
{
    [RequireComponent(typeof(StartMenu))]
    public class StartMenuConnection : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text statusText = null;

        StartMenu startMenu = null;

        private void Start()
        {
            startMenu = GetComponent<StartMenu>();
        }

        public void CreateConnection()
        {
            startMenu.ShowConnectingCanvas();
            statusText.text = "Connecting to Server...";
            ServerConnectionSettings.SetServerConnectionSettings();
            PhotonNetwork.ConnectUsingSettings();
        }

        override public void OnConnectedToMaster()
        {
            print("Connected to server : )");
            JoinLobby();
        }

        private void JoinLobby()
        {
            statusText.text = "Joining Lobby...";
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        override public void OnJoinedLobby()
        {
            startMenu.ShowGameLobbyCanvas();
        }

        override public void OnDisconnected(DisconnectCause cause)
        {
            if (cause != DisconnectCause.DisconnectByClientLogic)
            {
                if (cause == DisconnectCause.ExceptionOnConnect)
                {
                    statusText.text = "Failed to connect to server: ";
                }
                else
                {
                    statusText.text = "Disconnected from server: ";
                }
                statusText.text = statusText.text + cause.ToString();

                startMenu.ShowConnectingCanvas();
            }
            Debug.Log("disconnected: " + cause.ToString());
        }
    }
}