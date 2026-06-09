using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MossLib.Tool;
using Quantum.Lang;

namespace Quantum;

[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "org.explosivehydra.quantum";
    public const string Name = "Quantum";
    public const string Version = "1.0.0";
    internal new static ManualLogSource Logger;

    private static readonly Dictionary<string, ConfigEntryBase> Registry = new();

    public static ConfigEntry<bool> CtrlToExpand;
    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;

        LocaleGenerator.SetLogger(Logger);
        LocaleGenerator.Register(new EnLangGenerator(), Logger);
        LocaleGenerator.Register(new ZhCnLangGenerator(), Logger);
        LocaleGenerator.Register(new ZhTwLangGenerator(), Logger);
        LocaleGenerator.GenerateAll();

        _harmony.PatchAll();
        ModLocale.Initialize(Logger);

        CtrlToExpand = RegisterConfigInfo(Config, "ctrl_to_expand", true);
    }

    private static ConfigEntry<T> RegisterConfigInfo<T>(ConfigFile configFile, string key, T defaultValue)
    {
        return RegisterConfig(configFile, "Info", key, defaultValue);
    }

    private static ConfigEntry<T> RegisterConfig<T>(ConfigFile configFile, string section, string key, T defaultValue)
    {
        return MossLib.Tool.Config.Register(configFile, section, key, defaultValue,
            localeKey => ModLocale.GetFormat(localeKey), Registry);
    }
}