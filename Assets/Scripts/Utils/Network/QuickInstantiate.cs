using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInstantiate : MonoBehaviour
{
   [SerializeField]
   private GameObject _prefab;

   private void Awake()
   {
      Vector2 offset = Random.insideUnitCircle * 3.0f;
      Vector3 pos = new Vector3(
         transform.position.x + offset.x,
         transform.position.y + offset.y,
         transform.position.z
      );

      MasterManager.NetworkInstantiate(_prefab, pos, Quaternion.identity);
   }
}
