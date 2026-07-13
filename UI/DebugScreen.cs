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
    private static readonly List<string> _leftText = [];
    private static readonly List<string> _rightText = [];

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

    private static void RefreshText()
    {
        _leftText.Clear();
        _rightText.Clear();
        BuildText();
    }

    private static void BuildText()
    {
        for (var i = 0; i < 10; i++)
        {
            AddLeftText("Left测试!");
            AddRightText("Right测试!");
        }
    }
}