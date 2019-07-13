using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomsListingMenu : MonoBehaviourPunCallbacks
{
   [SerializeField]
   private Transform _content;

   [SerializeField]
   private RoomListing _roomListing;

   private List<RoomListing> _listings = new List<RoomListing>();

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
}
