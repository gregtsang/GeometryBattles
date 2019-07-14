using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomsListingMenu : MonoBehaviourPunCallbacks
{
   [SerializeField]
   private Transform _content = null;

   [SerializeField]
   private RoomListing _roomListing = null;

   private List<RoomListing> _listings = new List<RoomListing>();
   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }

   public override void OnRoomListUpdate(List<RoomInfo> roomList)
   {
      base.OnRoomListUpdate(roomList);

      foreach (RoomInfo info in roomList)
      {
         if (info.RemovedFromList)
         {
            int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
            if (index != -1)
            {
               Destroy(_listings[index].gameObject);
               _listings.RemoveAt(index);
            }
         }
         else
         {
            RoomListing listing = Instantiate(_roomListing, _content);

            if (listing != null)
            {
               listing.SetRoomInfo(info);
               _listings.Add(listing);
            }
         }
      }
   }

   public override void OnJoinedRoom()
   {
      base.OnJoinedRoom();
      _roomsGUI.CurrentRoomCanvas.Show();
   }


}
