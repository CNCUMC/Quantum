using HarmonyLib;

namespace Quantum.Mechanism;

[HarmonyPatch(typeof(Body))]
public class DontShit
{
    [HarmonyPatch("TheCoroutineThatMakesYouShitYourselfWhenUnconscious")]
    [HarmonyPrefix]
    public static bool TheCoroutineThatMakesYouShitYourselfWhenUnconsciousPrefix()
    {
        return !Plugin.DontShit;
    }
}
