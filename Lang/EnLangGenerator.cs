using MossLib.Base;

namespace Quantum.Lang;

public class EnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "EN";

    protected override void BuildLocaleData()
    {
        Add("hover.info.usable.true", "Can be used directly");
        Add("hover.info.usable.false", "Cannot be used directly");
        Add("hover.info.usable_on_limb.true", "Can be used on limbs");
        Add("hover.info.usable_on_limb.false", "Cannot be used on limbs");
        Add("hover.info.auto_attack", "Continuous use when long press");
        Add("hover.info.usable_with_lrb", "Can only be used with left click");
        Add("hover.info.ignore_depression", "Ignore depression status");
        Add("hover.info.recipe", "Recipes: ");

        Add("key.shift_to_expand.down", "Release Shift to Fold");
        Add("key.ctrl_to_expand.up", "Hold Ctrl to expand more information");
        Add("key.ctrl_to_expand.down", "Release Ctrl to Fold More Info");

        Add("config.ctrl_to_expand.description", "Press Ctrl to show more information");
    }
}