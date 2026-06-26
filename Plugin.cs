using Bark.BetterCCL;
using BepInEx;
using CUCoreLib.Data;
using BepInEx.Logging;
using HarmonyLib;
using Quantum.Lang;
using UnityEngine;

namespace Quantum;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency("net.cucorelib", "1.0.1")]
[BepInDependency("org.cncumc.bark", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.explosivehydra.quantum";
    public const string Name = "Quantum";
    public const string Version = "1.1.0";
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
    public static int MaxVisibleCandidates = 27;
    public static int MaxHistorySize = 100;

    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;

        new ZhCnLangGenerator().Initialize(Logger);
        new EnLangGenerator().Initialize(Logger);

        // Info
        BetterOptions.Bool(NameSpace, "ctrl_to_expand", NameSpace,
            CtrlToExpand, v => CtrlToExpand = v);
        BetterOptions.Float(NameSpace, "favourited_item_durability_exhaustion_alert", NameSpace,
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
        BetterOptions.Bool(NameSpace, "ammunition_ui", NameSpace, AmmunitionUi, v => AmmunitionUi = v);

        // BilingualName: dropdown
        var bilingualChoices = new[]
        {
            new ModDropdownChoice("off", "Off"),
            new ModDropdownChoice("EN", "EN"),
            new ModDropdownChoice("zh-CN", "zh-CN"),
            new ModDropdownChoice("zh-TW", "zh-TW"),
        };
        BetterOptions.Dropdown(NameSpace, "bilingual_name", NameSpace,
            0, bilingualChoices,
            i => BilingualName = i > 0 && i < bilingualChoices.Length
                ? bilingualChoices[i].Key
                : "");

        BetterOptions.Keybind(NameSpace, "sort_key", Setting.SettingCategory.Input, SortKey, k => SortKey = k);
        BetterOptions.Int(NameSpace, "max_visible_candidates", NameSpace, MaxVisibleCandidates, 1, 200,
            v => MaxVisibleCandidates = v);
        BetterOptions.Int(NameSpace, "max_history_size", NameSpace, MaxHistorySize, 10, 200,
            v => MaxHistorySize = v);

        BetterLocale.Flush();
        _harmony.PatchAll();
    }
}