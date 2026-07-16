using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Quantum.ItemChange;

[HarmonyPatch(typeof(Item))]
public static class ItemDescriptionPatch
{
    private const string LocaleKeyPre = "item_description_patch";

    private static readonly Dictionary<string, Dictionary<string, string>> LangCache = new();

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
        var bilingualCode = Plugin.BilingualName?.Trim();
        if (!string.IsNullOrEmpty(bilingualCode)
            && !string.Equals(bilingualCode, Locale.currentLangName, StringComparison.OrdinalIgnoreCase))
        {
            var secondName = GetItemNameInLang(id, bilingualCode);
            if (!string.IsNullOrEmpty(secondName) && !info.fullName.Contains(secondName))
                result += TextUtil.Italic($"({secondName})\n");
        }

        if (!BetterLocale.HasKeyOther(LocaleKeyPre + id))
            return string.IsNullOrEmpty(result.Trim())
                ? null
                : result.TrimEnd('\n');
        result += LocaleOther(id);
        result += "\n";

        return string.IsNullOrEmpty(result.Trim())
            ? null
            : result.TrimEnd('\n');
    }

    private static string GetItemNameInLang(string itemId, string langCode)
    {
        // 尝试从缓存获取
        if (LangCache.TryGetValue(langCode, out var mainDict))
            return mainDict?.TryGetValue(itemId, out var name) == true
                ? name
                : null;

        // 加载语言文件
        var path = $"{Application.dataPath}/Lang/{langCode}.json";
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
            LangCache[langCode] = mainDict;
        }
        catch
        {
            LangCache[langCode] = null;
            return null;
        }

        return mainDict?.TryGetValue(itemId, out var result) == true
            ? result
            : null;
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

    private static string LocaleOther(string key, params object[] args)
    {
        return BetterLocale.GetOther($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }
}