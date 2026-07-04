using System;
using System.Collections.Generic;
using System.IO;
using Bark.BetterCCL;
using BepInEx;
using CUCoreLib.Data;
using BepInEx.Logging;
using HarmonyLib;
using Quantum.Lang;
using UnityEngine;

namespace Quantum;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("net.cucorelib", "1.0.2")]
[BepInDependency("org.cncumc.bark", "1.0.2")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.quantum";
    public const string Name = "Quantum";
    public const string Version = "1.0.0";
    internal new static ManualLogSource Logger;

    private const string Ns = "quantum";

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
    public static bool DontShit = true;

    // Misc
    public static bool NoObserver;

    // UI
    public static bool AmmunitionUi = true;
    public static string BilingualName = "";
    public static KeyCode SortKey = KeyCode.E;
    public static float ConsoleScrollSpeed = 0.1f;
    public static int MaxVisibleCandidates = 27;
    public static int MaxHistorySize = 100;
    public static bool NoDemoTips = true;

    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;

        new ZhCnLangGenerator().Initialize(Logger);
        new EnLangGenerator().Initialize(Logger);

        // Info (Video tab)
        BoolV("ctrl_to_expand", CtrlToExpand, v => CtrlToExpand = v);
        FloatV("favourited_item_durability_exhaustion_alert", FavouritedItemDurabilityExhaustionAlert, 0f, 1f,
            v => FavouritedItemDurabilityExhaustionAlert = v,
            v => Mathf.FloorToInt(v * 100f) + "%");

        // Gun / Mechanism / Misc (Quantum tab)
        BoolQ("auto_rack", AutoRack, v => AutoRack = v);
        BoolQ("indestructible_gun", IndestructibleGun, v => IndestructibleGun = v);
        BoolQ("infinite_ammunition", InfiniteAmmunition, v => InfiniteAmmunition = v);
        BoolQ("never_jam", NeverJam, v => NeverJam = v);
        BoolQ("no_casing", NoCasing, v => NoCasing = v);
        BoolQ("recoilless", Recoilless, v => Recoilless = v);
        BoolQ("dont_shit", DontShit, v => DontShit = v);
        BoolQ("no_observer", NoObserver, v => NoObserver = v);

        // UI
        BoolV("ammunition_ui", AmmunitionUi, v => AmmunitionUi = v);
        RegisterBilingualOption();
        KeyI("sort_key", SortKey, k => SortKey = k);
        FloatI("console_scroll_speed", ConsoleScrollSpeed, 0.01f, 0.2f,
            v => ConsoleScrollSpeed = v,
            v => (v * 1000f).ToString("F0") + "ms");
        IntV("max_visible_candidates", MaxVisibleCandidates, 1, 200,
            v => MaxVisibleCandidates = v);
        IntV("max_history_size", MaxHistorySize, 10, 500,
            v => MaxHistorySize = v);
        BoolV("no_demo_tips", NoDemoTips, v => NoDemoTips = v);

        BetterLocale.Flush();
        _harmony.PatchAll();
    }

    // ── 注册辅助方法 ──

    private static void BoolQ(string key, bool val, Action<bool> set) =>
        BetterOptions.Bool(Ns, key, Ns, val, set);

    private static void BoolV(string key, bool val, Action<bool> set) =>
        BetterOptions.Bool(Ns, key, Setting.SettingCategory.Video, val, set);

    private static void FloatV(string key, float val, float min, float max, Action<float> set, Func<float, string> fmt) =>
        BetterOptions.Float(Ns, key, Setting.SettingCategory.Video, val, min, max, set, fmt);

    private static void FloatI(string key, float val, float min, float max, Action<float> set, Func<float, string> fmt) =>
        BetterOptions.Float(Ns, key, Setting.SettingCategory.Input, val, min, max, set, fmt);

    private static void IntV(string key, int val, int min, int max, Action<int> set) =>
        BetterOptions.Int(Ns, key, Setting.SettingCategory.Video, val, min, max, set);

    private static void KeyI(string key, KeyCode val, Action<KeyCode> set) =>
        BetterOptions.Keybind(Ns, key, Setting.SettingCategory.Input, val, set);
    
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
        BetterOptions.Dropdown(Ns, "bilingual_name", Setting.SettingCategory.Video,
            0, arr, 
            i => BilingualName = i > 0 && i < arr.Length ? arr[i].Key : "");
    }
}
