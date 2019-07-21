using UnityEngine;

public class EventManager
{
    public delegate void OnResourceUpdate(); 
    public static event OnResourceUpdate onResourceUpdate;

    public delegate void OnCreateScout(GameObject scout);
    public static event OnCreateScout onCreateScout;

    public static void RaiseOnResourceUpdate()
    {
        if (onResourceUpdate != null)
            onResourceUpdate();
    }

    public static void RaiseOnCreateScout(GameObject scout)
    {
        if (onCreateScout != null)
            onCreateScout(scout);
    }
}
