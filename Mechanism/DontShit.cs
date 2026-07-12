using System.Collections;
using HarmonyLib;

namespace Quantum.Mechanism;

[HarmonyPatch(typeof(Body))]
public class DontShit
{
    [HarmonyPatch("TheCoroutineThatMakesYouShitYourselfWhenUnconscious")]
    [HarmonyPrefix]
    public static bool Prefix(out IEnumerator __result)
    {
        if (Plugin.DontShit)
        {
            __result = EmptyCoroutine();
            return false;
        }
        __result = null;
        return true;
    }

    private static IEnumerator EmptyCoroutine()
    {
        yield break;
    }
}
