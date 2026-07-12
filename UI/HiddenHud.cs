using System.Linq;
using Bark.Tool;
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
        // 文本元素
        if (camera.weightText != null) camera.weightText.enabled = !Hidden;
        if (camera.statusText != null) camera.statusText.enabled = !Hidden;
        if (camera.timescaleText != null) camera.timescaleText.enabled = !Hidden;
        if (camera.holdingText != null) camera.holdingText.enabled = !Hidden;
        if (camera.holdingDescriptionText != null) camera.holdingDescriptionText.enabled = !Hidden;
        if (camera.holdingSecondaryText != null) camera.holdingSecondaryText.enabled = !Hidden;
        if (camera.consciousnessText != null) camera.consciousnessText.enabled = !Hidden;
        if (camera.alertText != null) camera.alertText.enabled = !Hidden;
        if (camera.containerText != null) camera.containerText.enabled = !Hidden;
        if (camera.radialText != null) camera.radialText.enabled = !Hidden;
        if (camera.charTalkText != null) camera.charTalkText.enabled = !Hidden;
        if (camera.traderNameText != null) camera.traderNameText.enabled = !Hidden;
        if (camera.traderTalkText != null) camera.traderTalkText.enabled = !Hidden;
        if (camera.traderRepText != null) camera.traderRepText.enabled = !Hidden;
        if (camera.traderValueText != null) camera.traderValueText.enabled = !Hidden;
        if (camera.pinRecipeText != null) camera.pinRecipeText.enabled = !Hidden;
        // 图像元素
        if (camera.consciousnessFade != null) camera.consciousnessFade.enabled = !Hidden;
        if (camera.consciousnessVignette != null) camera.consciousnessVignette.enabled = !Hidden;
        if (camera.alertBack != null) camera.alertBack.enabled = !Hidden;
        if (camera.containerFill != null) camera.containerFill.enabled = !Hidden;
        if (camera.containerIcon != null) camera.containerIcon.enabled = !Hidden;
        if (camera.holdingImage != null) camera.holdingImage.enabled = !Hidden;
        if (camera.dragImage != null) camera.dragImage.enabled = !Hidden;
        if (camera.radialCircle != null) camera.radialCircle.enabled = !Hidden;
        if (camera.gunRackImage != null) camera.gunRackImage.enabled = !Hidden;
        if (camera.gunSafeImage != null) camera.gunSafeImage.enabled = !Hidden;
        if (camera.handSwapImage != null) camera.handSwapImage.enabled = !Hidden;
        if (camera.waitImage != null) camera.waitImage.enabled = !Hidden;
        if (camera.waitBackImage != null) camera.waitBackImage.enabled = !Hidden;
        if (camera.endScreen != null) camera.endScreen.enabled = !Hidden;
        if (camera.woundButton != null) camera.woundButton.enabled = !Hidden;
        if (camera.selectedItemOverlay != null) camera.selectedItemOverlay.enabled = !Hidden;
        if (camera.pinRecipeImage != null) camera.pinRecipeImage.enabled = !Hidden;
        if (camera.brainFlash != null) camera.brainFlash.enabled = !Hidden;
        if (camera.playerLiquidRagdollBar != null) camera.playerLiquidRagdollBar.enabled = !Hidden;
        if (camera.traderRepBar != null) camera.traderRepBar.enabled = !Hidden;
        // 游戏对象
        if (camera.depthMeter != null) camera.depthMeter.SetActive(!Hidden);
        if (camera.bonusUseButton != null) camera.bonusUseButton.SetActive(!Hidden);
        if (camera.combineButton != null) camera.combineButton.gameObject.SetActive(!Hidden);
        if (camera.lastStandPanel != null && PlayerUtil.Body.succesfullyRolledLastStand) camera.lastStandPanel.SetActive(!Hidden);
        if (camera.backgroundSnow != null) camera.backgroundSnow.SetActive(!Hidden);
        if (camera.crouchGuide != null) camera.crouchGuide.gameObject.SetActive(!Hidden);
        if (camera.playerDragIndicator != null) camera.playerDragIndicator.gameObject.SetActive(!Hidden);
        if (camera.searchClearButton != null) camera.searchClearButton.SetActive(!Hidden);
        if (camera.recipesBackToTopButton != null) camera.recipesBackToTopButton.SetActive(!Hidden);
        if (camera.craftButton != null) camera.craftButton.SetActive(!Hidden);
        if (camera.traderMoveButon != null) camera.traderMoveButon.SetActive(!Hidden);
        if (camera.traderHagglebutton != null) camera.traderHagglebutton.gameObject.SetActive(!Hidden);
        if (camera.gunMenu != null) camera.gunMenu.SetActive(!Hidden);
        // 数组/列表元素
        if (camera.speedImages != null)
            foreach (var img in camera.speedImages)
                if (img != null)
                    img.enabled = !Hidden;
        if (camera.pickupButtons != null)
            foreach (var btn in camera.pickupButtons)
                if (btn != null)
                    btn.enabled = !Hidden;
        if (camera.radialEdgeImages != null)
            foreach (var img in camera.radialEdgeImages)
                if (img != null)
                    img.enabled = !Hidden;
        if (camera.recipeCategorySelectImages != null)
            foreach (var img in camera.recipeCategorySelectImages)
                if (img != null)
                    img.enabled = !Hidden;
        if (camera.containerUnloadButtons != null)
            foreach (var go in camera.containerUnloadButtons.Where(go => go != null))
                go.SetActive(!Hidden);
        if (camera.containerLoadButtons != null)
            foreach (var go in camera.containerLoadButtons.Where(go => go != null))
                go.SetActive(!Hidden);
        if (camera.unwearButtons != null)
            foreach (var go in camera.unwearButtons.Where(go => go != null))
                go.SetActive(!Hidden);
        // 其他组件
        if (camera.consciousnessECG != null) camera.consciousnessECG.enabled = !Hidden;
        if (camera.throwRenderer != null) camera.throwRenderer.enabled = !Hidden;
    }
}