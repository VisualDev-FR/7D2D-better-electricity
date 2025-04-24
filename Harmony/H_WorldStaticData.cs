using HarmonyLib;


[HarmonyPatch(typeof(WorldStaticData), "Init")]
public class H_WorldStaticData_Init
{
    public static void Postfix()
    {
        ElectricalComponentManager.LoadComponents();
    }
}