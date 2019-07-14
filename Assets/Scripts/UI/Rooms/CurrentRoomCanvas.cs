using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }

   public void Show()
   {
      gameObject.SetActive(true);
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
}
