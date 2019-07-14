using UnityEngine;

/* Class that holds a reference to an itself of itself. If more than one instance of this type
   exists  when retrieving the instance for the first time, an error will display and null will
   be returned. The singleton will not be automatically cleaned up if it's not being referenced.
*/
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {

   private static T _instance = null;

   public static T Instance {
      get {
         if (_instance == null) {
            T[] results = Resources.FindObjectsOfTypeAll<T>();

               // Check for user error: not having a singleton instance or having more than one
            if (results.Length == 0) {
               Debug.Log("SingletonScriptableObject -> Instance -> results length is 0 for type " + typeof(T).ToString() + ".");
               return null;
            }
            if (results.Length > 1) {
               Debug.Log("SingletonScriptableObject -> Instance -> results length is greater than 1 for type " + typeof(T).ToString() + ".");
               return null;
            }

            _instance = results[0];

               // Prevent the object from being detroyed automatically when it's not being referenced
            _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
         }

         return _instance;
      }
   }
}
