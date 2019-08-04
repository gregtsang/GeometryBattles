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
        [SerializeField] GameObject playerListing = null;
        [SerializeField] Transform targetTransform = null;

        override public void OnJoinedRoom()
        {
            InitializePlayerList();
        }
        
        override public void OnLeftRoom()
        {
            ClearPlayerList();
        }

        override public void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerToList(newPlayer);
        }

        private void AddPlayerToList(Player newPlayer)
        {
            PreGamePlayerListing playerListing = FindBlankListing();
            if (playerListing != null)
            {
                playerListing.InitializeListing(newPlayer.ActorNumber, newPlayer.NickName);
                object readyObject;
                if (newPlayer.CustomProperties.TryGetValue("ready", out readyObject))
                {
                    playerListing.SetReady((bool) readyObject);
                }
            }
        }

        override public void OnPlayerLeftRoom(Player otherPlayer)
        {
            RemovePlayerFromList(otherPlayer);
        }

        private void RemovePlayerFromList(Player otherPlayer)
        {
            PreGamePlayerListing playerListing = GetPlayerListing(otherPlayer);
            if (playerListing != null)
            {
                playerListing.UnSetPlayer();
            }
        }

        private void InitializePlayerList()
        {
            ClearPlayerList();
            PopulatePlayerList();
            foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerToList(player.Value);
            }
        }

        private void ClearPlayerList()
        {
            foreach (Transform child in targetTransform)
            {
                Destroy(child.gameObject);
            }
        }

        private void PopulatePlayerList()
        {
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                GameObject newPlayerListing = Instantiate(playerListing);
                newPlayerListing.transform.SetParent(targetTransform, false);
            }
        }

        PreGamePlayerListing FindBlankListing()
        {
            foreach (Transform child in targetTransform)
            {
                if(child.GetComponent<PreGamePlayerListing>().IsNotPlayer())
                {
                    return child.GetComponent<PreGamePlayerListing>();
                }
            }
            return null;
        }

        // override public void OnEnable()
        // {
        //     base.OnEnable();
        //     InitializePlayerList();
        // }

        override public void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
        {
            PreGamePlayerListing playerListing = GetPlayerListing(target);
            object readyObject;
            if (changedProps.TryGetValue("ready", out readyObject))
            {
                playerListing.SetReady((bool) readyObject);
            }
            else
            {
                playerListing.SetReady(false);
            }
        }

        PreGamePlayerListing GetPlayerListing(Player target)
        {
            foreach (Transform child in targetTransform)
            {
                if (child.GetComponent<PreGamePlayerListing>().ActorNumber == target.ActorNumber)
                {
                    return child.GetComponent<PreGamePlayerListing>();
                }
            } 
            return null;
        }

    }
}