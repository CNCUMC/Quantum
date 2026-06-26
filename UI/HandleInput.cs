using Bark.Constant;
using HarmonyLib;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class HandleInput
{
    [HarmonyPatch("HandleInput")]
    [HarmonyPostfix]
    private static void HandleInputPostfix(PlayerCamera __instance)
    {
        var camera = PlayerCamera.main;

        if (Input.GetKeyDown(Keys.ToggleInventory) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (camera.craftingPanel.activeSelf)
                camera.craftingPanel.SetActive(false);
            if (camera.woundView.activeSelf)
                __instance.ToggleWoundView();
            if (camera.tradeMenu.activeSelf)
                __instance.ToggleTradeMenu();
        }

        if (!Input.GetKeyDown(Plugin.SortKey))
            return;
        if (!SortButtons.CanSort(__instance))
            return;
        SortButtons.SortContainer(__instance);
    }
}
