using UnityEngine;

public static class Transforms
{
   public static void DestoryChildren(this Transform t, bool destroyNow = false)
   {
      foreach (Transform child in t)
      {
         if (destroyNow)
         {
            MonoBehaviour.DestroyImmediate(child.gameObject);
         }
         else
         {
            MonoBehaviour.Destroy(child.gameObject);
         }
      }
   }
}