using Photon.Pun;
using UnityEngine;

public class NetworkedManager : MonoBehaviour
{
   [SerializeField]
   private GameObject _boardPrefab = null;

   [SerializeField] bool offlineMode = false;

   private void Awake()
   {
      PhotonNetwork.OfflineMode = offlineMode;      
      
      if (PhotonNetwork.IsMasterClient)
      {
         Debug.Log("I am the master client and I am about to instantiate the board.");
         //MasterManager.NetworkInstantiate(_boardPrefab, Vector3.zero, Quaternion.identity);
         PhotonNetwork.Instantiate(
            "StartGameObject",
            Vector3.zero,
            Quaternion.identity
         );
      }
   }
}
