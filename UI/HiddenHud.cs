using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class HiddenHud
{
    public static bool Hidden;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void PostUpdate(PlayerCamera __instance)
    {
        if (!Input.GetKeyDown(Plugin.HiddenHud)) return;
        Hidden = !Hidden;
        ApplyVisibility(__instance);
    }

    private static void ApplyVisibility(PlayerCamera camera)
    {
        // 遍历所有 Canvas（包括子对象）
        foreach (var canvas in camera.GetComponentsInChildren<Canvas>(true))
        {
            var cg = canvas.GetComponent<CanvasGroup>();
            if (cg == null) cg = canvas.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = Hidden 
                ? 0f
                : 1f;
            cg.blocksRaycasts = !Hidden;
        }
    }
}