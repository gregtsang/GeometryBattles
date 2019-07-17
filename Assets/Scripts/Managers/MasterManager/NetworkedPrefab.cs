using UnityEngine;

[System.Serializable]
public class NetworkedPrefab
{
   public GameObject prefab;
   public string path;

   public NetworkedPrefab(GameObject _prefab, string _path)
   {
      prefab = _prefab;
      path = getModifiedPrefabPath(_path);
   }

      // Convert name to begin with resources subfolder (if any) and drop the extension
      // e.g. /Assets/Resources/f1/s1/myprefab.asset -> f1/s1/myprefab
   private string getModifiedPrefabPath(string _path)
   {
      int extensionLength = System.IO.Path.GetExtension(_path).Length;
      int resourceLength = "resources/".Length;
      int startIndex = _path.ToLower().IndexOf("resources");

      return startIndex == -1
         ? string.Empty
         : _path.Substring(startIndex + resourceLength, _path.Length - (resourceLength + startIndex + extensionLength));
   }
}
