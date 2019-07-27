using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

namespace GeometryBattles.MenuUI
{
    public class RoomInfoDisplay : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_InputField roomNameText = null;
        [SerializeField] TextMeshProUGUI playerCountText = null;
        [SerializeField] Button joinGameButton = null;

        [SerializeField] RoomListVertGroup roomListVertGroup = null;
        
        private void Start()
        {
            ClearRoomInfo();
            roomNameText.onValueChanged.AddListener(delegate {
                SetRoomInfo(roomNameText.text);
            });
        }

        public void SetRoomInfo(string roomName)
        {
            RoomInfo roomInfo = roomListVertGroup.GetRoomInfoByName(roomName);
            
            if (roomInfo == null)
            {
                ClearRoomInfo();
                return;
            }
            else
            {
                roomNameText.text = roomInfo.Name;
                playerCountText.text = roomInfo.PlayerCount.ToString() + " / " + roomInfo.MaxPlayers.ToString();
                joinGameButton.gameObject.SetActive(true);
            }
        }

        public void ClearRoomInfo()
        {
            roomNameText.text = "";
            playerCountText.text = "";
            joinGameButton.gameObject.SetActive(false);
        }

        override public void OnJoinedLobby()
        {
            ClearRoomInfo();
        }

        override public void OnLeftLobby()
        {
            ClearRoomInfo();
        }
    }
}