using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }

   public void OnClick_LeaveRoom()
   {
      PhotonNetwork.LeaveRoom(true);
      _roomsGUI.CurrentRoomCanvas.Hide();
   }


}
