using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{
   [SerializeField]
   private CreateRoomMenu _createRoomMenu = null;

   [SerializeField]
   private RoomsListingMenu _roomsListingMenu = null;

   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
      _createRoomMenu.FirstInitialize(canvases);
      _roomsListingMenu.FirstInitialize(canvases);
   }
}
