using HarmonyLib;
using Quantum.UI;

namespace Quantum.ItemChange;

[HarmonyPatch(typeof(GunScript))]
public static class GunScriptPatch
{
    internal static bool HasOne;

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static void UpdatePrefix(GunScript __instance)
    {
        HasOne = __instance.roundInChamber == GunScript.RoundInChamber.Round;

        if (Plugin.AutoRack
            && __instance.roundInChamber
                is GunScript.RoundInChamber.Casing
                or GunScript.RoundInChamber.None
            && __instance.roundsInMag > 0)
        {
            __instance.roundsInMag--;
            __instance.roundInChamber = GunScript.RoundInChamber.Round;
            __instance.racked = false;
        }

        if (Plugin.InfiniteAmmunition)
            __instance.roundsInMag = __instance.magCapacity;

        __instance.knockBack = Plugin.Recoilless
            ? 0
            : 8;

        if (Plugin.IndestructibleGun)
            __instance.conditionLossPerShot = 0;

        if (!Plugin.AmmunitionUi)
            AmmunitionUi.Destroy();
    }

    [HarmonyPatch("Fire")]
    [HarmonyPostfix]
    public static void FirePostfix(GunScript __instance)
    {
        if (Plugin.NoCasing
            && __instance.roundInChamber == GunScript.RoundInChamber.Casing)
            __instance.roundInChamber = GunScript.RoundInChamber.None;
    }

    [HarmonyPatch("JamChance")]
    [HarmonyPostfix]
    public static void JamChancePostfix(ref float __result)
    {
        if (!Plugin.NeverJam) return;
        __result = 0;
    }
}