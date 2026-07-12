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
    public static bool DontBiteLightbulb;
    public static bool DontShit;

    // Misc
    public static bool NoObserver;

    // UI
    public static bool AmmunitionUi = true;
    public static string BilingualName = "EN";
    public static float ConsoleScrollSpeed = 0.01f;
    public static KeyCode HiddenHud = KeyCode.F1;
    public static int MaxVisibleCandidates = 27;
    public static int MaxHistorySize = 100;
    public static bool NoDemoTips = true;
    public static KeyCode SortKey = KeyCode.E;

    public void Awake()
    {
        Logger = base.Logger;

        new ZhCnLangGenerator().Initialize(Logger);
        new EnLangGenerator().Initialize(Logger);

        // Info
        VideoBool("ctrl_to_expand", CtrlToExpand, v => CtrlToExpand = v);
        VideoFloat("favourited_item_durability_exhaustion_alert", FavouritedItemDurabilityExhaustionAlert, 0f, 1f,
            v => FavouritedItemDurabilityExhaustionAlert = v,
            v => Mathf.FloorToInt(v * 100f) + "%");

        // Item
        // Gun
        QuantumBool("auto_rack", AutoRack, v => AutoRack = v);
        QuantumBool("indestructible_gun", IndestructibleGun, v => IndestructibleGun = v);
        QuantumBool("infinite_ammunition", InfiniteAmmunition, v => InfiniteAmmunition = v);
        QuantumBool("never_jam", NeverJam, v => NeverJam = v);
        QuantumBool("no_casing", NoCasing, v => NoCasing = v);
        QuantumBool("recoilless", Recoilless, v => Recoilless = v);

        // Mechanism
        QuantumBool("dont_shit", DontShit, v => DontShit = v);
        QuantumBool("dont_bite_lightbulb", DontBiteLightbulb, v => DontBiteLightbulb = v);

        // Misc
        QuantumBool("no_observer", NoObserver, v => NoObserver = v);

        // UI
        VideoBool("ammunition_ui", AmmunitionUi, v => AmmunitionUi = v);
        RegisterBilingualOption();
        InputFloat("console_scroll_speed", ConsoleScrollSpeed, 0.0001f, 0.01f, v => ConsoleScrollSpeed = v, v => (v * 1000f).ToString("F1") + "ms");
        InputKeybind("hidden_hud", HiddenHud, k => HiddenHud = k);
        VideoFloat("max_visible_candidates", MaxVisibleCandidates, 1f, 50f, v => MaxVisibleCandidates = Convert.ToInt32(v), v => v.ToString("F0"));
        VideoFloat("max_history_size", MaxHistorySize, 1f, 500f, v => MaxHistorySize = Convert.ToInt32(v), v => v.ToString("F0"));
        VideoBool("no_demo_tips", NoDemoTips, v => NoDemoTips = v);
        InputKeybind("sort_key", SortKey, k => SortKey = k);

        BetterLocale.Flush();
        _harmony.PatchAll();

        UpdateUtil.Check("CNCUMC/Quantum", Name, Version, Logger);
    }

    private static void QuantumBool(string key, bool value, Action<bool> set)
    {
        BetterOptions.Bool(NameSpace, key, Name, value, set);
    }

    private static void VideoBool(string key, bool value, Action<bool> set)
    {
        BetterOptions.Bool(NameSpace, key, Setting.SettingCategory.Video, value, set);
    }

    private static void VideoFloat(string key, float value, float min, float max, Action<float> set,
        Func<float, string> fmt)
    {
        BetterOptions.Float(NameSpace, key, Setting.SettingCategory.Video, value, min, max, set, fmt);
    }

    private static void InputFloat(string key, float value, float min, float max, Action<float> set,
        Func<float, string> fmt)
    {
        BetterOptions.Float(NameSpace, key, Setting.SettingCategory.Input, value, min, max, set, fmt);
    }

    private static void VideoInt(string key, int value, int min, int max, Action<int> set)
    {
        BetterOptions.Int(NameSpace, key, Setting.SettingCategory.Video, value, min, max, set);
    }

    private static void InputKeybind(string key, KeyCode value, Action<KeyCode> set)
    {
        BetterOptions.Keybind(NameSpace, key, Setting.SettingCategory.Input, value, set);
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