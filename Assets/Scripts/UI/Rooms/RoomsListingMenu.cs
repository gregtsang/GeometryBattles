using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomsListingMenu : MonoBehaviourPunCallbacks
{
   [SerializeField]
   private UnityEngine.Transform _content = null;

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
            // Consider updating the _listings to hashmap to save time here...
         int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);

            // If the room has been flagged for removal and is _listings, remove
         if (info.RemovedFromList)
         {
            if (index != -1)
            { 
               Destroy(_listings[index].gameObject);
               _listings.RemoveAt(index);
            }
         }
            // Otherwise, if the room doesn't exist yet, create it
         else if (index == -1)
         {
            RoomListing listing = Instantiate(_roomListing, _content);

            if (listing != null)
            {
               listing.SetRoomInfo(info);
               _listings.Add(listing);
            }
         }
            // Otherwise, the room already exists, so it's being updated
         else
         {
               // Update room listing here...
            _listings[index].SetRoomInfo(info);
         }
      }
   }

   public override void OnJoinedRoom()
   {
      base.OnJoinedRoom();
      _roomsGUI.CurrentRoomCanvas.Show();
      _content.DestoryChildren();
      _listings.Clear();
   }


}
