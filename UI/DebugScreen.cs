using System.Collections.Generic;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx.Bootstrap;
using HarmonyLib;
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
        World();
        TargetBlock();
    }

    private static void LeftHead()
    {
        AddLeftText($"Casualties Unknown Demo v{Application.version}");
        AddLeftText($"BepInEx v{_bepInExAssembly.GetName().Version}");
        AddLeftTextLocale("loading_mods", Chainloader.PluginInfos.Count);

        AddLeftLine();
    }

    private static void Profiler()
    {
        // 内存（已使用 / 总内存）
        var gcBytes = System.GC.GetTotalMemory(false);
        var gcMb = gcBytes / (1024f * 1024f);
        var totalMb = SystemInfo.systemMemorySize / 1024f;
        AddLeftTextLocale("profiler.memory", gcMb.ToString("F1"), totalMb.ToString("F0"));

        // 帧时间
        var frameMs = Time.unscaledDeltaTime * 1000f;
        AddLeftTextLocale("profiler.frame", frameMs.ToString("F2") + " ms");

        // FPS
        var fps = 1f / Time.unscaledDeltaTime;
        AddLeftTextLocale("profiler.fps", fps.ToString("F0"));

        AddLeftLine();
    }

    private static void World()
    {
        AddLeftTextLocale("world.position",
            PlayerUtil.Body.transform.position.x.ToString("F2"),
            PlayerUtil.Body.transform.position.y.ToString("F2"));
        AddLeftTextLocale("world.looking_position",
            PlayerUtil.Body.overrideLookPos.x.ToString("F2"),
            PlayerUtil.Body.overrideLookPos.y.ToString("F2"));
        AddLeftTextLocale("world.layer", (WorldUtil.World.biomeDepth + 1).ToString());

        AddLeftLine();
    }

    private static void TargetBlock()
    {
        var world = WorldUtil.World;
        if (world == null)
        {
            AddLeftTextLocale("target_block", "N/A");
            return;
        }

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var blockPos = world.WorldToBlockPos(mouseWorldPos);
        var blockId = world.GetBlock(blockPos);
        var blockInfo = world.GetBlockInfo(blockId);

        if (blockInfo != null && !string.IsNullOrEmpty(blockInfo.name))
        {
            AddLeftTextLocale("target_block", blockInfo.name);
        }
        else
        {
            AddLeftTextLocale("target_block", "Air");
        }
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
