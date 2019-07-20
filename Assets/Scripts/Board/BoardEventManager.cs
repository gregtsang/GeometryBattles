using GeometryBattles.BoardManager;
using GeometryBattles.StructureManager;

public class BoardEventManager
{
    public delegate void OnBoardUpdate(); 
    public static event OnBoardUpdate onBoardUpdate;

    public delegate void OnCreateScout(CubeScout scout);
    public static event OnCreateScout onCreateScout;

    public static void RaiseOnBoardUpdate()
    {
        if (onBoardUpdate != null)
            onBoardUpdate();
    }

    public static void RaiseOnCreateScout(CubeScout scout)
    {
        if (onCreateScout != null)
            onCreateScout(scout);
    }
}
