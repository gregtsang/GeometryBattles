using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace GeometryBattles.MenuUI
{
    public class RoomInfoSelectable : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        TextMeshProUGUI roomNameText = null;
        TextMeshProUGUI playerCountText = null;
        RoomInfo roomInfo;

        bool selected = false;

        public RoomInfo RoomInfo { get => roomInfo; }
        public TextMeshProUGUI RoomNameText { get => roomNameText; set => roomNameText = value; }
        public TextMeshProUGUI PlayerCountText { get => playerCountText; set => playerCountText = value; }

        public void SetRoomInfo(RoomInfo roomInfoIn)
        {
            roomInfo = roomInfoIn;
            if (selected)
            {
                UpdateRoomInfoGUI();
            }
        }

        private void ClearRoomInfoGUI()
        {
            RoomNameText.text = "";
            PlayerCountText.text = "";
        }

        private void UpdateRoomInfoGUI()
        {
            RoomNameText.text = roomInfo.Name;
            PlayerCountText.text = roomInfo.PlayerCount.ToString() + " / " + roomInfo.MaxPlayers.ToString();
        }

        public void OnSelect(BaseEventData eventData)
        {
            selected = true;
            UpdateRoomInfoGUI();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            selected = false;
            ClearRoomInfoGUI();
        }

        private void OnDestroy()
        {
            if (selected)
            {
                ClearRoomInfoGUI();
            }
        }
    }
}