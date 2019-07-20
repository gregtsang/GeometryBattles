using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInstantiateExample : MonoBehaviour
{
   [SerializeField]
   private GameObject _prefab = null;

   private void Awake()
   {
      MasterManager.NetworkInstantiate(_prefab, transform.position, Quaternion.identity);
   }
}
