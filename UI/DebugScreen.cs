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
    public static bool ShowFpsGraph = true;

    private static float _currentX;
    private static float _velocity;
    private const float PanelWidth = 300f;
    private static MonoBehaviour _guiHelper;

    private static readonly bool MultiplayerRunning = Chainloader.PluginInfos.ContainsKey("KrokoshaCasualtiesMP");
    private static readonly Assembly _bepInExAssembly = typeof(Paths).Assembly;

    private static readonly List<DebugInfoGroup> _groups = [];

    private static readonly Queue<float> _frameTimes = new();
    private const int MaxFrameRecords = 120;
    private const float GraphMaxMs = 50f;
    private static Texture2D _graphTexture;

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

        _frameTimes.Enqueue(Time.unscaledDeltaTime * 1000f);
        if (_frameTimes.Count > MaxFrameRecords)
            _frameTimes.Dequeue();

        if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.F))
        {
            ShowFpsGraph = !ShowFpsGraph;
        }

        var targetX = Hidden 
            ? -PanelWidth
            : 0f;
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

            if (ShowFpsGraph)
            {
                DrawFpsGraph();
            }
        }

        private static string BuildPanelText(Side side)
        {
            var lines = new List<string>();
            foreach (var matchedInfos in _groups
                         .Select(group =>
                             group.Infos.Where(info => (info.InfoSide ?? group.GroupSide) == side).ToList())
                         .Where(matchedInfos => matchedInfos.Count != 0))
            {
                lines.AddRange(matchedInfos.Select(info => info.Text));
                lines.Add("");
            }

            if (lines.Count > 0 && lines[lines.Count - 1] == "")
                lines.RemoveAt(lines.Count - 1);
            return string.Join("\n", lines);
        }

        private static void DrawFpsGraph()
        {
            if (!_graphTexture)
            {
                _graphTexture = new Texture2D(MaxFrameRecords, 100)
                {
                    filterMode = FilterMode.Point
                };
            }

            var bgColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            for (var x = 0; x < MaxFrameRecords; x++)
            for (var y = 0; y < 100; y++)
                _graphTexture.SetPixel(x, y, bgColor);

            var y60 = Mathf.Clamp(Mathf.RoundToInt(100f - (16.67f / GraphMaxMs) * 100f), 0, 99);
            var y30 = Mathf.Clamp(Mathf.RoundToInt(100f - (33.33f / GraphMaxMs) * 100f), 0, 99);
            for (var x = 0; x < MaxFrameRecords; x++)
            {
                _graphTexture.SetPixel(x, y60, new Color(0f, 1f, 0f, 0.5f));
                _graphTexture.SetPixel(x, y30, new Color(1f, 1f, 0f, 0.5f));
            }

            var times = _frameTimes.ToArray();
            for (var i = 0; i < times.Length && i < MaxFrameRecords; i++)
            {
                var y = Mathf.Clamp(Mathf.RoundToInt(100f - (times[i] / GraphMaxMs) * 100f), 0, 99);
                var c = times[i] <= 16.67f
                    ? Color.green
                    : times[i] <= 33.33f
                        ? Color.yellow
                        : Color.red;
                _graphTexture.SetPixel(MaxFrameRecords - times.Length + i, y, c);
            }

            _graphTexture.Apply();

            // Draw graph aligned with right panel (full width)
            var graphX = Screen.width - PanelWidth - _currentX;
            var graphRect = new Rect(graphX, Screen.height - 180f, PanelWidth, 150f);
            GUI.DrawTexture(graphRect, _graphTexture);

            GUI.Label(new Rect(graphX + 10f, Screen.height - 200f, PanelWidth - 20f, 20f),
                $"FPS (60={16.67f}ms, 30={33.33f}ms)");
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
