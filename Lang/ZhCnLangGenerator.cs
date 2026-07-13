using Bark.Base;

namespace Quantum.Lang;

public class ZhCnLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "zh-CN";

    protected override void BuildLocaleData()
    {
        // Info
        Option("quantum.video.ctrl_to_expand", "Ctrl 更多信息", "按下Ctrl才显示更多信息");
        Option("quantum.video.favourited_item_durability_exhaustion_alert", "收藏品耐久警报阈值", "当收藏的物品耐久度低于此比例时发出警报（0 = 关闭）");

        // Item - Gun
        Option("quantum.quantum.auto_rack", "自动上膛", "如果开启，当有弹药时，枪械将自动拉栓并保持拉栓状态");
        Option("quantum.quantum.indestructible_gun", "不毁枪械", "如果开启，枪械将不会损坏");
        Option("quantum.quantum.infinite_ammunition", "无限弹药", "∞ 无限子弹 ∞");
        Option("quantum.quantum.never_jam", "永不卡壳", "如果开启，枪械将不会卡壳");
        Option("quantum.quantum.no_casing", "无弹壳", "如果开启，枪械将不会弹出弹壳");
        Option("quantum.quantum.recoilless", "无后座力", "如果开启，枪械将没有后坐力");

        // Mechanism
        Option("quantum.quantum.dont_shit", "不排泄", "如果开启，昏迷时不再排泄");
        Option("quantum.quantum.dont_bite_lightbulb", "不咬灯泡", "如果开启，不会再咬灯泡");

        // Misc
        Option("quantum.quantum.no_observer", "无观察者", "如果开启，再无观察者");

        // UI
        Option("quantum.video.ammunition_ui", "弹药UI", "在原枪械菜单的上方显示枪械剩余弹量和最大弹量");
        Option("quantum.video.bilingual_name", "双语名称", "设定后会在物品原名旁附加指定语言的翻译（如 EN / zh-CN / zh-TW），留空则只显示原名");
        Option("quantum.input.console_parameter_switching_speed", "控制台参数切换速度", "控制台中长按上下键切换候选参数的速度（毫秒）");
        Option("quantum.video.debug_screen_speed", "调试屏幕滑动速度", "调试屏幕滑动的速度（毫秒）");
        Option("quantum.input.sort_key", "整理按键", "按下整理容器物品");
        Option("quantum.video.max_visible_candidates", "最大候选数", "控制台参数候选列表最多显示的行数");
        Option("quantum.video.max_history_size", "历史记录上限", "控制台历史命令最多保留的条数");
        Option("quantum.video.no_demo_tips", "隐藏试玩提示", "隐藏试玩版提示文字");
        
        // Other
        // Sort
        Other("ui.sort.mode.name", "名称");
        Other("ui.sort.mode", "价值");
        Other("ui.sort.mode.weight", "重量");
        Other("ui.sort.ascending", "↑ 升序");
        Other("ui.sort.descending", "↓ 降序");
        Other("ui.sort.mode_tip", "排序方式");
        Other("ui.sort.mode_desc", "按{0}排序");
        Other("ui.sort.completed", "已整理: {0} {1}");
        Other("ui.sort.no_change", "无需整理");
        Other("ui.sort.execute_tip", "整理");
        Other("ui.sort.execute_desc", "整理容器物品");
        
        // Hover Info
        Other("hover.info.usable.true", "可直接使用");
        Other("hover.info.usable.false", "不可直接使用");
        Other("hover.info.usable_on_limb.true", "可对肢体使用");
        Other("hover.info.usable_on_limb.false", "不可对肢体使用");
        Other("hover.info.auto_attack", "长按时持续使用");
        Other("hover.info.usable_with_lrb", "只能左键使用");
        Other("hover.info.ignore_depression", "无视抑郁状态");
        Other("hover.info.recipe", "合成配方: ");

        // Key
        Other("key.shift_to_expand.down", "松开Shift折叠");
        Other("key.ctrl_to_expand.up", "按住Ctrl展开更多信息");
        Other("key.ctrl_to_expand.down", "松开Ctrl折叠更多信息");
                
        // DebugScreen
        Other("debug_screen.loading_mod_list", "加载模组列表: {0}");
        Other("debug_screen.profiler.memory", "已使用内存: {0} / {1} MiB");
        Other("debug_screen.profiler.render", "渲染时间: {0}");
        Other("debug_screen.profiler.frame", "帧时间: {0}");
        Other("debug_screen.profiler.fps", "FPS: {0}");

        // Log
        // PlayerCameraPatch
        Log("player_camera_patch.pinyin.library.not_found", "TinyPinyin 库未加载 — 拼音搜索已禁用");
        Log("player_camera_patch.pinyin.api_not_found", "TinyPinyin API 方法未找到 — 拼音搜索已禁用");
        Log("player_camera_patch.pinyin.init_failed", "TinyPinyin 初始化失败: {0} — 拼音搜索已禁用");

        // FavouritedItemDurabilityExhaustionAlert
        Log("favourited_item_durability_exhaustion_alert.durability_exhaustion_alert", "{0} 的耐久度已降至 {1}%");
    }
}