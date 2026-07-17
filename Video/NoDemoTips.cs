using HarmonyLib;
using UnityEngine;

namespace Quantum.Video;

[HarmonyPatch(typeof(GlobalDark))]
public static class NoDemoTips
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void NoDemoTipsPostfix(GlobalDark __instance)
    {
        var demoTips = GameObject.Find("GlobalDark(Clone)/betabuild");
        if (demoTips == null) return;
        demoTips.SetActive(!Plugin.NoDemoTips);
    }
}