using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
   [SerializeField]
   GameSettings _gameSettings = null;
   public static GameSettings GameSettings { get { return Instance._gameSettings; } }

   [SerializeField]
   private List<NetworkedPrefab> _networkPrefabs = new List<NetworkedPrefab>();


   public static GameObject NetworkInstantiate(GameObject obj, Vector3 pos, Quaternion rot)
   {
      Debug.Log("Attempting to instantiate across the network…");

      foreach (var networkPrefab in Instance._networkPrefabs)
      {
         Debug.Log("looping…");
         if (networkPrefab.prefab == obj)
         {
            Debug.Log("networkPrefab.prefab == obj");
            if (networkPrefab.path != string.Empty)
            {
               Debug.Log("path found: " + networkPrefab.path);
               return PhotonNetwork.Instantiate(networkPrefab.path, pos, rot);
            }
            else
            {
               Debug.LogError("Path string empty for gameobject " + networkPrefab.prefab);
               return null;
            }
         }
      }

      return null;
   }

   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   public static void PopulateNetworkPrefabs()
   {

#if UNITY_EDITOR

         // Clear out serialized prefabs we no longer need
      Instance._networkPrefabs.Clear();
      Debug.Log("Populating prefabs");
      
      PhotonView[] res = Resources.LoadAll<PhotonView>("");
      foreach (var pv in res)
      {
         string path = AssetDatabase.GetAssetPath(pv.gameObject);
         Debug.Log(path);
         Instance._networkPrefabs.Add(new NetworkedPrefab(pv.gameObject, path));
      }
#endif
   }
}
