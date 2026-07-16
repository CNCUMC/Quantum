using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Quantum.Misc;

[HarmonyPatch(typeof(TutorialHandler))]
public class AutoSandbox
{
    private static bool _autoStarted;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(TutorialHandler __instance)
    {
        _autoStarted = false;
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(TutorialHandler __instance)
    {
        if (!Plugin.AutoSandbox || _autoStarted) return;

        if (__instance.courseSelectScreen == null || !__instance.courseSelectScreen.activeSelf
                                                  || __instance.activeCourse != null) return;
        var sandboxIndex = TutorialHandler.availableCourses
            .FindIndex(c => c.localeName == "sandbox");

        if (sandboxIndex < 0) return;
        __instance.selectedCourse = sandboxIndex;
                
        var buttons = __instance.courseSelectScreen.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            if (!btn.interactable || !btn.gameObject.activeInHierarchy) continue;
            btn.onClick.Invoke();
            _autoStarted = true;
            return;
        }
    }
}
