using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
   [SerializeField]
   private PlayersListingsMenu _playersListingsMenu = null;

   [SerializeField]
   private LeaveRoomMenu _leaveRoomMenu = null;
   public LeaveRoomMenu LeaveRoomMenu { get {  return _leaveRoomMenu; } }

   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
      _playersListingsMenu.FirstInitialize(canvases);
      _leaveRoomMenu.FirstInitialize(canvases);
   }

   public void Show()
   {
      gameObject.SetActive(true);
   }

   public void Hide()
   {
      gameObject.SetActive(false);
   }
}
