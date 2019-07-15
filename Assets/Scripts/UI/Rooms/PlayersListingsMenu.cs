using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayersListingsMenu : MonoBehaviourPunCallbacks
{
   [SerializeField]
   private UnityEngine.Transform _content = null;

   [SerializeField]
   private PlayerListing _playerListing = null;

   private RoomsGUI _roomsGUI = null;
   private List<PlayerListing> _listings = new List<PlayerListing>();

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }

   public override void OnPlayerEnteredRoom(Player newPlayer)
   {
      base.OnPlayerEnteredRoom(newPlayer);
      AddPlayerListing(newPlayer);
   }


   private void AddPlayerListing(Player player)
   {
      int index = _listings.FindIndex(x => x.Player == player);

         // If we already had that player listing, update it
      if (index != -1)
      {
         _listings[index].SetPlayerInfo(player);
      }
         // Otherwise, create a new listing
      else
      { 
         PlayerListing listing = Instantiate(_playerListing, _content);

         if (listing != null)
         {
            listing.SetPlayerInfo(player);
            _listings.Add(listing);
         }
      }
   }

   public override void OnDisable()
   {
      base.OnDisable();
      foreach (var listing in _listings)
      {
         Destroy(listing.gameObject);
      }

      _listings.Clear();
   }

   public override void OnPlayerLeftRoom(Player otherPlayer)
   {
      base.OnPlayerLeftRoom(otherPlayer);

      int index = _listings.FindIndex(x => x.Player == otherPlayer);
      if (index != -1)
      {
         Destroy(_listings[index].gameObject);
         _listings.RemoveAt(index);
      }
   }

   public override void OnEnable()
   {
      base.OnEnable();
      //SetUpReady(false);
      GetCurrentRoomPlayers();
   }

   public override void OnLeftRoom()
   {
      base.OnLeftRoom();
      _content.DestoryChildren();
   }

   private void GetCurrentRoomPlayers()
   {
         // Don't get players if not in a room
      if (!PhotonNetwork.InRoom)
      {
         return;
      }

      foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
      {
         AddPlayerListing(playerInfo.Value);
      }
   }
}
