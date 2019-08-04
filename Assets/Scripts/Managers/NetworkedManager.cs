using Photon.Pun;
using UnityEngine;

public class NetworkedManager : MonoBehaviour
{
   [SerializeField]
   private GameObject _boardPrefab = null;

   [Header("Offline Settings")]
   [SerializeField] bool offlineMode = false;
   [SerializeField] int initNumberOfPlayers = 2;

   static int numberOfPlayers;

   private void Awake()
   {
      PhotonNetwork.OfflineMode = offlineMode;
      if (PhotonNetwork.OfflineMode)
      {
         InitializeOfflineGame();
      }
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

   private void InitializeOfflineGame()
   {
      PhotonNetwork.CreateRoom("offline");
      numberOfPlayers = initNumberOfPlayers;
      PhotonNetwork.LocalPlayer.NickName = "Offline Player";
   }

   static public int GetNumberOfPlayers()
   {
      if (PhotonNetwork.OfflineMode)
      {
         return numberOfPlayers;
      }
      else
      {
         return PhotonNetwork.CurrentRoom.PlayerCount;
      }
   }

   static public int GetNumberOfLivePlayers()
   {
      return PhotonNetwork.CurrentRoom.PlayerCount;
   }
}
