using Bark.Tool;
using HarmonyLib;
using Quantum.ItemChange;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quantum.Video;

[HarmonyPatch(typeof(PlayerCamera))]
public static class AmmunitionUi
{
    public static TextMeshProUGUI AmmunitionUiText;
    public static GameObject AmunitionUiObject;
    public static int RemainingAmmunition;
    public static int MaximumAmmunition;

    [HarmonyPatch("HandleGunMenu")]
    [HarmonyPostfix]
    private static void HandleGunMenuPostfix(PlayerCamera __instance)
    {
        if (!Plugin.AmmunitionUi) return;

        var handSlot = InventoryUtil.GetHandSlot();
        if (InventoryUtil.IsSlotEmpty(handSlot))
        {
            Destroy();
            return;
        }

        var item = InventoryUtil.GetItemInHand();
        if (item == null || item.Stats == null || !item.Stats.HasTag("gun"))
        {
            Destroy();
            return;
        }

        var component = item.GetComponent<GunScript>();
        if (component == null)
        {
            Destroy();
            return;
        }

        RemainingAmmunition = component.roundsInMag;
        MaximumAmmunition = component.magCapacity;

        CreateOrUpdateAmmunitionUi(__instance);
        UpdateAmmunitionUi();

        SyncVisibility(__instance.gunMenu);
    }

    private static void CreateOrUpdateAmmunitionUi(PlayerCamera camera)
    {
        if (AmunitionUiObject == null)
        {
            var ammunitionUi = new GameObject("AmmunitionUi");
            Object.DontDestroyOnLoad(ammunitionUi);

            var canvas = ammunitionUi.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var canvasScaler = ammunitionUi.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);

            ammunitionUi.AddComponent<GraphicRaycaster>();

            AmunitionUiObject = ammunitionUi;

            var gameObject = new GameObject("AmmunitionText");
            gameObject.transform.SetParent(AmunitionUiObject.transform, false);

            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(150f, 30f);

            AmmunitionUiText = gameObject.AddComponent<TextMeshProUGUI>();
            AmmunitionUiText.alignment = TextAlignmentOptions.Center;

            AmmunitionUiText.font = TextUtil.RetroGamingTMP;
        }

        var gunMenuPos = GetGunMenuPosition(camera);
        var textRectTransform = AmmunitionUiText.GetComponent<RectTransform>();
        textRectTransform.anchoredPosition = new Vector2(gunMenuPos.x, gunMenuPos.y - 450f);

        SyncVisibility(camera.gunMenu);
    }

    private static Vector2 GetGunMenuPosition(PlayerCamera camera)
    {
        if (camera.gunMenu == null) return new Vector2(0f, 50f);
        var gunMenuRect = camera.gunMenu.GetComponent<RectTransform>();
        if (gunMenuRect == null) return new Vector2(0f, 50f);
        var pos = gunMenuRect.anchoredPosition;
        pos.y -= gunMenuRect.rect.height * 0.5f;
        return pos;
    }

    private static void UpdateAmmunitionUi()
    {
        var realRemainingAmmunition = GunScriptPatch.HasOne
            ? RemainingAmmunition + 1
            : RemainingAmmunition;
        if (AmmunitionUiText == null)
            return;

        if (!Plugin.InfiniteAmmunition)
        {
            if (realRemainingAmmunition >= 0.8)
                AmmunitionUiText.color = Color.green;
            else if (realRemainingAmmunition >= 0.5)
                AmmunitionUiText.color = Color.yellow;
            else
                AmmunitionUiText.color = Color.red;

            AmmunitionUiText.fontSize = 32;
            AmmunitionUiText.text = $"{realRemainingAmmunition} / {MaximumAmmunition + 1}";
        }
        else
        {
            AmmunitionUiText.fontSize = 64;
            AmmunitionUiText.color = Color.black;
            AmmunitionUiText.text = "\u221e";
        }

        AmmunitionUiText.alpha = HiddenHud.Hidden
            ? 0f
            : 1f;
    }

    private static void SyncVisibility(GameObject gunMenu)
    {
        if (AmunitionUiObject == null || gunMenu == null)
            return;

        var camera = PlayerCamera.main;
        if (camera == null)
        {
            AmunitionUiObject.SetActive(gunMenu.activeSelf);
            return;
        }

        var shouldShow = gunMenu.activeSelf
                         && !camera.craftingPanel.activeSelf
                         && !camera.woundView.activeSelf
                         && !camera.tradeMenu.activeSelf
                         && !PauseHandler.paused;

        AmunitionUiObject.SetActive(shouldShow);
    }

    public static void Destroy()
    {
        if (AmunitionUiObject == null) return;
        Object.Destroy(AmunitionUiObject);
        AmunitionUiObject = null;
        AmmunitionUiText = null;
    }
}