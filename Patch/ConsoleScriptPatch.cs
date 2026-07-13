using System;
using System.Linq;
using System.Text;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Quantum.Patch;

[HarmonyPatch(typeof(ConsoleScript))]
public static class ConsoleScriptPatch
{
    private static string[] _candidates = [];
    private static int _index;
    private static string _cmdName = "";
    private static int _paramIdx = -1;
    private static string _lastPartial = "";
    private static float _lastUpTime;
    private static float _lastDownTime;
    private static string _previousText = "";
    private static bool _isInAutocomplete;

    private static int MaxHistorySize => Plugin.MaxHistorySize;
    private static int MaxVisible => Plugin.MaxVisibleCandidates;

    public static bool IsAutocomplete()
    {
        return _isInAutocomplete;
    }

    [HarmonyPatch(nameof(ConsoleScript.TryFinishCommandPart))]
    [HarmonyPrefix]
    private static bool BlockTryFinishCommandPart()
    {
        return !_isInAutocomplete;
    }

    [HarmonyPatch("AddCommandToLogAndClearInput")]
    [HarmonyPostfix]
    private static void PostAddCommandToLogAndClearInput(ConsoleScript __instance)
    {
        if (__instance.executedCommands.Count <= MaxHistorySize)
            return;
        __instance.executedCommands.RemoveRange(0, __instance.executedCommands.Count - MaxHistorySize);
    }

    [HarmonyPatch("GoToCommandHistory")]
    [HarmonyPrefix]
    private static bool BlockGoToCommandHistory()
    {
        return !_isInAutocomplete;
    }

    [HarmonyPatch("HandleDescriptionText")]
    [HarmonyPostfix]
    private static void PostHandleDescriptionText(ConsoleScript __instance)
    {
        if (!_isInAutocomplete || __instance.descriptionText == null)
            return;

        var text = __instance.descriptionText.text;
        if (string.IsNullOrEmpty(text))
            return;

        text = text.Replace("<color=yellow>", "").Replace("</color>", "");

        var newlineIdx = text.IndexOf('\n');
        var header = newlineIdx >= 0
            ? text.Substring(0, newlineIdx)
            : text;

        var maxVisible = MaxVisible;
        int windowStart, windowEnd;
        if (_candidates.Length <= maxVisible)
        {
            windowStart = 0;
            windowEnd = _candidates.Length;
        }
        else
        {
            var half = maxVisible / 2;
            windowStart = _index - half;
            windowEnd = windowStart + maxVisible;

            if (windowStart < 0)
            {
                windowStart = 0;
                windowEnd = maxVisible;
            }
            else if (windowEnd > _candidates.Length)
            {
                windowEnd = _candidates.Length;
                windowStart = windowEnd - maxVisible;
            }
        }

        var sb = new StringBuilder();
        sb.Append(header);
        sb.Append('\n');

        for (var i = windowStart; i < windowEnd; i++)
            if (i == _index)
            {
                sb.Append("<b><color=yellow>");
                sb.Append(_candidates[i]);
                sb.Append("</color></b>\n");
            }
            else
            {
                sb.Append(_candidates[i]);
                sb.Append('\n');
            }

        __instance.descriptionText.text = sb.ToString();
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void PostUpdate(ConsoleScript __instance)
    {
        if (!__instance.active || __instance.input == null)
            return;

        var text = __instance.input.text;
        var args = text.Split([' '], StringSplitOptions.None);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !string.IsNullOrEmpty(text))
        {
            __instance.ExecuteCommand(text);
            return;
        }

        switch (args.Length)
        {
            case 1 when !string.IsNullOrEmpty(args[0]):
            {
                var matchedCommands = ConsoleScript.Search(args[0]);
                if (matchedCommands == null || matchedCommands.Count == 0)
                {
                    ClearState();
                    return;
                }

                _isInAutocomplete = true;
                var matchedNames = matchedCommands.Select(c => c.name).ToArray();
                var partial = args[0];

                if (_cmdName != "!" || _lastPartial != partial)
                {
                    _cmdName = "!";
                    _paramIdx = -1;
                    _candidates = matchedNames;
                    _lastPartial = partial;
                    _index = 0;
                }

                break;
            }
            case >= 2 when !string.IsNullOrEmpty(args[0]):
            {
                var cmd = ConsoleScript.SearchExact(args[0]);
                if (cmd?.argAutofill == null)
                {
                    ClearState();
                    return;
                }

                var paramIdx = args.Length - 2;
                if (!cmd.argAutofill.TryGetValue(paramIdx, out var fills))
                {
                    ClearState();
                    return;
                }

                _isInAutocomplete = true;

                var partial = args[args.Length - 1];
                var filteredFills = ConsoleScript.SearchArgumentAutofill(partial, fills);
                var fuzzyMatches = fills
                    .Where(f => FuzzyDotMatch(f, partial))
                    .Except(filteredFills);
                filteredFills = filteredFills.Concat(fuzzyMatches).ToList();

                if (_cmdName != args[0] || _paramIdx != paramIdx)
                {
                    _cmdName = args[0];
                    _paramIdx = paramIdx;
                    _candidates = filteredFills.ToArray();
                    _lastPartial = partial;
                    _index = 0;
                }
                else if (_lastPartial != partial)
                {
                    _candidates = filteredFills.ToArray();
                    _lastPartial = partial;
                    ClampIndex();
                }

                break;
            }
            default:
                ClearState();
                return;
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(KeyCode.Z)
            && !string.IsNullOrEmpty(_previousText))
        {
            __instance.input.text = _previousText;
            __instance.SetCaretToEnd();
            ClearState();
            return;
        }

        if (_candidates.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DoReplace(__instance, args);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            // 加速逻辑：Shift*2，Ctrl+Shift*5，Ctrl无效果
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var speed = ctrl && shift
                ? Plugin.ConsoleParameterSwitchingSpeed * 0.2f  // 5倍速
                : shift
                    ? Plugin.ConsoleParameterSwitchingSpeed * 0.5f  // 2倍速
                    : Plugin.ConsoleParameterSwitchingSpeed;       // 正常

            if (speed <= 0f)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _index = (_index - 1 + _candidates.Length) % _candidates.Length;
                }
            }
            else if (Time.unscaledTime - _lastUpTime > speed)
            {
                _lastUpTime = Time.unscaledTime;
                _index = (_index - 1 + _candidates.Length) % _candidates.Length;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // 加速逻辑：Shift*2，Ctrl+Shift*5，Ctrl无效果
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var speed = ctrl && shift
                ? Plugin.ConsoleParameterSwitchingSpeed * 0.2f  // 5倍速
                : shift
                    ? Plugin.ConsoleParameterSwitchingSpeed * 0.5f  // 2倍速
                    : Plugin.ConsoleParameterSwitchingSpeed;       // 正常

            if (speed <= 0f)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _index = (_index + 1) % _candidates.Length;
                }
            }
            else if (Time.unscaledTime - _lastDownTime > speed)
            {
                _lastDownTime = Time.unscaledTime;
                _index = (_index + 1) % _candidates.Length;
            }
        }
    }

    private static void ClearState()
    {
        _candidates = [];
        _cmdName = "";
        _paramIdx = -1;
        _lastPartial = "";
        _index = 0;
        _lastUpTime = 0f;
        _lastDownTime = 0f;
        _previousText = "";
        _isInAutocomplete = false;
    }

    private static bool FuzzyDotMatch(string candidate, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return true;
        if (SequentialMatch(candidate.Replace(".", ""), pattern))
            return true;
        var initials = string.Concat(candidate.Split('.').Where(s => s.Length > 0).Select(s => s[0]));
        return SequentialMatch(initials, pattern);
    }

    private static bool SequentialMatch(string text, string pattern)
    {
        var ti = 0;
        foreach (var p in pattern)
        {
            ti = text.IndexOf(p.ToString(), ti, StringComparison.OrdinalIgnoreCase);
            if (ti < 0) return false;
            ti++;
        }

        return true;
    }

    private static void ClampIndex()
    {
        if (_candidates.Length == 0)
            _index = 0;
        else if (_index >= _candidates.Length)
            _index = _candidates.Length - 1;
    }

    private static void DoReplace(ConsoleScript instance, string[] args)
    {
        _previousText = instance.input.text;

        var prefix = string.Join(" ", args.Take(args.Length - 1));
        var replacement = _candidates[_index];
        instance.input.text = string.IsNullOrEmpty(prefix)
            ? replacement + " "
            : $"{prefix} {replacement}";
        instance.SetCaretToEnd();

        _lastPartial = replacement;
    }


    [HarmonyPatch(typeof(TMP_InputField), "KeyPressed")]
    [HarmonyPrefix]
    private static bool BlockArrowNavigation(Event evt)
    {
        if (!IsAutocomplete()) return true;
        return evt?.keyCode is not (KeyCode.UpArrow or KeyCode.DownArrow);
    }
}
