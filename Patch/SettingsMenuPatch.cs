using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CUCoreLib.Helpers;
using HarmonyLib;
using UnityEngine;

namespace Quantum.Patch;

[HarmonyPatch(typeof(SettingsMenu))]
public class SettingsMenuPatch
{
    private static int _currentTabIndex;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(SettingsMenu __instance)
    {
        _currentTabIndex = 0;
        CUCoreUtils.StartCoroutine(InputLoop());
    }

    [HarmonyPatch("SelectTab", typeof(int))]
    [HarmonyPrefix]
    private static void SelectTabPrefix(int index)
    {
        _currentTabIndex = index;
    }

    [HarmonyPatch("Close")]
    [HarmonyPostfix]
    private static void ClosePostfix()
    {
        SettingsMenu.instance = null;
    }

    private static IEnumerator InputLoop()
    {
        while (SettingsMenu.instance != null)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SettingsMenu.instance.Close();
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var categories = GetAllCategoryIndices();
                if (categories.Count > 0)
                {
                    var currentListIndex = categories.IndexOf(_currentTabIndex);
                    if (currentListIndex < 0) currentListIndex = 0;
                    var nextListIndex = shift
                        ? (currentListIndex - 1 + categories.Count) % categories.Count
                        : (currentListIndex + 1) % categories.Count;
                    SettingsMenu.instance.SelectTab(categories[nextListIndex]);
                }
            }

            yield return null;
        }
    }

    private static List<int> GetAllCategoryIndices()
    {
        var indices = Enum.GetValues(typeof(Setting.SettingCategory)).Cast<int>().ToList();

        try
        {
            var registryType = Type.GetType("CUCoreLib.Registries.ModOptionsRegistry, CUCoreLib");
            if (registryType != null)
            {
                var method = registryType.GetMethod("GetCustomCategories",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    if (method.Invoke(null, null) is IEnumerable result)
                    {
                        foreach (var entry in result)
                        {
                            var prop = entry.GetType().GetProperty("CategoryIndex");
                            if (prop == null) continue;
                            var val = prop.GetValue(entry);
                            if (val is int intVal)
                                indices.Add(intVal);
                        }
                    }
                }
            }
        }
        catch
        {
            // ignored
        }

        return indices.Distinct().OrderBy(i => i).ToList();
    }
}