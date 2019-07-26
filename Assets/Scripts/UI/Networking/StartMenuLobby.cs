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
        [SerializeField] TMP_Text gameNameField = null;
        [SerializeField] Slider playersCountSlider = null;

        
        public void CreateGame()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = BitConverter.GetBytes((int) playersCountSlider.value)[0];

            PhotonNetwork.CreateRoom(gameNameField.text, roomOptions);
        }
        
        override public void OnCreatedRoom()
        {

        }

        override public void OnCreateRoomFailed(short returnCode, string message)
        {

        }

        override public void OnJoinedRoom()
        {

        }
    }
}