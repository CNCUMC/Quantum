using System.Linq;
using Bark.Tool;
using HarmonyLib;
using Quantum.ItemChange.Gun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class AmmunitionUi
{
    private static TextMeshProUGUI _ammunitionText;
    private static GameObject _ammunitionUiObject;
    private static int _remainingAmmunition;
    private static int _maximumAmmunition;
    
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

        _remainingAmmunition = component.roundsInMag;
        _maximumAmmunition = component.magCapacity;

        CreateOrUpdateAmmunitionUi(__instance);
        UpdateAmmunitionUi();

        SyncVisibility(__instance.gunMenu);
    }

    private static void CreateOrUpdateAmmunitionUi(PlayerCamera camera)
    {
        if (_ammunitionUiObject == null)
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

            _ammunitionUiObject = ammunitionUi;

            var gameObject = new GameObject("AmmunitionText");
            gameObject.transform.SetParent(_ammunitionUiObject.transform, false);

            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(150f, 30f);

            _ammunitionText = gameObject.AddComponent<TextMeshProUGUI>();
            _ammunitionText.alignment = TextAlignmentOptions.Center;

            _ammunitionText.font = Plugin.Unifont;
        }

        var gunMenuPos = GetGunMenuPosition(camera);
        var textRectTransform = _ammunitionText.GetComponent<RectTransform>();
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
            ? _remainingAmmunition + 1
            : _remainingAmmunition;
        if (_ammunitionText == null)
            return;

        if (!Plugin.InfiniteAmmunition)
        {
            if (realRemainingAmmunition >= 0.8)
                _ammunitionText.color = Color.green;
            else if (realRemainingAmmunition >= 0.5)
                _ammunitionText.color = Color.yellow;
            else
                _ammunitionText.color = Color.red;

            _ammunitionText.fontSize = 32;
            _ammunitionText.text = $"{realRemainingAmmunition} / {_maximumAmmunition + 1}";
        }
        else
        {
            _ammunitionText.fontSize = 64;
            _ammunitionText.color = Color.black;
            _ammunitionText.text = "\u221e";
        }

        _ammunitionText.alpha = HiddenHud.Hidden
            ? 0f
            : 1f;
    }

    private static void SyncVisibility(GameObject gunMenu)
    {
        if (_ammunitionUiObject == null || gunMenu == null)
            return;

        var camera = PlayerCamera.main;
        if (camera == null)
        {
            _ammunitionUiObject.SetActive(gunMenu.activeSelf);
            return;
        }

        var shouldShow = gunMenu.activeSelf
                         && !camera.craftingPanel.activeSelf
                         && !camera.woundView.activeSelf
                         && !camera.tradeMenu.activeSelf
                         && !PauseHandler.paused;

        _ammunitionUiObject.SetActive(shouldShow);
    }

    public static void Destroy()
    {
        if (_ammunitionUiObject == null) return;
        Object.Destroy(_ammunitionUiObject);
        _ammunitionUiObject = null;
        _ammunitionText = null;
    }
}