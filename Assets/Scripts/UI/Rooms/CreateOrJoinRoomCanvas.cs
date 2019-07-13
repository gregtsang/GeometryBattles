using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{
   [SerializeField]
   private CreateRoomMenu _createRoomMenu;

   private RoomsGUI _roomsGUI;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
      _createRoomMenu.FirstInitialize(canvases);
   }
}
