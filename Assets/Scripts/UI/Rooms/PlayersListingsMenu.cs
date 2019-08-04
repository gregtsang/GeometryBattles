using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayersListingsMenu : MonoBehaviourPunCallbacks
{
      // Set to multiplayer game scene
   private int sceneIndex = 1;

   [SerializeField]
   private UnityEngine.Transform _content = null;

   [SerializeField]
   private PlayerListing _playerListing = null;

   [SerializeField]
   private Text _readyText = null;

   private RoomsGUI _roomsGUI = null;
   private List<PlayerListing> _listings = new List<PlayerListing>();
   private bool _ready = false;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }

      // Force players to leave room if the master client leaves.
      // The alternative would be to have all players hold onto ready
      // information of all the players (probably a better solution…).
   public override void OnMasterClientSwitched(Player newMasterClient)
   {
      base.OnMasterClientSwitched(newMasterClient);
      _roomsGUI.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
   }

   private void SetReady(bool isReady)
   {
      _ready = isReady;
      _readyText.text = isReady ? "Ready!" : "Ready?";
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
      SetReady(false);
      GetCurrentRoomPlayers();
   }

   public override void OnLeftRoom()
   {
      base.OnLeftRoom();
      _content.DestoryChildren();
   }

   private void GetCurrentRoomPlayers()
   {
         // Only get the players in the room if we actually have a connection and are in a room with players
      if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom is null || PhotonNetwork.CurrentRoom.Players is null)
         return;

      foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
      {
         AddPlayerListing(playerInfo.Value);
      }
   }

   public void OnClick_StartGame()
   {
      if (PhotonNetwork.IsMasterClient)
      {
            // If a player isn't ready, don't start the game (ignore the master client)
         //foreach (var player in _listings)
         //   if (!player.Ready && PhotonNetwork.LocalPlayer != player.Player)
         //      return;

            // Prevent anyone from joining or seeing room and load next scene
         PhotonNetwork.CurrentRoom.IsOpen = false;
         PhotonNetwork.CurrentRoom.IsVisible = false;
         PhotonNetwork.LoadLevel(sceneIndex);
      }
   }

   public void OnClick_Ready()
   {
      if (!PhotonNetwork.IsMasterClient)
      { 
         SetReady(!_ready);
         photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _ready);
      }
   }

   [PunRPC]
   private void RPC_ChangeReadyState(Player player, bool isReady)
   {
      int index = _listings.FindIndex(x => x.Player == player);
      if (index != -1)
      {
         _listings[index].Ready = isReady;
      }
   }
}
