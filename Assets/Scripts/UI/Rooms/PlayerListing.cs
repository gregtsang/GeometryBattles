using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{
   [SerializeField]
   private Text _text = null;

   public Player Player { get; private set; }
   public bool Ready = false;

   public void SetPlayerInfo(Player player)
   {
      Player = player;

      int result = -1;
      if (player.CustomProperties.ContainsKey("RandNum"))
      { 
         result = (int) player.CustomProperties["RandNum"];
      }

      _text.text = result.ToString() + ", " + player.NickName;
   }
}
