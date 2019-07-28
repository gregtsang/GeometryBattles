using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GeometryBattles.MenuUI
{
    public class RoomList : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_InputField gameNameInputField = null;
        
        [SerializeField] Transform targetTransform = null;

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
                    if (!roomInfo.RemovedFromList)
                    {
                        AddRoomToList(roomInfo);
                    }
                }
            }
        }

        override public void OnLeftLobby()
        {
            ResetRoomList();
        }

        override public void OnJoinedLobby()
        {
            ResetRoomList();
            gameNameInputField.text = "";
        }

        public RoomInfo GetRoomInfoByName(string gameName)
        {
            if (currentRooms.ContainsKey(gameName))
            {
                return currentRooms[gameName].RoomInfo;
            }
            return null;
        }

        private void ResetRoomList()
        {
            foreach (RoomInfoSelectable roomInfo in currentRooms.Values)
            {
                Destroy(roomInfo.gameObject);
            }
            currentRooms = new Dictionary<string, RoomInfoSelectable>();
        }

        private void AddRoomToList(RoomInfo newRoom)
        {
            GameObject newGameObject = new GameObject();
            RoomInfoSelectable roomInfoSelectable = newGameObject.AddComponent<RoomInfoSelectable>();
            roomInfoSelectable.SetRoomInfo(newRoom);
            roomInfoSelectable.InputField = gameNameInputField;

            newGameObject.AddComponent<TextMeshProUGUI>().text = newRoom.Name;
            newGameObject.AddComponent<Selectable>();
            newGameObject.transform.SetParent(targetTransform, false);

            currentRooms.Add(newRoom.Name, roomInfoSelectable);
        }

        private void DeleteRoomFromList(string name)
        {
            foreach (TMP_Text child in targetTransform.GetComponentsInChildren<TMP_Text>())
            {
                if (child.text == name)
                {
                    Destroy(child.gameObject);
                    currentRooms.Remove(name);
                    if (gameNameInputField.text == name)
                    {
                        gameNameInputField.text = "";
                    }
                    break;
                }
            }
        }
    }
}