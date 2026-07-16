using HarmonyLib;

namespace Quantum.Mechanism;

[HarmonyPatch(typeof(CustomItemBehaviour))]
public static class DontBiteLightbulb
{
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static bool UpdatePrefix(CustomItemBehaviour __instance)
    {
        var item = __instance.GetComponent<Item>();
        return item == null || item.id != "lightbulb" || !Plugin.DontBiteLightbulb;
    }
}