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

    public delegate void OnStructureHeal(int q, int r, int amount);
    public static event OnStructureHeal onStructureHeal;

    public delegate void OnCreateCube(GameObject cube);
    public static event OnCreateCube onCreateCube;

    public delegate void OnCreatePentagon(GameObject pentagon);
    public static event OnCreatePentagon onCreatePentagon;

    public delegate void OnProjectileCollision(GameObject projectile);
    public static event OnProjectileCollision onProjectileCollision;

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

    public static void RaiseOnStructureHeal(int q, int r, int amount)
    {
        if (onStructureHeal != null)
            onStructureHeal(q, r, amount);
    }

    public static void RaiseOnCreateCube(GameObject cube)
    {
        if (onCreateCube != null)
            onCreateCube(cube);
    }

    public static void RaiseOnCreatePentagon(GameObject pentagon)
    {
        if (onCreatePentagon != null)
            onCreatePentagon(pentagon);
    }

    public static void RaiseOnProjectileCollision(GameObject projectile)
    {
        if (onProjectileCollision != null)
            onProjectileCollision(projectile);
    }
}
