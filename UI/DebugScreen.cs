using System.Collections.Generic;
using System.Reflection;
using Bark.BetterCCL;
using Bark.Tool;
using BepInEx;
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

    private static readonly bool MultiplayerRunning = Chainloader.PluginInfos.ContainsKey("KrokoshaCasualtiesMP");
    private static readonly Assembly _bepInExAssembly = typeof(Paths).Assembly;

    private static readonly List<DebugInfoGroup> _leftGroups = [];
    private static readonly List<DebugInfoGroup> _rightGroups = [];

    private static void BuildText()
    {
        _leftGroups.Clear();
        _rightGroups.Clear();

        // Left side groups
        var leftGroup = new DebugInfoGroup();
        leftGroup.Add(new DebugInfo("game_version", $"Casualties Unknown Demo v{Application.version}"));
        leftGroup.Add(new DebugInfo("bepinex_version", $"BepInEx v{_bepInExAssembly.GetName().Version}"));
        leftGroup.Add(new DebugInfo("cucorelib_version", $"CUCoreLib v{CUCoreLib.CUCoreLibPlugin.VERSION}"));
        leftGroup.Add(new DebugInfo("bark_version", $"Bark v{Bark.Plugin.Version}"));
        leftGroup.Add(new DebugInfo("quantum_version", $"Quantum v{Plugin.Version}"));
        if (MultiplayerRunning && Chainloader.PluginInfos.TryGetValue("KrokoshaCasualtiesMP", out var info))
            leftGroup.Add(new DebugInfo("multiplayer_version", $"KrokoshaCasualtiesMP v{info.Metadata.Version}"));
        leftGroup.Add(new DebugInfo("mod_count", LocaleOther("loading_mods", Chainloader.PluginInfos.Count)));
        _leftGroups.Add(leftGroup);

        // Profiler group
        var profilerGroup = new DebugInfoGroup();
        profilerGroup.Add(new DebugInfo("frame_time", LocaleOther("profiler.frame", (Time.unscaledDeltaTime * 1000f).ToString("F2") + " ms")));
        profilerGroup.Add(new DebugInfo("fps", LocaleOther("profiler.fps", (1f / Time.unscaledDeltaTime).ToString("F0"))));
        _leftGroups.Add(profilerGroup);

        // World group
        var worldGroup = new DebugInfoGroup();
        worldGroup.Add(new DebugInfo("position", LocaleOther("world.position",
            PlayerUtil.Body.transform.position.x.ToString("F2"),
            PlayerUtil.Body.transform.position.y.ToString("F2"))));
        worldGroup.Add(new DebugInfo("look_pos", LocaleOther("world.looking_position",
            PlayerUtil.Body.overrideLookPos.x.ToString("F2"),
            PlayerUtil.Body.overrideLookPos.y.ToString("F2"))));
        worldGroup.Add(new DebugInfo("target_block", GetTargetBlockText()));
        worldGroup.Add(new DebugInfo("layer", LocaleOther("world.layer", (WorldUtil.World.biomeDepth + 1).ToString())));
        _leftGroups.Add(worldGroup);

        // Right side group
        var rightGroup = new DebugInfoGroup();
        rightGroup.Add(new DebugInfo("cpu", $"CPU: {SystemInfo.processorType}"));
        rightGroup.Add(new DebugInfo("gpu", $"GPU: {SystemInfo.graphicsDeviceName} - {SystemInfo.graphicsDeviceType}"));
        rightGroup.Add(new DebugInfo("sys", $"SYS: {SystemInfo.operatingSystem}"));
        _rightGroups.Add(rightGroup);
    }

    private static string GetTargetBlockText()
    {
        var world = WorldUtil.World;
        var body = PlayerUtil.Body;
        if (world == null || body == null)
            return "";

        var lookPos = body.overrideLookTime >= 0
            ? body.overrideLookPos
            : (Vector2)body.targetLookPos;

        var blockPos = world.WorldToBlockPos(lookPos);
        var blockId = world.GetBlock(blockPos);
        var blockInfo = world.GetBlockInfo(blockId);

        return blockInfo != null && !string.IsNullOrEmpty(blockInfo.name)
            ? LocaleOther("world.target_block", blockInfo.name, blockId)
            : "";
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerCamera __instance)
    {
        if (_guiHelper == null)
            _guiHelper = __instance.gameObject.AddComponent<DebugGuiHelper>();

        var targetX = Hidden ? -PanelWidth : 0f;
        _currentX = Mathf.SmoothDamp(_currentX, targetX, ref _velocity, Plugin.DebugScreenSpeed, Mathf.Infinity,
            Time.unscaledDeltaTime);

        if (!Hidden)
            BuildText();
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

            var leftText = BuildPanelText(_leftGroups);
            var rightText = BuildPanelText(_rightGroups);

            GUI.Label(new Rect(_currentX + 10f, 10f, PanelWidth - 20f, height - 20f), leftText);
            GUI.Label(new Rect(Screen.width - PanelWidth - _currentX + 10f, 10f, PanelWidth - 20f, height - 20f), rightText);
        }

        private static string BuildPanelText(List<DebugInfoGroup> groups)
        {
            var lines = new List<string>();
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                foreach (var info in group.Infos)
                {
                    lines.Add(info.Text);
                }
                if (i < groups.Count - 1)
                {
                    lines.Add("");
                }
            }
            return string.Join("\n", lines);
        }
    }

    public class DebugInfoGroup
    {
        public List<DebugInfo> Infos { get; } = [];

        public void Add(DebugInfo info)
        {
            Infos.Add(info);
        }
    }

    public class DebugInfo(string id, string text)
    {
        public string Id { get; } = id;
        public string Text { get; } = text;
    }
    
    private static string LocaleOther(string key, params object[] args)
    {
        return BetterLocale.GetOther(LocaleKeyPre + key, args);
    }
}
