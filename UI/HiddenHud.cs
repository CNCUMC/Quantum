using HarmonyLib;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class HiddenHud
{
    public static bool Hidden;

    internal static void ApplyVisibility(PlayerCamera camera)
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