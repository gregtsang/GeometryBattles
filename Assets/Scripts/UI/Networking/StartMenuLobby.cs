using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;

namespace GeometryBattles.MenuUI
{
    public class StartMenuLobby : MonoBehaviourPunCallbacks
    {

        [SerializeField] TMP_Text statusText = null;

        [Header("Join Game Fields")]
        [SerializeField] TMP_Text joinGameNameField = null;

        [Header("Create Game Fields")]
        [SerializeField] TMP_Text gameNameField = null;
        [SerializeField] Slider playersCountSlider = null;
        [SerializeField] Toggle publicToggle = null;

        StartMenu startMenu = null;

        private void Start()
        {
            startMenu = GetComponent<StartMenu>();
        }
        
        public void CreateGame()
        {
            startMenu.ShowConnectingCanvas();
            statusText.text = "Creating Room...";            
            
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = BitConverter.GetBytes((int) playersCountSlider.value)[0];
            roomOptions.IsVisible = publicToggle.isOn;

            PhotonNetwork.CreateRoom(gameNameField.text, roomOptions);
        }
        
        public void JoinGame()
        {
            PhotonNetwork.JoinRoom(joinGameNameField.text);
            Debug.Log("Joining game " + joinGameNameField.text);
            startMenu.ShowConnectingCanvas();
            statusText.text = "Joining Room...";
        }

        override public void OnCreatedRoom()
        {
            Debug.Log("Created Room!");
        }

        override public void OnCreateRoomFailed(short returnCode, string message)
        {
            statusText.text = message;
        }

        override public void OnJoinedRoom()
        {
            Debug.Log("Joined Room!");
        }

        override public void OnJoinRoomFailed(short returnCode, string message)
        {
            statusText.text = message;
        }
    }
}