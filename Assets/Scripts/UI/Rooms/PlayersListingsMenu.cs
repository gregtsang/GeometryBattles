using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayersListingsMenu : MonoBehaviourPunCallbacks
{
   [SerializeField]
   private Transform _content = null;

   [SerializeField]
   private PlayerListing _playerListing = null;

   private List<PlayerListing> _listings = new List<PlayerListing>();

   public override void OnPlayerEnteredRoom(Player newPlayer)
   {
      base.OnPlayerEnteredRoom(newPlayer);
      AddPlayerListing(newPlayer);
   }


   private void AddPlayerListing(Player player)
   {
      PlayerListing listing = Instantiate(_playerListing, _content);

      if (listing != null)
      {
         listing.SetPlayerInfo(player);
         _listings.Add(listing);
      }
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

   private void Awake()
   {
      GetCurrentRoomPlayers();
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
