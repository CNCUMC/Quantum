using Bark.Tool;
using HarmonyLib;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class HiddenHud
{
    [HarmonyPatch("HandleInput")]
    [HarmonyPostfix]
    public static void HandleInputPostfix(PlayerCamera __instance)
    {
        LogUtil.CheckWorld(Plugin.Logger);
        if (Input.GetKeyDown(Plugin.HiddenHud))
        {
            
        }
    }
}