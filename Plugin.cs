using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Antiquantum;

[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public const string Guid = "org.explosivehydra.antiquantum";
    public const string Name = "Antiquantum";
    public const string Version = "1.0.0";
    private readonly Harmony _harmony = new(Guid);

    public void Awake()
    {
        Logger = base.Logger;
        _harmony.PatchAll();
    }
}