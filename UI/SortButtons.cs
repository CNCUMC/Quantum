using System;
using System.Collections.Generic;
using System.Linq;
using Bark.BetterCCL;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class SortButtons
{
    private static SortMode _currentSortMode;
    private static bool _currentSortAscending = true;
    private static GameObject _sortButton;
    private static GameObject _orderButton;
    private static GameObject _executeButton;

    [HarmonyPatch("OpenContainer")]
    [HarmonyPostfix]
    private static void ContainerOpenedPostfix(PlayerCamera __instance)
    {
        CreateSortButtons(__instance);
    }

    [HarmonyPatch("CloseContainer")]
    [HarmonyPostfix]
    private static void ContainerClosedPostfix()
    {
        DestroySortButtons();
    }

    public static bool CanSort(PlayerCamera camera)
    {
        if (camera == null || !camera.isActiveAndEnabled)
            return false;
        if (camera.currentContainer == null)
            return false;
        if (camera.craftingPanel != null && camera.craftingPanel.activeSelf)
            return false;
        if (camera.tradeMenu != null && camera.tradeMenu.activeSelf)
            return false;
        return camera.dragItem == null;
    }

    public static void SortContainer(PlayerCamera camera)
    {
        var container = camera.currentContainer;
        if (container == null)
            return;

        var items = GetDirectContainerItems(container);
        if (items.Count <= 1)
            return;

        var sorted = SortItems(items);
        if (HasSameOrder(items, sorted))
        {
            ShowSortNotification(true);
            return;
        }

        foreach (var item in items)
            container.UnloadItem(item);

        foreach (var item in sorted)
            container.LoadItem(item);

        camera.RepopulateContainer();
        camera.PlayUISound(PlayerCamera.UISoundType.Click);
        ShowSortNotification(false);
    }

    private static List<Item> GetDirectContainerItems(Container container)
    {
        var items = new List<Item>(container.transform.childCount);
        for (var i = 0; i < container.transform.childCount; i++)
        {
            var child = container.transform.GetChild(i);
            if (child == null) continue;
            var item = child.GetComponent<Item>();
            if (item != null)
                items.Add(item);
        }

        return items;
    }

    private static List<Item> SortItems(List<Item> items)
    {
        var ascending = _currentSortAscending;

        return _currentSortMode switch
        {
            SortMode.Name => ascending
                ? items.OrderByDescending(i => i.favourited)
                    .ThenBy(i => i.id, StringComparer.OrdinalIgnoreCase).ToList()
                : items.OrderByDescending(i => i.favourited)
                    .ThenByDescending(i => i.id, StringComparer.OrdinalIgnoreCase).ToList(),

            SortMode.Value => ascending
                ? items.OrderByDescending(i => i.favourited)
                    .ThenBy(GetItemValue).ToList()
                : items.OrderByDescending(i => i.favourited)
                    .ThenByDescending(GetItemValue).ToList(),

            SortMode.Weight => ascending
                ? items.OrderByDescending(i => i.favourited)
                    .ThenBy(i => i.Stats.weight).ToList()
                : items.OrderByDescending(i => i.favourited)
                    .ThenByDescending(i => i.Stats.weight).ToList(),

            _ => items
        };
    }

    private static float GetItemValue(Item item)
    {
        float val = item.Stats.value;
        if (item.condition > 0f)
            val *= item.condition;
        return val;
    }

    private static bool HasSameOrder(IReadOnlyList<Item> current, IReadOnlyList<Item> sorted)
    {
        if (current.Count != sorted.Count)
            return false;
        return !current.Where((t, i) => t != sorted[i]).Any();
    }

    private static void CreateSortButtons(PlayerCamera camera)
    {
        if (camera.containerMenu == null)
            return;
        var top = camera.containerMenu.transform.Find("Top");
        if (top == null)
            return;
        var weight = top.Find("Weight");
        if (weight == null)
            return;

        DestroySortButtons();

        var weightRect = weight.GetComponent<RectTransform>();
        var baseX = weightRect.anchoredPosition.x + weightRect.sizeDelta.x + 5f;
        var y = weightRect.anchoredPosition.y - 3f;
        var layer = LayerMask.NameToLayer("UI");

        var modeLabel = GetSortModeLabel(_currentSortMode);
        _sortButton = CreateSmallButton(top, "SortButton", layer,
            modeLabel.Length > 0
                ? modeLabel.Substring(0, 1)
                : "?", baseX - 50f, y - 5f);

        _orderButton = CreateSmallButton(top, "OrderButton", layer,
            _currentSortAscending
                ? "\u2191"
                : "\u2193", baseX, y - 5f);

        _executeButton = CreateSmallButton(top, "ExecuteButton", layer,
            "S", baseX + 50f, y - 5f);

        AddSortModeClickEvent(_sortButton, camera);
        AddSortExecuteClickEvent(_executeButton, camera);
        AddOrderClickEvent(_orderButton, camera);

        AddTooltip(_sortButton,
            Locale("ui.sort.mode_tip"),
            Locale("ui.sort.mode_desc", GetSortModeLabel(_currentSortMode)));

        AddTooltip(_executeButton,
            Locale("ui.sort.execute_tip"),
            Locale("ui.sort.execute_desc"));

        AddTooltip(_orderButton,
            Locale("ui.sort.order_tip"),
            _currentSortAscending
                ? Locale("ui.sort.ascending")
                : Locale("ui.sort.descending"));
    }

    private static void DestroySortButtons()
    {
        if (_sortButton != null)
        {
            Object.Destroy(_sortButton);
            _sortButton = null;
        }

        if (_executeButton != null)
        {
            Object.Destroy(_executeButton);
            _executeButton = null;
        }

        if (_orderButton == null) return;
        Object.Destroy(_orderButton);
        _orderButton = null;
    }

    private static GameObject CreateSmallButton(
        Transform parent, string name, int uiLayer,
        string buttonText, float xPos, float yPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.layer = uiLayer;

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(48f, 48f);
        rect.anchoredPosition = new Vector2(xPos, yPos);

        var image = go.AddComponent<Image>();
        image.raycastTarget = true;
        image.color = Color.white;

        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(go.transform, false);
        fillGo.layer = uiLayer;
        var fillRect = fillGo.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = new Vector2(-4f, -4f);
        var fillImage = fillGo.AddComponent<Image>();
        fillImage.color = Color.black;
        fillImage.raycastTarget = false;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        textGo.layer = uiLayer;
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = buttonText;
        tmp.fontSize = 16f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var oldBtn = go.GetComponent<Button>();
        if (oldBtn != null)
            Object.Destroy(oldBtn);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = fillImage;
        btn.colors = new ColorBlock
        {
            normalColor = new Color(0f, 0f, 0f, 1f),
            highlightedColor = new Color(0f, 0f, 0f, 1f),
            pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        btn.navigation = new Navigation { mode = Navigation.Mode.None };

        return go;
    }

    private static void AddTooltip(GameObject target, string name, string desc)
    {
        var old = target.GetComponent<UITooltip>();
        if (old != null)
            Object.Destroy(old);
        var tip = target.AddComponent<UITooltip>();
        tip.tipName = name;
        tip.tipDesc = desc;
        tip.skipLocale = true;
    }

    private static void AddSortModeClickEvent(GameObject button, PlayerCamera camera)
    {
        var trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(data =>
        {
            var ped = (PointerEventData)data;
            switch (ped.button)
            {
                case PointerEventData.InputButton.Left:
                case PointerEventData.InputButton.Right:
                {
                    var modes = Enum.GetValues(typeof(SortMode));
                    _currentSortMode = (SortMode)(((int)_currentSortMode + 1) % modes.Length);
                    UpdateSortButtonText();
                    UpdateOrderButtonText();
                    camera.PlayUISound(PlayerCamera.UISoundType.Click);
                    break;
                }
                case PointerEventData.InputButton.Middle:
                default:
                    break;
            }
        });
        trigger.triggers.Add(entry);
    }

    private static void AddSortExecuteClickEvent(GameObject button, PlayerCamera camera)
    {
        var trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(data =>
        {
            var ped = (PointerEventData)data;
            switch (ped.button)
            {
                case PointerEventData.InputButton.Left:
                case PointerEventData.InputButton.Right:
                    SortContainer(camera);
                    return;
                case PointerEventData.InputButton.Middle:
                default:
                    break;
            }
        });
        trigger.triggers.Add(entry);
    }

    private static void AddOrderClickEvent(GameObject button, PlayerCamera camera)
    {
        var trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(data =>
        {
            var ped = (PointerEventData)data;
            if (ped.button is not (PointerEventData.InputButton.Left or PointerEventData.InputButton.Right))
                return;

            _currentSortAscending = !_currentSortAscending;

            var tmp = _orderButton?.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = _currentSortAscending
                    ? "\u2191"
                    : "\u2193";

            AddTooltip(_orderButton,
                Locale("ui.sort.order_tip"),
                _currentSortAscending
                    ? Locale("ui.sort.ascending")
                    : Locale("ui.sort.descending"));

            camera.PlayUISound(PlayerCamera.UISoundType.Click);
        });
        trigger.triggers.Add(entry);
    }

    private static string GetSortModeLabel(SortMode mode)
    {
        return mode switch
        {
            SortMode.Name => Locale("ui.sort.mode.name"),
            SortMode.Value => Locale("ui.sort.mode.value"),
            SortMode.Weight => Locale("ui.sort.mode.weight"),
            _ => "?"
        };
    }

    private static void UpdateSortButtonText()
    {
        var tmp = _sortButton?.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            var label = GetSortModeLabel(_currentSortMode);
            tmp.text = label.Length > 0
                ? label.Substring(0, 1)
                : "?";
        }

        AddTooltip(_sortButton,
            Locale("ui.sort.mode_tip"),
            Locale("ui.sort.mode_desc", GetSortModeLabel(_currentSortMode)));
    }

    private static void UpdateOrderButtonText()
    {
        AddTooltip(_orderButton,
            Locale("ui.sort.order_tip"),
            _currentSortAscending
                ? Locale("ui.sort.ascending")
                : Locale("ui.sort.descending"));
    }

    private static void ShowSortNotification(bool noChange)
    {
        var modeLabel = _currentSortMode switch
        {
            SortMode.Name => Locale("ui.sort.mode.name"),
            SortMode.Value => Locale("ui.sort.mode.value"),
            SortMode.Weight => Locale("ui.sort.mode.weight"),
            _ => ""
        };
        var orderLabel = _currentSortAscending
            ? Locale("ui.sort.ascending")
            : Locale("ui.sort.descending");

        Alert(noChange
            ? Locale("ui.sort.no_change")
            : Locale("ui.sort.completed", modeLabel, orderLabel));
    }

    private static string Locale(string key, params object[] args)
    {
        return BetterLocale.GetOther(key, args);
    }

    private static void Alert(string text)
    {
        PlayerCamera.main.DoAlert(text);
    }

    private enum SortMode
    {
        Name,
        Value,
        Weight
    }
}
