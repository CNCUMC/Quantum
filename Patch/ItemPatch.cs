using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using MossLib.Tool;
using Newtonsoft.Json.Linq;

namespace Quantum.Patch;

[HarmonyPatch(typeof(Item))]
public static class ItemPatch
{
    private const string LocaleKeyPre = "item.";

    // 日志本地化前缀，同 PlayerCameraPatch 格式
    private const string LogLocaleKeyPre = "log.item_patch.";

    private static readonly Dictionary<string, Dictionary<string, string>> LangCache = new();
    
    private static readonly HashSet<string> AlertedItemIds = [];
    private static double _lastDurabilityCheckTime;
    private const double DurabilityCheckInterval = 2.0;
    private static int _lastCheckedFrame = -1;

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static void UpdatePrefix()
    {
        if (Item.GlobalItems == null)
            return;

        var currentFrame = Time.frameCount;
        if (currentFrame == _lastCheckedFrame)
            return;
        _lastCheckedFrame = currentFrame;

        var now = Time.realtimeSinceStartup;
        if (now - _lastDurabilityCheckTime < DurabilityCheckInterval)
            return;
        _lastDurabilityCheckTime = now;

        var threshold = Plugin.FavouritedItemDurabilityExhaustionAlert.Value;

        if (threshold <= 0f)
            return;

        var items = Inventory.GetAllItems();
        if (items == null)
            return;

        foreach (var item in items)
        {
            if (item == null || !item.favourited || item.id == null)
                continue;

            if (item.condition >= threshold)
            {
                AlertedItemIds.Remove(item.id);
                continue;
            }

            if (!AlertedItemIds.Add(item.id))
                continue;

            var itemName = item.fullName ?? item.id;
            var condPercent = Mathf.FloorToInt(item.condition * 100f);
            var thresholdPercent = Mathf.FloorToInt(threshold * 100f);

            LogAlert("durability_exhaustion_alert",
                itemName, thresholdPercent, condPercent);
        }
    }

    [HarmonyPatch("SetupItems")]
    [HarmonyPostfix]
    public static void SetupItemsPostfix()
    {
        if (Item.GlobalItems == null)
            return;

        var query = from kvp in Item.GlobalItems
            let itemId = kvp.Key
            let itemInfo = kvp.Value
            where itemInfo != null
            select new { itemId, itemInfo };

        foreach (var item in query)
        {
            var extra = BuildInfo(item.itemId, item.itemInfo);
            if (!string.IsNullOrEmpty(extra))
                item.itemInfo.description = AppendIfMissing(
                    item.itemInfo.description ?? "", extra);
        }
    }

    private static string BuildInfo(string id, ItemInfo info)
    {
        if (info == null)
            return null;


        var result = "";
        result += $"ID: {id}\n";

        // 双语名称：在物品原名后附加指定语言的翻译
        var bilingualCode = Plugin.BilingualName.Value?.Trim();
        if (!string.IsNullOrEmpty(bilingualCode)
            && !string.Equals(bilingualCode, global::Locale.currentLangName, StringComparison.OrdinalIgnoreCase))
        {
            var secondName = GetItemNameInLang(id, bilingualCode);
            if (!string.IsNullOrEmpty(secondName) && !info.fullName.Contains(secondName))
            {
                result += RichText.Italic($"({secondName})\n");
            }
        }

        if (!ModLocale.HasLocaleKey(LocaleKeyPre + id))
            return string.IsNullOrEmpty(result.Trim())
                ? null
                : result.TrimEnd('\n');
        result += Locale(id);
        result += "\n";

        return string.IsNullOrEmpty(result.Trim())
            ? null
            : result.TrimEnd('\n');
    }

    private static string GetItemNameInLang(string itemId, string langCode)
    {
        // 尝试从缓存获取
        if (LangCache.TryGetValue(langCode, out var mainDict))
            return mainDict?.TryGetValue(itemId, out var name) == true ? name : null;

        // 加载语言文件
        var path = $"{UnityEngine.Application.dataPath}/Lang/{langCode}.json";
        if (!File.Exists(path))
        {
            LangCache[langCode] = null;
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            var obj = JObject.Parse(json);
            mainDict = obj["main"]?.ToObject<Dictionary<string, string>>();
            LangCache[langCode] = mainDict; // 可能为 null（JSON 中无 main 字段时）
        }
        catch
        {
            LangCache[langCode] = null;
            return null;
        }

        return mainDict?.TryGetValue(itemId, out var result) == true ? result : null;
    }

    private static string AppendIfMissing(string current, string addition)
    {
        if (string.IsNullOrWhiteSpace(addition)
            || current.IndexOf(addition,
                StringComparison.OrdinalIgnoreCase) >= 0)
            return current;

        return string.IsNullOrWhiteSpace(current)
            ? addition
            : $"{current.TrimEnd()}\n\n{addition}";
    }

    private static string Locale(string key, params object[] args)
    {
        return ModLocale.GetFormat($"{LocaleKeyPre}{key}", args);
    }

    // ── 日志本地化包装器（同 PlayerCameraPatch Warning/Info/Error 格式） ──

    private static void LogAlert(string text, params object[] args)
    {
        Log.Alert(ModLocale.GetFormat(LogLocaleKeyPre + text, args), Plugin.Logger, false);
    }
}