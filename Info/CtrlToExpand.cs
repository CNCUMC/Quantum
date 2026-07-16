using System;
using System.Collections.Generic;
using System.Linq;
using Bark.BetterCCL;
using Bark.Tool;
using HarmonyLib;
using UnityEngine;

namespace Quantum.Info;

[HarmonyPatch(typeof(PlayerCamera))]
public class CtrlToExpand
{
    private const string LocaleKeyPre = "ctrl_to_expand";
    private static readonly Dictionary<string, List<Recipe>> ProductToRecipes = new();

    [HarmonyPatch("ItemHoverDescription")]
    [HarmonyPostfix]
    public static void Postfix(Item item, ref ValueTuple<string, string> __result)
    {
        if (item == null || item.Stats?.rec is not { recognizable: true })
            return;

        // Shift 没按住时原版显示"按住Shift展开"，不干涉
        if (!Input.GetKey(KeyBinds.GetBind("expanddesc")))
            return;

        var description = __result.Item2;
        var extraInfo = BuildTechnicalInfo(item);
        if (string.IsNullOrEmpty(extraInfo)) return;

        // Shift 按住时原版"按住Shift展开"消失，加上"松开Shift"替代
        var hint =
            $"<color=#a2e8af><sprite index=2 tint=1><i>{LocaleOther("key.shift_to_expand.down")}</i></color>\n";
        extraInfo = hint + extraInfo;

        if (string.IsNullOrEmpty(description))
            __result.Item2 = extraInfo;
        else if (description.IndexOf(extraInfo, StringComparison.OrdinalIgnoreCase) < 0)
            __result.Item2 = $"{description.TrimEnd()}\n{extraInfo}";
    }

    private static string BuildTechnicalInfo(Item item)
    {
        var info = item.Stats;
        if (info == null)
            return null;

        var needCtrl = Plugin.CtrlToExpand;
        var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        var result = "";

        // 物品专属描述
        if (BetterLocale.HasKeyOther($"hover.{item.id}"))
        {
            result += "\n";
            result += LocaleOther($"hover.{item.id}");
            result += "\n\n";
        }

        // Ctrl 提示行 + 配方（二级展开）
        var recipeInfo = BuildRecipeString(item.id);
        var showRecipe = !needCtrl || ctrlHeld;

        var ctrlHint = ctrlHeld
            ? LocaleOther("key.ctrl_to_expand.down")
            : LocaleOther("key.ctrl_to_expand.up");
        if (needCtrl) result += $"<color=#a2e8af><sprite index=2 tint=1><i>{ctrlHint}</i></color>\n";

        if (showRecipe && !string.IsNullOrEmpty(recipeInfo))
            result += recipeInfo + "\n\n";

        // 技术标志（始终显示）
        result += info.usable
            ? TextUtil.Green("? " + LocaleOther("hover.info.usable.true"))
            : TextUtil.Red("X  " + LocaleOther("hover.info.usable.false"));
        result += "\n";

        result += info.usableOnLimb
            ? TextUtil.Green("? " + LocaleOther("hover.info.usable_on_limb.true"))
            : TextUtil.Red("X  " + LocaleOther("hover.info.usable_on_limb.false"));
        result += "\n";

        result += info.autoAttack
            ? LocaleOther("hover.info.auto_attack") + "\n"
            : null;

        result += info.usableWithLMB
            ? LocaleOther("hover.info.usable_with_lrb") + "\n"
            : null;

        result += info.ignoreDepression
            ? TextUtil.Color(LocaleOther("hover.info.ignore_depression"), "#FFFB91") + "\n"
            : null;

        return string.IsNullOrEmpty(result.Trim())
            ? null
            : result.TrimEnd('\n');
    }

    private static string BuildRecipeString(string itemId)
    {
        var recipes = GetRecipesByProduct(itemId);
        if (recipes == null || recipes.Count == 0)
            return null;

        var recipeBlocks = new List<string>();

        foreach (var recipe in recipes)
        {
            if (recipe?.items == null || recipe.items.Count == 0)
                continue;

            // 合并相同材料
            var grouped = recipe.items
                .Where(ri => ri != null)
                .GroupBy(ri => new
                {
                    ri.specific,
                    ri.specificId,
                    ri.isLiquid,
                    qualityId = ri.quality?.id,
                    qualityAmount = Math.Round(ri.quality?.amount ?? 0f, 4),
                    ri.minimumCondition,
                    ri.destroyItem
                })
                .Select(g => new { Item = g.First(), Count = g.Count() })
                .ToList();

            var blockLines = new List<string>();

            foreach (var g in grouped)
            {
                var ri = g.Item;
                var count = g.Count;

                string nameLine;
                if (!ri.specific)
                {
                    if (ri.isLiquid)
                        nameLine = Locale.GetOther("craftanyliquid");
                    else if (ri.quality is { id: "hammering" or "cutting" })
                        nameLine = Locale.GetOther("craftanytool");
                    else
                        nameLine = Locale.GetOther("craftanyitem");
                }
                else
                {
                    nameLine = ri.isLiquid
                        ? Locale.GetOther(ri.specificId)
                        : Locale.GetItem(ri.specificId);
                }

                if (count > 1)
                    nameLine += $" x{count}";

                var constraints = new List<string>();

                if (ri.isLiquid)
                {
                    switch (ri.specific)
                    {
                        case false when ri.quality != null:
                        {
                            var q = Locale.GetOther("craftliquidquality")
                                .Replace("<1>", ri.quality.amount.ToString("0.#"))
                                .Replace("<2>", ri.quality.LocaleName);
                            constraints.Add(q);
                            break;
                        }
                        case true when ri.minimumCondition > 0f:
                        {
                            var m = Locale.GetOther("craftml")
                                .Replace("<>", ri.minimumCondition.ToString("0.#"));
                            constraints.Add(m);
                            break;
                        }
                    }
                }
                else
                {
                    if (!ri.specific && ri.quality != null)
                    {
                        var q = Locale.GetOther("craftitemquality")
                            .Replace("<1>", ri.quality.amount.ToString("0.#"))
                            .Replace("<2>", ri.quality.LocaleName);
                        constraints.Add(q);

                        if (Recipes.QualityExamples != null)
                        {
                            var example = Recipes.QualityExamples
                                .FirstOrDefault(kvp =>
                                    kvp.Key.id == ri.quality.id &&
                                    Math.Abs(kvp.Key.amount - ri.quality.amount) < 0.001f);
                            if (example.Value != null)
                            {
                                var ex = Locale.GetOther("craftexample")
                                    .Replace("<>", Locale.GetItem(example.Value));
                                constraints.Add(ex);
                            }
                        }
                    }

                    if (ri.minimumCondition > 0f)
                    {
                        var c = Locale.GetOther("craftcondition")
                            .Replace("<>",
                                PlayerCamera.ConditionToColorCode(ri.minimumCondition) +
                                (ri.minimumCondition * 100f).ToString("0.#") +
                                "</color>"
                            );
                        constraints.Add(c);
                    }
                }

                if (constraints.Count > 0)
                    nameLine += " " + string.Join(" ", constraints);

                blockLines.Add($"  - {nameLine}");
            }

            if (blockLines.Count <= 0) continue;

            recipeBlocks.Add(string.Join("\n", blockLines));
        }

        return recipeBlocks.Count > 0
            ? TextUtil.White("\n" +
                             LocaleOther("hover.info.recipe") +
                             "\n" +
                             string.Join("\n", recipeBlocks))
            : null;
    }

    private static void EnsureRecipeLookup()
    {
        if (ProductToRecipes.Count > 0)
            return;
        if (Recipes.recipes == null || Recipes.recipes.Count == 0)
            return;
        foreach (var recipe in Recipes.recipes)
        {
            if (recipe?.result == null || string.IsNullOrEmpty(recipe.result.id))
                continue;
            var pid = recipe.result.id;
            if (!ProductToRecipes.ContainsKey(pid))
                ProductToRecipes[pid] = [];
            ProductToRecipes[pid].Add(recipe);
        }
    }

    private static List<Recipe> GetRecipesByProduct(string productId)
    {
        EnsureRecipeLookup();
        ProductToRecipes.TryGetValue(productId, out var list);
        return list ?? [];
    }

    private static string LocaleOther(string key, params object[] args)
    {
        return BetterLocale.GetOther($"{Plugin.NameSpace}.{LocaleKeyPre}.{key}", args);
    }
}