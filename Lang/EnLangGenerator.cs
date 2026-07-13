using Bark.Base;

namespace Quantum.Lang;

public class EnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "EN";

    protected override void BuildLocaleData()
    {
        // Info
        Option("quantum.video.ctrl_to_expand", "Ctrl for more information", "Press Ctrl to show more information");
        Option("quantum.video.favourited_item_durability_exhaustion_alert", "Favourite Durability Alert Threshold", "Alert when favourited item durability falls below this ratio (0 = disabled)");

        // Config - Item - Gun
        Option("quantum.quantum.auto_rack", "Auto Rack", "If true, guns will automatically rack and stay racked when ammo is available");
        Option("quantum.quantum.indestructible_gun", "Indestructible Gun", "If true, guns will not be destroyed");
        Option("quantum.quantum.infinite_ammunition", "Infinite Ammunition", "♾ INFINITE AMMUNITION ♾");
        Option("quantum.quantum.never_jam", "Never Jam", "If true, guns will never jam");
        Option("quantum.quantum.no_casing", "No Shell Case", "If true, guns will not eject the cartridge casing");
        Option("quantum.quantum.recoilless", "Recoilless", "If true, guns will not have recoil");

        // Mechanism
        Option("quantum.quantum.dont_shit", "Don't Shit", "If true, you won't shit yourself when unconscious");
        Option("quantum.quantum.dont_bite_lightbulb", "Don't Bite Lightbulb", "If true, you won't bite lightbulb");

        // Misc
        Option("quantum.quantum.no_observer", "No Observer", "If true, the world will not have observers");

        // UI
        Option("quantum.video.ammunition_ui", "Ammunition UI", "Display your ammunition in real time!");
        Option("quantum.video.bilingual_name", "Bilingual Name", "Appends a translation in the specified language (e.g. EN / zh-CN / zh-TW) to item names; leave empty for original only");
        Option("quantum.input.console_parameter_switching_speed", "Console Parameter Switching Speed", "The speed of switching candidate parameters by long pressing the up and down keys in the console (ms)");
        Option("quantum.video.debug_screen_speed", "Debug Screen Speed", "Speed of debug screen slide animation (ms)");
        Option("quantum.video.max_visible_candidates", "Max Candidates", "Maximum number of candidate lines displayed in console autocomplete");
        Option("quantum.video.max_history_size", "History Size", "Maximum number of executed commands kept in console history");
        Option("quantum.video.no_demo_tips", "No Demo Tips", "Hides the demo-version tips text");
        Option("quantum.input.sort_key", "Sort Key", "Press to sort container items");

        // Other
        // Sort
        Other("ui.sort.mode.name", "Name");
        Other("ui.sort.mode", "Value");
        Other("ui.sort.mode.weight", "Weight");
        Other("ui.sort.ascending", "⬆ Ascending");
        Other("ui.sort.descending", "⬇ Descending");
        Other("ui.sort.mode_tip", "Sort Mode");
        Other("ui.sort.mode_desc", "Sort by {0}");
        Other("ui.sort.completed", "Sorted: {0} {1}");
        Other("ui.sort.no_change", "Already sorted");
        Other("ui.sort.execute_tip", "Sort");
        Other("ui.sort.execute_desc", "Sort container items");
        
        // Hover Info
        Other("hover.info.usable.true", "Can be used directly");
        Other("hover.info.usable.false", "Cannot be used directly");
        Other("hover.info.usable_on_limb.true", "Can be used on limbs");
        Other("hover.info.usable_on_limb.false", "Cannot be used on limbs");
        Other("hover.info.auto_attack", "Continuous use when long press");
        Other("hover.info.usable_with_lrb", "Can only be used with left click");
        Other("hover.info.ignore_depression", "Ignore depression status");
        Other("hover.info.recipe", "Recipes: ");

        // Key
        Other("key.shift_to_expand.down", "Release Shift to Fold");
        Other("key.ctrl_to_expand.up", "Hold Ctrl to expand more information");
        Other("key.ctrl_to_expand.down", "Release Ctrl to Fold More Info");

        // Log
        // PlayerCameraPatch
        Log("player_camera_patch.pinyin.library.not_found", "TinyPinyin library not loaded - pinyin search disabled");
        Log("player_camera_patch.pinyin.api_not_found", "TinyPinyin API methods not found - pinyin search disabled");
        Log("player_camera_patch.pinyin.init_failed", "TinyPinyin init failed: {0} - pinyin search disabled");

        // FavouritedItemDurabilityExhaustionAlert
        Log("favourited_item_durability_exhaustion_alert.durability_exhaustion_alert", "{0} durability dropped to {1}%");
    }
}