using System.Collections.Generic;
using System.Linq;
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

    private static readonly List<DebugInfoGroup> _groups = [];

    private static void BuildText()
    {
        _groups.Clear();

        var versionGroup = new DebugInfoGroup("version", Side.Left);
        versionGroup.Add(new DebugInfo("game_version", $"Casualties Unknown Demo v{Application.version}"));
        versionGroup.Add(new DebugInfo("bepinex_version", $"BepInEx v{_bepInExAssembly.GetName().Version}"));
        versionGroup.Add(new DebugInfo("cucorelib_version", $"CUCoreLib v{CUCoreLib.CUCoreLibPlugin.VERSION}"));
        versionGroup.Add(new DebugInfo("bark_version", $"Bark v{Bark.Plugin.Version}"));
        versionGroup.Add(new DebugInfo("quantum_version", $"Quantum v{Plugin.Version}"));
        if (MultiplayerRunning && Chainloader.PluginInfos.TryGetValue("KrokoshaCasualtiesMP", out var mpInfo))
            versionGroup.Add(new DebugInfo("mp_version", $"KrokoshaCasualtiesMP v{mpInfo.Metadata.Version}"));
        versionGroup.Add(new DebugInfo("mod_count", LocaleOther("loading_mods", Chainloader.PluginInfos.Count)));
        _groups.Add(versionGroup);

        var profilerGroup = new DebugInfoGroup("profiler", Side.Left);
        profilerGroup.Add(new DebugInfo("frame_time",
            LocaleOther("profiler.frame", (Time.unscaledDeltaTime * 1000f).ToString("F2") + " ms")));
        profilerGroup.Add(new DebugInfo("fps",
            LocaleOther("profiler.fps", (1f / Time.unscaledDeltaTime).ToString("F0"))));
        _groups.Add(profilerGroup);

        var worldGroup = new DebugInfoGroup("world", Side.Left);
        worldGroup.Add(new DebugInfo("position", LocaleOther("world.position",
            PlayerUtil.Body.transform.position.x.ToString("F2"),
            PlayerUtil.Body.transform.position.y.ToString("F2"))));
        worldGroup.Add(new DebugInfo("looking_position", LocaleOther("world.looking_position",
            PlayerUtil.Body.overrideLookPos.x.ToString("F2"),
            PlayerUtil.Body.overrideLookPos.y.ToString("F2"))));
        worldGroup.Add(new DebugInfo("target_block", GetTargetBlockText()));
        worldGroup.Add(new DebugInfo("layer", LocaleOther("world.layer", (WorldUtil.World.biomeDepth + 1).ToString())));
        _groups.Add(worldGroup);

        var systemGroup = new DebugInfoGroup("system", Side.Right);
        systemGroup.Add(new DebugInfo("cpu", $"CPU: {SystemInfo.processorType}"));
        systemGroup.Add(new DebugInfo("gpu",
            $"GPU: {SystemInfo.graphicsDeviceName} - {SystemInfo.graphicsDeviceType}"));
        systemGroup.Add(new DebugInfo("sys", $"SYS: {SystemInfo.operatingSystem}"));
        _groups.Add(systemGroup);
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

            GUI.Label(new Rect(_currentX + 10f, 10f, PanelWidth - 20f, height - 20f), BuildPanelText(Side.Left));
            GUI.Label(new Rect(Screen.width - PanelWidth - _currentX + 10f, 10f, PanelWidth - 20f, height - 20f),
                BuildPanelText(Side.Right));
        }

        private static string BuildPanelText(Side side)
        {
            var lines = new List<string>();
            foreach (var group in _groups)
            {
                var matchedInfos = group.Infos.Where(info => (info.InfoSide ?? group.GroupSide) == side).ToList();
                if (matchedInfos.Count == 0) continue;

                lines.AddRange(matchedInfos.Select(info => info.Text));
                lines.Add("");
            }
            if (lines.Count > 0 && lines[lines.Count - 1] == "")
                lines.RemoveAt(lines.Count - 1);
            return string.Join("\n", lines);
        }
    }

    public enum Side
    {
        Left,
        Right
    }

    public class DebugInfoGroup(string id, Side groupSide)
    {
        public string Id { get; } = id;
        public Side GroupSide { get; set; } = groupSide;
        public List<DebugInfo> Infos { get; } = [];

        public void Add(DebugInfo info) => Infos.Add(info);

        public void TurnLeft() => Turn(Side.Left);
        public void TurnRight() => Turn(Side.Right);

        public void Turn(Side side)
        {
            GroupSide = side;
            foreach (var info in Infos)
                info.Turn(side);
        }
    }

    public class DebugInfo(string id, string text, Side? infoSide = null)
    {
        public string Id { get; } = id;
        public string Text { get; } = text;
        public Side? InfoSide { get; set; } = infoSide;

        public void TurnLeft() => Turn(Side.Left);
        public void TurnRight() => Turn(Side.Right);

        public void Turn(Side side) => InfoSide = side;
    }

    private static string LocaleOther(string key, params object[] args) =>
        BetterLocale.GetOther(LocaleKeyPre + key, args);
}
