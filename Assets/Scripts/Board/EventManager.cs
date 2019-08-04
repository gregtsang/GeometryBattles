using UnityEngine;

public class EventManager
{
    public delegate void OnGameOver(GameObject winner);
    public static event OnGameOver onGameOver;

    public delegate void OnResourceUpdate(); 
    public static event OnResourceUpdate onResourceUpdate;

    public delegate void OnCreateBase(int q, int r, GameObject playerBase);
    public static event OnCreateBase onCreateBase;

    public delegate void OnStructureDamage(int q, int r, int amount);
    public static event OnStructureDamage onStructureDamage;

    public delegate void OnCreateScout(GameObject scout);
    public static event OnCreateScout onCreateScout;

    public static void RaiseOnGameOver(GameObject winner)
    {
        if (onGameOver != null)
            onGameOver(winner);
    }

    public static void RaiseOnResourceUpdate()
    {
        if (onResourceUpdate != null)
            onResourceUpdate();
    }

    public static void RaiseOnCreateBase(int q, int r, GameObject playerBase)
    {
        if (onCreateBase != null)
            onCreateBase(q, r, playerBase);
    }

    public static void RaiseOnStructureDamage(int q, int r, int amount)
    {
        if (onStructureDamage != null)
            onStructureDamage(q, r, amount);
    }

    public static void RaiseOnCreateScout(GameObject scout)
    {
        if (onCreateScout != null)
            onCreateScout(scout);
    }
}
