using System;
using System.Collections.Generic;
using System.IO;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
using BepInEx.Logging;
using CUCoreLib.Data;
using HarmonyLib;
using UnityEngine;

namespace Quantum;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("net.cucorelib", "1.0.3")]
[BepInDependency("org.cncumc.bark", "1.1.1")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.cncumc.quantum";
    public const string Name = "Quantum";
    public const string Version = "1.1.1";
    internal const string NameSpace = "quantum";
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
    public static bool AutoSandbox;

    // Video
    public static bool AmmunitionUi = true;
    public static string BilingualName = "EN";
    public static float ConsoleParameterSwitchingSpeed = 0.05f;
    public static KeyCode DebugScreen = KeyCode.F3;
    public static KeyCode DebugScreenFpsGraph = KeyCode.F;
    public static float DebugScreenSpeed = 0.01f;
    public static KeyCode HiddenHud = KeyCode.F1;
    public static int MaxVisibleCandidates = 27;
    public static int MaxHistorySize = 100;
    public static bool NoDemoTips = true;
    public static KeyCode SortKey = KeyCode.E;
    public static KeyCode ZoomKey = KeyCode.Backslash;
    public static float ZoomMultiplier = 1f;
    public static float ZoomSensitivity = 0.5f;
    public static bool SmoothZoom = true;

    public void Awake()
    {
        Logger = base.Logger;

        new LangGenerator().Initialize(Logger);

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
        QuantumBool("auto_sandbox", AutoSandbox, v => AutoSandbox = v);
        
        // Input
        InputFloat("console_parameter_switching_speed", ConsoleParameterSwitchingSpeed, 0f, 0.1f,
            v => ConsoleParameterSwitchingSpeed = v,
            v => v <= 0f
                ? "0"
                : (v * 1000f).ToString("F0") + "ms");
        InputKeybind("debug_screen", DebugScreen, k => DebugScreen = k);
        InputKeybind("debug_screen_fps_graph", DebugScreenFpsGraph, k => DebugScreenFpsGraph = k);
        InputKeybind("sort_key", SortKey, k => SortKey = k);
        InputKeybind("zoom_key", ZoomKey, k => ZoomKey = k);

        // Video
        VideoBool("ctrl_to_expand", CtrlToExpand, v => CtrlToExpand = v);
        VideoFloat("favourited_item_durability_exhaustion_alert", FavouritedItemDurabilityExhaustionAlert, 0f, 1f,
            v => FavouritedItemDurabilityExhaustionAlert = v,
            v => Mathf.FloorToInt(v * 100f) + "%");
        VideoBool("ammunition_ui", AmmunitionUi, v => AmmunitionUi = v);
        RegisterBilingualOption();
        VideoFloat("debug_screen_speed", DebugScreenSpeed, 0f, 0.1f,
            v => DebugScreenSpeed = v,
            v => (v * 1000f).ToString("F1") + "ms");
        InputKeybind("hidden_hud", HiddenHud, k => HiddenHud = k);
        VideoFloat("max_visible_candidates", MaxVisibleCandidates, 1f, 50f,
            v => MaxVisibleCandidates = Convert.ToInt32(v), v => v.ToString("F0"));
        VideoFloat("max_history_size", MaxHistorySize, 1f, 500f, v => MaxHistorySize = Convert.ToInt32(v),
            v => v.ToString("F0"));
        VideoBool("no_demo_tips", NoDemoTips, v => NoDemoTips = v);
        VideoFloat("zoom_sensitivity", ZoomSensitivity, 0.1f, 2f,
            v => ZoomSensitivity = v,
            v => v.ToString("F1"));
        VideoBool("smooth_zoom", SmoothZoom, v => SmoothZoom = v);

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
                    BetterLocale.SetDefault("EN", NameSpace, "option", $"video.bilingual_name{code}", code);
                    choices.Add(new ModDropdownChoice(code, code));
                }
        }
        catch (Exception ex)
        {
            LogUtil.Warning($"Failed to scan language directory '{langDir}': {ex.Message}", Logger);
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