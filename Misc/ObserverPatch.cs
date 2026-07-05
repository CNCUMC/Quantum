using HarmonyLib;

namespace Quantum.Misc;

[HarmonyPatch(typeof(Observer))]
public class ObserverPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix()
    {
        if (!Plugin.NoObserver) return true;

        Observer.main.gameObject.SetActive(false);
        return false;
    }
}