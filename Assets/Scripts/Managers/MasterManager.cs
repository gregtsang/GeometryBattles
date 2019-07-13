using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
   [SerializeField] GameSettings _gameSettings = null;
   public static GameSettings GameSettings { get { return Instance._gameSettings; } }

}
