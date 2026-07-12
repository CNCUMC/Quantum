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
        if (camera.timescaleText != null) camera.timescaleText.enabled = !Hidden;
        if (camera.pinRecipeText != null) camera.pinRecipeText.enabled = !Hidden;

        if (camera.consciousnessFade != null) camera.consciousnessFade.enabled = !Hidden;
        if (camera.gunRackImage != null && camera.gunRackImage.enabled) camera.gunRackImage.enabled = !Hidden;
        if (camera.gunSafeImage != null && camera.gunSafeImage.enabled) camera.gunSafeImage.enabled = !Hidden;
        if (camera.endScreen != null) camera.endScreen.enabled = !Hidden;
        if (camera.woundButton != null) camera.woundButton.enabled = !Hidden;

        SetGoVisible(camera.depthMeter, !Hidden);
        SetGoVisible(camera.craftButton, !Hidden);
        SetGoVisible(camera.gunMenu, !Hidden);
        SetGoVisible(camera.gunCrosshair.gameObject, !Hidden);

        if (camera.lastStandPanel != null && camera.lastStandPanel.activeSelf)
            SetGoVisible(camera.lastStandPanel, !Hidden);
    }

    private static void SetGoVisible(GameObject go, bool visible)
    {
        if (go == null || !go.activeSelf) return;

        var cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();

        cg.alpha = visible 
            ? 1f 
            : 0f;
        cg.blocksRaycasts = visible;
    }
}
