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

    private const string NameSpace = "quantum";

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

        // Info
        BetterOptions.Bool(NameSpace, "ctrl_to_expand", Setting.SettingCategory.Video,
            CtrlToExpand, v => CtrlToExpand = v);
        BetterOptions.Float(NameSpace, "favourited_item_durability_exhaustion_alert", Setting.SettingCategory.Video,
            FavouritedItemDurabilityExhaustionAlert, 0f, 1f,
            v => FavouritedItemDurabilityExhaustionAlert = v,
            v => Mathf.FloorToInt(v * 100f) + "%");

        // Item - Gun
        BetterOptions.Bool(NameSpace, "auto_rack", NameSpace, AutoRack, v => AutoRack = v);
        BetterOptions.Bool(NameSpace, "indestructible_gun", NameSpace, IndestructibleGun, v => IndestructibleGun = v);
        BetterOptions.Bool(NameSpace, "infinite_ammunition", NameSpace, InfiniteAmmunition,
            v => InfiniteAmmunition = v);
        BetterOptions.Bool(NameSpace, "never_jam", NameSpace, NeverJam, v => NeverJam = v);
        BetterOptions.Bool(NameSpace, "no_casing", NameSpace, NoCasing, v => NoCasing = v);
        BetterOptions.Bool(NameSpace, "recoilless", NameSpace, Recoilless, v => Recoilless = v);

        // Mechanism
        BetterOptions.Bool(NameSpace, "dont_shit", NameSpace, DontShit, v => DontShit = v);

        // Misc
        BetterOptions.Bool(NameSpace, "no_observer", NameSpace, NoObserver, v => NoObserver = v);

        // UI
        BetterOptions.Bool(NameSpace, "ammunition_ui", Setting.SettingCategory.Video, AmmunitionUi,
            v => AmmunitionUi = v);
        RegisterBilingualOption();
        BetterOptions.Keybind(NameSpace, "sort_key", Setting.SettingCategory.Input, SortKey, k => SortKey = k);
        BetterOptions.Float(NameSpace, "console_scroll_speed", Setting.SettingCategory.Input,
            ConsoleScrollSpeed, 0.01f, 0.2f,
            v => ConsoleScrollSpeed = v,
            v => (v * 1000f).ToString("F0") + "ms");
        BetterOptions.Int(NameSpace, "max_visible_candidates", Setting.SettingCategory.Video,
            MaxVisibleCandidates, 1, 200,
            v => MaxVisibleCandidates = v);
        BetterOptions.Int(NameSpace, "max_history_size", Setting.SettingCategory.Video,
            MaxHistorySize, 10, 500,
            v => MaxHistorySize = v);
        BetterOptions.Bool(NameSpace, "no_demo_tips", Setting.SettingCategory.Video, NoDemoTips, v => NoDemoTips = v);

        BetterLocale.Flush();
        _harmony.PatchAll();
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
        BetterOptions.Dropdown(NameSpace, "bilingual_name", Setting.SettingCategory.Video,
            0, arr,
            i => BilingualName = i > 0 && i < arr.Length ? arr[i].Key : "");
    }
}