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
            var canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = Hidden 
                ? 0f
                : 1f;
            canvasGroup.blocksRaycasts = !Hidden;
        }
    }
}