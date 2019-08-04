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
    public class RoomListVertGroup : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_InputField gameNameInputField = null;

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

        public RoomInfo GetRoomInfoByName(string gameName)
        {
            Debug.Log(gameName + "(" + gameName.Length + ")");
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
        }

        private void AddRoomToList(RoomInfo newRoom)
        {
            GameObject newGameObject = new GameObject();
            RoomInfoSelectable roomInfoSelectable = newGameObject.AddComponent<RoomInfoSelectable>();
            roomInfoSelectable.SetRoomInfo(newRoom);
            roomInfoSelectable.InputField = gameNameInputField;

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