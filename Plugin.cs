using System;
using System.Collections.Generic;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
using BepInEx.Logging;
using CUCoreLib.Data;
using HarmonyLib;
using Quantum.Lang;
using UnityEngine;

namespace Quantum;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("net.cucorelib", "1.0.2")]
[BepInDependency("org.cncumc.bark", "1.0.3")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.quantum";
    public const string Name = "Quantum";
    public const string Version = "1.0.1";

    private const string NameSpace = "quantum";
    internal new static ManualLogSource Logger;
    private readonly Harmony _harmony = new(Guid);

    // Info
    public static bool CtrlToExpand = true;
    public static float FavouritedItemDurabilityExhaustionAlert = 0.3f;

    // Item - Gun
    public static bool AutoRack;
    public static bool IndestructibleGun;
    public static bool InfiniteAmmunition;
    public static bool NeverJam;
    public static bool NoCasing;
    public static bool Recoilless;

    // Mechanism
    public static bool DontShit;
    public static bool DontBiteLightbulb;

    // Misc
    public static bool NoObserver;

    // UI
    public static bool AmmunitionUi = true;
    public static string BilingualName = "EN";
    public static KeyCode SortKey = KeyCode.E;
    public static float ConsoleScrollSpeed = 0.1f;
    public static int MaxVisibleCandidates = 27;
    public static int MaxHistorySize = 100;
    public static bool NoDemoTips = true;

    public void Awake()
    {
        Logger = base.Logger;

        new ZhCnLangGenerator().Initialize(Logger);
        new EnLangGenerator().Initialize(Logger);

        // Info
        BoolVideo("ctrl_to_expand", CtrlToExpand, v => CtrlToExpand = v);
        FloatVideo("favourited_item_durability_exhaustion_alert", FavouritedItemDurabilityExhaustionAlert, 0f, 1f,
            v => FavouritedItemDurabilityExhaustionAlert = v,
            v => Mathf.FloorToInt(v * 100f) + "%");

        // Item
        // Gun
        BoolQuantum("auto_rack", AutoRack, v => AutoRack = v);
        BoolQuantum("indestructible_gun", IndestructibleGun, v => IndestructibleGun = v);
        BoolQuantum("infinite_ammunition", InfiniteAmmunition, v => InfiniteAmmunition = v);
        BoolQuantum("never_jam", NeverJam, v => NeverJam = v);
        BoolQuantum("no_casing", NoCasing, v => NoCasing = v);
        BoolQuantum("recoilless", Recoilless, v => Recoilless = v);
        
        // Mechanism
        BoolQuantum("dont_shit", DontShit, v => DontShit = v);
        BoolQuantum("dont_bite_lightbulb", DontBiteLightbulb, v => DontBiteLightbulb = v);
        
        // Misc
        BoolQuantum("no_observer", NoObserver, v => NoObserver = v);

        // UI
        BoolVideo("ammunition_ui", AmmunitionUi, v => AmmunitionUi = v);
        RegisterBilingualOption();
        KeyInput("sort_key", SortKey, k => SortKey = k);
        FloatInput("console_scroll_speed", ConsoleScrollSpeed, 0.01f, 0.2f,
            v => ConsoleScrollSpeed = v,
            v => (v * 1000f).ToString("F0") + "ms");
        IntVideo("max_visible_candidates", MaxVisibleCandidates, 1, 200,
            v => MaxVisibleCandidates = v);
        IntVideo("max_history_size", MaxHistorySize, 10, 500,
            v => MaxHistorySize = v);
        BoolVideo("no_demo_tips", NoDemoTips, v => NoDemoTips = v);

        BetterLocale.Flush();
        _harmony.PatchAll();
        
        UpdateUtil.Check("CNCUMC/Quantum", Name, Version, Logger);
    }

    private static void BoolQuantum(string key, bool val, Action<bool> set)
    {
        BetterOptions.Bool(NameSpace, key, Name, val, set);
    }

    private static void BoolVideo(string key, bool val, Action<bool> set)
    {
        BetterOptions.Bool(NameSpace, key, Setting.SettingCategory.Video, val, set);
    }

    private static void FloatVideo(string key, float val, float min, float max, Action<float> set, Func<float, string> fmt)
    {
        BetterOptions.Float(NameSpace, key, Setting.SettingCategory.Video, val, min, max, set, fmt);
    }

    private static void FloatInput(string key, float val, float min, float max, Action<float> set, Func<float, string> fmt)
    {
        BetterOptions.Float(NameSpace, key, Setting.SettingCategory.Input, val, min, max, set, fmt);
    }

    private static void IntVideo(string key, int val, int min, int max, Action<int> set)
    {
        BetterOptions.Int(NameSpace, key, Setting.SettingCategory.Video, val, min, max, set);
    }

    private static void KeyInput(string key, KeyCode val, Action<KeyCode> set)
    {
        BetterOptions.Keybind(NameSpace, key, Setting.SettingCategory.Input, val, set);
    }

    private static void RegisterBilingualOption()
    {
        var choices = new List<ModDropdownChoice>();
        var langDir = $"{Application.dataPath}/Lang";
        try
        {
            if (Directory.Exists(langDir))
                foreach (var file in Directory.GetFiles(langDir, "*.json"))
                {
                    var code = Path.GetFileNameWithoutExtension(file);
                    BetterLocale.SetDefault("EN", "option", $"quantum.video.bilingual_name{code}", code);
                    choices.Add(new ModDropdownChoice(code, code));
                }
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Failed to scan language directory '{langDir}': {ex.Message}");
        }

        var arr = choices.ToArray();
        var defaultIndex = Math.Max(0, Array.FindIndex(arr, c => c.Key == "EN"));
        BetterOptions.Dropdown(NameSpace, "bilingual_name", Setting.SettingCategory.Video,
            defaultIndex, arr,
            i => BilingualName = i >= 0 && i < arr.Length
                ? arr[i].Key
                : "EN");
    }
}