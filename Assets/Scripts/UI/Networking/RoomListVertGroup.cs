﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GeometryBattles.MenuUI
{
    public class RoomListVertGroup : MonoBehaviourPunCallbacks
    {
        [SerializeField] TextMeshProUGUI roomNameText = null;
        [SerializeField] TextMeshProUGUI playerCountText = null;

        Dictionary<string, RoomInfoSelectable> currentRooms = new Dictionary<string, RoomInfoSelectable>();

        override public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach(RoomInfo roomInfo in roomList)
            {
                if (currentRooms.ContainsKey(roomInfo.Name))
                {
                    if (roomInfo.RemovedFromList)
                    {
                        DeleteRoomFromList(roomInfo.Name);
                    }
                    else
                    {
                        currentRooms[roomInfo.Name].SetRoomInfo(roomInfo);
                    }
                }
                else
                {
                    AddRoomToList(roomInfo);
                }
            }
        }

        private void AddRoomToList(RoomInfo newRoom)
        {
            GameObject newGameObject = new GameObject();
            RoomInfoSelectable roomInfoSelectable = newGameObject.AddComponent<RoomInfoSelectable>();
            roomInfoSelectable.SetRoomInfo(newRoom);
            roomInfoSelectable.RoomNameText = roomNameText;
            roomInfoSelectable.PlayerCountText = playerCountText;

            newGameObject.AddComponent<TextMeshProUGUI>().text = newRoom.Name;
            newGameObject.AddComponent<Selectable>();
            newGameObject.transform.SetParent(gameObject.transform, false);

            currentRooms.Add(newRoom.Name, roomInfoSelectable);
        }

        private void DeleteRoomFromList(string name)
        {
            foreach (TMP_Text child in gameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                if (child.text == name)
                {
                    Destroy(child.gameObject);
                    currentRooms.Remove(name);
                    break;
                }
            }
        }
    }
}