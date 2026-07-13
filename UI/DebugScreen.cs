using System.Collections.Generic;
using Bark.Tool;
using HarmonyLib;
using UnityEngine;

namespace Quantum.UI;

[HarmonyPatch(typeof(PlayerCamera))]
public static class DebugScreen
{
    public static bool Hidden = true;

    private static float _currentX;
    private static float _velocity;
    private const float PanelWidth = 300f;
    private static MonoBehaviour _guiHelper;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerCamera __instance)
    {
        if (_guiHelper == null)
        {
            _guiHelper = __instance.gameObject.AddComponent<DebugGuiHelper>();
        }

        var targetX = Hidden ? -PanelWidth : 0f;
        _currentX = Mathf.SmoothDamp(_currentX, targetX, ref _velocity, 1f / Plugin.DebugScreenSpeed, Mathf.Infinity, Time.unscaledDeltaTime);
    }

    private class DebugGuiHelper : MonoBehaviour
    {
        private void OnGUI()
        {
            if (Hidden && _currentX <= -PanelWidth + 1f) return;

            var height = Screen.height;

            // 左侧
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.Box(new Rect(_currentX, 0, PanelWidth, height), "");

            // 右侧
            GUI.Box(new Rect(Screen.width - PanelWidth - _currentX, 0, PanelWidth, height), "");

            // 左侧
            GUI.color = Color.green;
            GUI.Label(new Rect(_currentX + 10f, 10f, PanelWidth - 20f, height - 20f), BuildLeftText());

            // 右侧
            GUI.Label(new Rect(Screen.width - PanelWidth - _currentX + 10f, 10f, PanelWidth - 20f, height - 20f), BuildRightText());
        }
    }

    private static string BuildLeftText()
    {
        var body = PlayerUtil.Body;
        if (body == null) return "No Body";

        var info = new List<string>
        {
            "<b>=== Player ===</b>",
            $"Hunger: {body.hunger:F1}%",
            $"Thirst: {body.thirst:F1}%",
            $"Stamina: {body.stamina:F1}%",
            $"Temperature: {body.temperature:F1}C",
            $"Conscious: {body.conscious}",
            "",
            "<b>=== Limbs ===</b>"
        };

        foreach (var limb in body.limbs)
        {
            info.Add($"{limb.name}: {limb.muscleHealth:F0}%");
        }

        return string.Join("\n", info);
    }

    private static string BuildRightText()
    {
        var camera = PlayerCamera.main;
        if (camera == null) return "No Camera";

        var runTime = WorldGeneration.TotalRunTime();
        var timeSpan = System.TimeSpan.FromSeconds(runTime);

        var info = new List<string>
        {
            "<b>=== World ===</b>",
            $"Run Time: {timeSpan:hh\\:mm\\:ss}",
            "",
            "<b>=== Performance ===</b>",
            $"FPS: {(1f / Time.unscaledDeltaTime):F0}",
            $"Frame Time: {(Time.unscaledDeltaTime * 1000f):F1}ms",
            "",
            "<b>=== Position ===</b>",
            $"X: {camera.transform.position.x:F1}",
            $"Y: {camera.transform.position.y:F1}",
            $"Z: {camera.transform.position.z:F1}"
        };

        return string.Join("\n", info);
    }
}
