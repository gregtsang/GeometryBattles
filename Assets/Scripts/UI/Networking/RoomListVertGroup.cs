using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace GeometryBattles.MenuUI
{
    public class RoomListVertGroup : MonoBehaviourPunCallbacks
    {
        HashSet<string> currRooms = new HashSet<string>();
        
        int i = 0;

        override public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach(RoomInfo roomInfo in roomList)
            {
                if (currRooms.Contains(roomInfo.Name) && roomInfo.RemovedFromList)
                {
                    DeleteRoomFromList(roomInfo.Name);
                }
                else
                {
                    AddRoomToList(roomInfo.Name);
                }
            }
        }

        private void AddRoomToList(string name)
        {
            GameObject newGameObject = new GameObject(name);
            newGameObject.AddComponent<TextMeshProUGUI>().text = name;
            newGameObject.transform.SetParent(gameObject.transform, false);
            currRooms.Add(name);
        }

        private void DeleteRoomFromList(string name)
        {
            foreach (TMP_Text child in gameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                if (child.text == name)
                {
                    Destroy(child.gameObject);
                    currRooms.Remove(name);
                    break;
                }
            }
        }

        private void Start()
        {
        }

        override public void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(CreateFakeRoom());
        }

        IEnumerator CreateFakeRoom()
        {
            while (true)
            {
                AddRoomToList("Room " + i++.ToString());
                yield return new WaitForSeconds(5);
                if (i % 2 == 0 && i != 0)
                {
                    DeleteRoomFromList("Room " + (i - 1));
                }
            }
        }
    }
}