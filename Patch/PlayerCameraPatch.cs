using System;
using System.Collections.Generic;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using HarmonyLib;
using Quantum.Video;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Quantum.Patch;

[HarmonyPatch(typeof(PlayerCamera))]
public static class PlayerCameraPatch
{
    private const string LocaleKeyPre = "player_camera_patch";

    private static readonly Dictionary<string, string> PinyinCache = new();
    private static bool _pinyinInitialized;
    private static bool _hasPinyinLibrary;
    private static Func<string, string, string> _getPinyin;
    private static Func<string, string, string> _getPinyinInitials;
    private static string _pinyinFilterCache;

    private static void EnsurePinyinLibrary()
    {
        if (_pinyinInitialized)
            return;
        _pinyinInitialized = true;

        try
        {
            var type = Type.GetType("TinyPinyin.PinyinHelper, TinyPinyin");
            if (type == null)
            {
                Warning("pinyin.library.not_found");
                return;
            }

            var getPinyin = type.GetMethod("GetPinyin", [typeof(string), typeof(string)]);
            var getInitials = type.GetMethod("GetPinyinInitials", [typeof(string), typeof(string)]);

            if (getPinyin == null || getInitials == null)
            {
                Warning("pinyin.api_not_found");
                return;
            }

            _getPinyin = (str, sep) => (string)getPinyin.Invoke(null, [str, sep]);
            _getPinyinInitials = (str, sep) => (string)getInitials.Invoke(null, [str, sep]);
            _hasPinyinLibrary = true;
        }
        catch (Exception ex)
        {
            Warning("pinyin.init_failed", ex.Message);
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerCamera __instance)
    {
        if (Input.GetKeyDown(Plugin.HiddenHud))
        {
            HiddenHud.Hidden = !HiddenHud.Hidden;
            HiddenHud.ApplyVisibility(__instance);
        }

        if (Input.GetKeyDown(Plugin.DebugScreen)) DebugScreen.Hidden = !DebugScreen.Hidden;

        if (Input.GetKey(Plugin.DebugScreen) && Input.GetKeyDown(Plugin.DebugScreenFpsGraph))
            DebugScreen.ShowFpsGraph = !DebugScreen.ShowFpsGraph;
    }

    [HarmonyPatch("RefreshRecipeList")]
    [HarmonyPrefix]
    private static void PreRefreshRecipeList(PlayerCamera __instance)
    {
        _pinyinFilterCache = null;

        var filter = __instance.recipeFilter;
        if (string.IsNullOrEmpty(filter))
            return;

        // 如果 filter 包含非 ASCII 字符（中文直接输入），走原始逻辑即可
        var isAscii = filter.All(t => t <= 127);

        if (!isAscii)
            return;

        // 纯 ASCII → 可能是拼音输入，绕过原始过滤
        EnsurePinyinLibrary();
        if (!_hasPinyinLibrary)
            return;

        _pinyinFilterCache = filter;
        __instance.recipeFilter = "";
    }

    [HarmonyPatch("RefreshRecipeList")]
    [HarmonyPostfix]
    private static void PostRefreshRecipeList(PlayerCamera __instance)
    {
        var rawFilter = _pinyinFilterCache;
        _pinyinFilterCache = null;
        if (rawFilter == null)
            return;

        var cleanFilter = rawFilter.Replace(" ", "").ToUpperInvariant();

        // recipeObjects 是 PlayerCamera 的私有字段，通过反射访问
        var recipeObjectsField = AccessTools.Field(typeof(PlayerCamera), "recipeObjects");
        var recipeObjects = (List<GameObject>)recipeObjectsField.GetValue(__instance);
        if (recipeObjects == null || recipeObjects.Count == 0)
            return;

        // 先重排：将匹配项推到列表前面，不匹配项推到后面
        // 再过滤：销毁不匹配项
        var matched = new List<GameObject>(recipeObjects.Count);
        var unmatched = new List<GameObject>(recipeObjects.Count);

        foreach (var go in recipeObjects)
        {
            var displayName = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (IsPinyinMatch(displayName, cleanFilter))
                matched.Add(go);
            else
                unmatched.Add(go);
        }

        // 重排：匹配项在前，不匹配项在后
        recipeObjects.Clear();
        recipeObjects.AddRange(matched);
        recipeObjects.AddRange(unmatched);

        // 过滤：销毁不匹配项
        foreach (var go in unmatched)
            Object.Destroy(go);

        // 修正匹配项的 anchoredPosition，使其连续排列在顶部
        // 原始代码中每个 recipe 的位置是 -index * 64
        for (var i = 0; i < matched.Count; i++)
        {
            var rect = matched[i].GetComponent<RectTransform>();
            if (rect != null)
                rect.anchoredPosition = new Vector2(-9f, -i * 64);
        }

        // 修正 recipeListContent 的 sizeDelta 以适应新的数量
        var recipeListContent =
            (RectTransform)AccessTools.Field(typeof(PlayerCamera), "recipeListContent").GetValue(__instance);
        if (recipeListContent != null)
            recipeListContent.sizeDelta = new Vector2(1f, matched.Count * 64);
    }

    private static bool IsPinyinMatch(string displayName, string filter)
    {
        // 1. 原名匹配
        if (displayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        // 2. 拼音匹配
        EnsurePinyinLibrary();
        if (!_hasPinyinLibrary)
            return false;

        if (!PinyinCache.TryGetValue(displayName, out var pinyin))
        {
            try
            {
                var full = _getPinyin(displayName, "");
                var initials = _getPinyinInitials(displayName, "");
                pinyin = full + "\n" + initials;
            }
            catch
            {
                pinyin = "";
            }

            PinyinCache[displayName] = pinyin;
        }

        if (string.IsNullOrEmpty(pinyin))
            return false;

        var parts = pinyin.Split('\n');

        // 全拼匹配
        if (parts[0].IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        // 首字母简码匹配
        if (parts.Length > 1 && parts[1].IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        // 逐字符顺序匹配：用于 "bdai" → "BENGDAI" 这类模糊拼音
        // 检查 filter 的每个字符是否按顺序出现在全拼中
        if (SequentialMatch(parts[0], filter))
            return true;

        // 简码也尝试逐字符
        return parts.Length > 1 && SequentialMatch(parts[1], filter);
    }

    private static bool SequentialMatch(string text, string pattern)
    {
        var ti = 0;
        foreach (var t in pattern)
        {
            ti = text.IndexOf(t, ti);
            if (ti < 0)
                return false;
            ti++;
        }

        return true;
    }

    private static string LocaleLog(string key, params object[] args)
    {
        return BetterLocale.GetLog($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }

    private static void Warning(string text, params object[] args)
    {
        LogUtil.Warning(LocaleLog(LocaleKeyPre + text, args), Plugin.Logger);
    }
}