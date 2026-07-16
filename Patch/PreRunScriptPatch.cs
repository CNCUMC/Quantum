using HarmonyLib;
using UnityEngine;

namespace Quantum.Patch;

[HarmonyPatch(typeof(PreRunScript))]
public class PreRunScriptPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(PreRunScript __instance)
    {
        if (__instance.creditsScreen.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            __instance.creditsScreen.SetActive(false);
            return;
        }
        if (__instance.runSettingsScreen.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            __instance.runSettingsScreen.SetActive(false);
        }
    }
}