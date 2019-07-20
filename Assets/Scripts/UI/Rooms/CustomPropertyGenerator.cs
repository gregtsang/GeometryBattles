using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CustomPropertyGenerator : MonoBehaviour
{
   [SerializeField]
   private Text _text = null;

   private ExitGames.Client.Photon.Hashtable _customProperties = new ExitGames.Client.Photon.Hashtable();

   public void OnClick_Button()
   {
      SetNumber();
   }

   private void SetNumber()
   {
      var rng = new System.Random();
      var result = rng.Next(0, 99);
      _text.text = result.ToString();

      _customProperties["RandNum"] = result;
      PhotonNetwork.LocalPlayer.CustomProperties = _customProperties;
   }
}
