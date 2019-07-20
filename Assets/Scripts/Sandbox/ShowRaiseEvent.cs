using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

public class ShowRaiseEvent : MonoBehaviourPun
{
   private SpriteRenderer _spriteRenderer;
   private const byte COLOR_CHANGE_EVENT = 0;
   
   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
   }

   private void Update()
   {
         // Only change color if client is the owner of the object (master client owns scene objects)
      if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
      {
         ChangeColor();
      }
   }

   private void ChangeColor()
   {
      Vector3 rgb = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      _spriteRenderer.color = new Color(rgb.x, rgb.y, rgb.z);

      object[] data = new object[] {rgb.x, rgb.y, rgb.z};

      PhotonNetwork.RaiseEvent(COLOR_CHANGE_EVENT, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
   }

   private void OnEnable()
   {
      PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
   }

   private void OnDisable()
   {
      PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
   }

   private void NetworkingClient_EventReceived(EventData eventData)
   {
      if (eventData.Code == COLOR_CHANGE_EVENT)
      {
         object[] data = (object[]) eventData.CustomData;
         float r = (float) data[0];
         float g = (float) data[1];
         float b = (float) data[2];

         _spriteRenderer.color = new Color(r, g, b);
      }
   }
}
