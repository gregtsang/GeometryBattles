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

        [SerializeField] RoomList roomList = null;
        
        private void Start()
        {
            ClearRoomInfo();
            roomNameText.onValueChanged.AddListener(delegate {
                SetRoomInfo(roomNameText.text);
            });
        }

        public void SetRoomInfo(string roomName)
        {
            RoomInfo roomInfo = roomList.GetRoomInfoByName(roomName);
            
            if (roomInfo == null)
            {
                ClearRoomInfo();
                return;
            }
            else
            {
                playerCountText.text = roomInfo.PlayerCount.ToString() + " / " + roomInfo.MaxPlayers.ToString();
            }
        }

        public void ClearRoomInfo()
        {
            playerCountText.text = "";
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