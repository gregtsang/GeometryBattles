using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class provides a .photonView and all callbacks/events that PUN can call. Override the events/methods you want to use.
/// </summary>
/// <remarks>
/// By extending this class, you can implement individual methods as override.
/// 
/// Do not add <b>new</b> <code>MonoBehaviour.OnEnable</code> or <code>MonoBehaviour.OnDisable</code>
/// Instead, you should override those and call <code>base.OnEnable</code> and <code>base.OnDisable</code>.
/// 
/// Visual Studio and MonoDevelop should provide the list of methods when you begin typing "override".
/// <b>Your implementation does not have to call "base.method()".</b>
/// 
/// This class implements all callback interfaces and extends <see cref="T:Photon.Pun.MonoBehaviourPun"/>.
/// </remarks>
/// \ingroup callbacks
public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
   private RoomsGUI _roomsGUI = null;

   public void FirstInitialize(RoomsGUI canvases)
   {
      _roomsGUI = canvases;
   }


   [SerializeField] Text _roomName = null;

   public void OnClick_CreateRoom()
   {
      if (!PhotonNetwork.IsConnected)
      {
         Debug.Log("Cannot create room. Currently not connected to the Photon Network...");
         return;
      }

      RoomOptions options = new RoomOptions();
      options.MaxPlayers = 10;
      PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
   }

   public override void OnCreatedRoom()
   {
      base.OnCreatedRoom();

      Debug.Log("Created room successfully." + this);
      _roomsGUI.CurrentRoomCanvas.Show();
   }

   public override void OnCreateRoomFailed(short returnCode, string message)
   {
      base.OnCreateRoomFailed(returnCode, message);

      Debug.Log("Room created failed: " + message + this);
   }
}
