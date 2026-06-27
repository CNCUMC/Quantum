using Bark.Base;

namespace Quantum.Lang;

public class EnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "EN";

    protected override void BuildLocaleData()
    {
        // hover - Info
        QOther("hover.info.usable.true", "Can be used directly");
        QOther("hover.info.usable.false", "Cannot be used directly");
        QOther("hover.info.usable_on_limb.true", "Can be used on limbs");
        QOther("hover.info.usable_on_limb.false", "Cannot be used on limbs");
        QOther("hover.info.auto_attack", "Continuous use when long press");
        QOther("hover.info.usable_with_lrb", "Can only be used with left click");
        QOther("hover.info.ignore_depression", "Ignore depression status");
        QOther("hover.info.recipe", "Recipes: ");

        // key
        QOther("key.shift_to_expand.down", "Release Shift to Fold");
        QOther("key.ctrl_to_expand.up", "Hold Ctrl to expand more information");
        QOther("key.ctrl_to_expand.down", "Release Ctrl to Fold More Info");

        // Config - Info
        Option("quantum.video.ctrl_to_expand", "Ctrl for more information", "Press Ctrl to show more information");
        Option("quantum.video.favourited_item_durability_exhaustion_alert", "Favourite Durability Alert Threshold", "Alert when favourited item durability falls below this ratio (0 = disabled)");
        
        // Config - Item - Gun
        QOption("auto_rack", "Auto Rack", "If true, guns will automatically rack and stay racked when ammo is available");
        QOption("indestructible_gun", "Indestructible Gun", "If true, guns will not be destroyed");
        QOption("infinite_ammunition", "Infinite Ammunition", "♾ INFINITE AMMUNITION ♾");
        QOption("never_jam", "Never Jam", "If true, guns will never jam");
        QOption("no_casing", "No Shell Case", "If true, guns will not eject the cartridge casing");
        QOption("recoilless", "Recoilless", "If true, guns will not have recoil");
        
        // Config - Mechanism
        QOption("dont_shit", "Don't Shit", "If true, you won't shit yourself when unconscious");
        
        // Config - Misc
        QOption("no_observer", "No Observer", "If true, the world will not have observers");
        
        // Config - UI
        Option("quantum.video.ammunition_ui", "Ammunition UI", "Display your ammunition in real time!");
        Option("quantum.video.bilingual_name", "Bilingual Name", "Appends a translation in the specified language (e.g. EN / zh-CN / zh-TW) to item names; leave empty for original only");
        Option("quantum.input.sort_key", "Sort Key", "Press to sort container items");
        Option("quantum.video.max_visible_candidates", "Max Candidates", "Maximum number of candidate lines displayed in console autocomplete");
        Option("quantum.video.max_history_size", "History Size", "Maximum number of executed commands kept in console history");

        // UI - Sort
        QOther("ui.sort.mode.name", "Name");
        QOther("ui.sort.mode", "Value");
        QOther("ui.sort.mode.weight", "Weight");
        QOther("ui.sort.ascending", "⬆ Ascending");
        QOther("ui.sort.descending", "⬇ Descending");
        QOther("ui.sort.mode_tip", "Sort Mode");
        QOther("ui.sort.mode_desc", "Sort by {0}");
        QOther("ui.sort.completed", "Sorted: {0} {1}");
        QOther("ui.sort.no_change", "Already sorted");
        QOther("ui.sort.execute_tip", "Sort");
        QOther("ui.sort.execute_desc", "Sort container items");

        // Log - PlayerCameraPatch - Pin Yin
        QLog("player_camera_patch.pinyin.library.not_found", "TinyPinyin library not loaded - pinyin search disabled");
        QLog("player_camera_patch.pinyin.api_not_found", "TinyPinyin API methods not found - pinyin search disabled");
        QLog("player_camera_patch.pinyin.init_failed", "TinyPinyin init failed: {0} - pinyin search disabled");

        // Log - ItemPatch
        QLog("item_patch.durability_exhaustion_alert", "{0} durability dropped to {1}%");
    }
    
    private void QOther(string key, string value)
    {
        Other($"quantum.{key}", value);
    }

    private void QOption(string key, string value, string description)
    {
        Option($"quantum.quantum.{key}", value, description);
    }
    
    private void QLog(string key, string value)
    {
        Log($"quantum.{key}", value);
    }
}