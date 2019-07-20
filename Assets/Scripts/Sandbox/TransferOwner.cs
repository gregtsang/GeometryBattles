using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TransferOwner : MonoBehaviourPun, IPunOwnershipCallbacks
{
   private void Awake()
   {
         // Register IPunOwnershipCallbacks
      PhotonNetwork.AddCallbackTarget(this);
   }

   private void OnDestroy()
   {
      PhotonNetwork.RemoveCallbackTarget(this);
   }

   public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
   {
         // Make sure the request is being made for this object
      if (targetView != photonView) {
         return;
      }

         // Currently an unconditional transfer, but you could control who ownership is transferred to
      photonView.TransferOwnership(requestingPlayer);
   }

   public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
   {

         // Make sure the request is being made for this object
      if (targetView != photonView) {
         return;
      }
   }

   private void OnMouseDown()
   {
      base.photonView.RequestOwnership();
   }
}
