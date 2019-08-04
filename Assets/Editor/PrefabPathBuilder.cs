#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PrefabPathBuilder : IPreprocessBuildWithReport
{
   public int callbackOrder { get { return 0; } }

      // Creates the list of prefabs before starting a build
   public void OnPreprocessBuild(BuildReport report)
   {
      MasterManager.PopulateNetworkPrefabs();
   }
}
#endif
