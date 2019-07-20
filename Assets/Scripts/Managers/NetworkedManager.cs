using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkedManager : MonoBehaviour
{
   [SerializeField]
   private GameObject _boardPrefab;

   private void Awake()
   {
      if (PhotonNetwork.IsMasterClient)
      {
         MasterManager.NetworkInstantiate(_boardPrefab, Vector3.zero, Quaternion.identity);
      }
   }

   // Start is called before the first frame update
   void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
