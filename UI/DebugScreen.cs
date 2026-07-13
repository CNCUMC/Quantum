using System.Collections.Generic;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx.Bootstrap;
using HarmonyLib;
using Unity.Profiling;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class DebugScreen
{
    private const string LocaleKeyPre = "debug_screen.";
    public static bool Hidden = true;

    private static float _currentX;
    private static float _velocity;
    private const float PanelWidth = 300f;
    private static MonoBehaviour _guiHelper;
    private static readonly List<string> _leftText = [];
    private static readonly List<string> _rightText = [];
    
    private static readonly Assembly _bepInExAssembly = typeof(BepInEx.Paths).Assembly;

    private static void BuildText()
    {
        LeftHead();
        Profiler();
    }
    
    private static void LeftHead()
    {
        AddLeftText($"Casualties Unknown Demo v{Application.version}");
        AddLeftText($"BepInEx v{_bepInExAssembly.GetName().Version}");
        AddLeftTextLocale("loading_mod_list", Chainloader.PluginInfos.Count);
        AddLeftLine();
    }
    
    private static void Profiler()
    {
        AddLeftTextLocale("profiler.memory", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Used Memory").CurrentValue);
        AddLeftLine();
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerCamera __instance)
    {
        if (_guiHelper == null)
        {
            _guiHelper = __instance.gameObject.AddComponent<DebugGuiHelper>();
        }

        var targetX = Hidden ? -PanelWidth : 0f;
        _currentX = Mathf.SmoothDamp(_currentX, targetX, ref _velocity, Plugin.DebugScreenSpeed, Mathf.Infinity,
            Time.unscaledDeltaTime);

        if (!Hidden)
        {
            RefreshText();
        }
    }

    private class DebugGuiHelper : MonoBehaviour
    {
        private void OnGUI()
        {
            if (Hidden && _currentX <= -PanelWidth + 1f) return;

            GUI.skin.font = TextUtil.Unifont;

            var height = Screen.height;

            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.Box(new Rect(_currentX, 0, PanelWidth, height), "");

            GUI.Box(new Rect(Screen.width - PanelWidth - _currentX, 0, PanelWidth, height), "");

            GUI.color = Color.white;
            GUI.Label(new Rect(_currentX + 10f, 10f, PanelWidth - 20f, height - 20f),
                string.Join("\n", _leftText));

            GUI.Label(new Rect(Screen.width - PanelWidth - _currentX + 10f, 10f, PanelWidth - 20f, height - 20f),
                string.Join("\n", _rightText));
        }
    }

    public static void AddLeftText(string text)
    {
        _leftText.Add(text);
    }

    public static void AddRightText(string text)
    {
        _rightText.Add(text);
    }
    
    public static void AddLeftLine()
    {
        AddLeftText("");
    }

    public static void AddRightLine()
    {
        AddRightText("");
    }

    private static void AddLeftTextLocale(string text, params object[] args)
    {
        _leftText.Add(LocaleOther(text, args));
    }
    
    private static void AddRightTextLocale(string text, params object[] args)
    {
        _rightText.Add(LocaleOther(text, args));
    }

    private static void RefreshText()
    {
        _leftText.Clear();
        _rightText.Clear();
        BuildText();
    }
    
    private static string LocaleOther(string key, params object[] args)
    {
        return BetterLocale.GetOther(LocaleKeyPre + key, args);
    }
}