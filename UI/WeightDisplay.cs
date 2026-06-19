using System.Linq;
using HarmonyLib;
using MossLib.Tool;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class WeightDisplay
{
    [HarmonyPatch("HandleRadialMenu")]
    [HarmonyPostfix]
    private static void HandleRadialMenuPostfix(PlayerCamera __instance)
    {
        if (__instance.weightText == null || __instance.body == null)
            return;

        var totalValue = Inventory.GetAllItemInfosThorough().Sum(info => info.value);
        __instance.weightText.text +=
            $" <color=#f6ff73>{totalValue}</color>";
    }
}