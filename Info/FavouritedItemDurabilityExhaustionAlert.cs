using System.Collections.Generic;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using CUCoreLib.Helpers;
using HarmonyLib;
using UnityEngine;

namespace Quantum.Info;

[HarmonyPatch(typeof(Item))]
public static class FavouritedItemDurabilityExhaustionAlert
{
    private const double DurabilityCheckInterval = 2.0;

    private const string LogLocaleKeyPre = "log.item_patch.";

    // item.id -> 上次提醒时的耐久百分比 (0-100)，每下降 >=5% 提醒一次
    private static readonly Dictionary<string, int> AlertedItemPercents = [];
    private static double _lastDurabilityCheckTime;
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

        var threshold = Plugin.FavouritedItemDurabilityExhaustionAlert;

        if (threshold <= 0f)
            return;

        var items = InventoryUtil.GetAllItems();

        const int alertStep = 5; // 每下降 5% 提醒一次

        foreach (var item in items
                     .Where(item => item != null
                                    && item.favourited
                                    && item.id != null))
        {
            var condPercent = Mathf.FloorToInt(item.condition * 100f);

            if (item.condition >= threshold)
            {
                // 耐久恢复至阈值以上，清除记录
                AlertedItemPercents.Remove(item.id);
                continue;
            }

            // 首次低于阈值或下降 >= 5% 时提醒
            if (!AlertedItemPercents.TryGetValue(item.id, out var lastPercent))
                // 首次：直接提醒，记录当前百分比
                AlertedItemPercents[item.id] = condPercent;
            else if (lastPercent - condPercent >= alertStep)
                // 下降了 >= 5%，再次提醒，更新记录
                AlertedItemPercents[item.id] = condPercent;
            else
                continue;

            var itemName = item.fullName ?? item.id;

            LogAlert("durability_exhaustion_alert",
                itemName, condPercent);
        }
    }

    private static void LogAlert(string text, params object[] args)
    {
        CUCoreUtils.ShowAlert(BetterLocale.GetOther(LogLocaleKeyPre + text, args));
    }
}