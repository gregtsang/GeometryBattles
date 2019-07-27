using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

namespace GeometryBattles.MenuUI
{
    public class PlayerList : MonoBehaviourPunCallbacks
    {
        override public void OnJoinedRoom()
        {
            UpdatePlayerList();
        }
        
        override public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            AddPlayerToList(newPlayer.NickName);
        }

        private void AddPlayerToList(string nickName)
        {
            GameObject newGameObject = new GameObject();
            newGameObject.AddComponent<TextMeshProUGUI>().text = nickName;
            newGameObject.transform.parent = gameObject.transform;
        }

        override public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RemovePlayerFromList(otherPlayer.NickName);
        }

        private void RemovePlayerFromList(string nickName)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<TextMeshProUGUI>().text == nickName)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

        private void UpdatePlayerList()
        {
            ClearPlayerList();
            foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerToList(player.Value.NickName);
            }
        }

        private void ClearPlayerList()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        override public void OnEnable()
        {
            base.OnEnable();
            UpdatePlayerList();
        }
    }
}