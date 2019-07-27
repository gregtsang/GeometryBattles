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
        TMP_InputField inputField = null;

        RoomInfo roomInfo;

        bool selected = false;

        public TMP_InputField InputField { get => inputField; set => inputField = value; }
        public RoomInfo RoomInfo { get => roomInfo; }

        public void SetRoomInfo(RoomInfo roomInfoIn)
        {
            roomInfo = roomInfoIn;
            if (selected)
            {
                UpdateRoomNameText();
            }
        }

        private void ClearRoomInfoGUI()
        {
            //roomInfoDisplay.ClearRoomInfo();
        }

        private void UpdateRoomNameText()
        {
            inputField.text = roomInfo.Name;
            //roomInfoDisplay.SetRoomInfo(roomInfo);
        }

        public void OnSelect(BaseEventData eventData)
        {
            selected = true;
            FormatSelfSelected();
            UpdateRoomNameText();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            selected = false;
            FormatSelfUnSelected();
            //ClearRoomInfoGUI();
        }

        private void FormatSelfSelected()
        {
            GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        }

        private void FormatSelfUnSelected()
        {
            GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
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