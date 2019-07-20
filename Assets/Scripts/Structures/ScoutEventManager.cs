namespace GeometryBattles.StructureManager
{
    public class ScoutEventManager
    {
        public delegate void OnCreate(CubeScout scout);
        public static event OnCreate onCreate;
        
        public static void RaiseOnCreate(CubeScout scout)
        {
            if (onCreate != null)
                onCreate(scout);
        }
    }
}
