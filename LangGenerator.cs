using System.Collections.Generic;
using Bark.Base;

namespace Quantum;

internal class LangGenerator : ModLangGenMultiBase
{
    protected override string NameSpace => Plugin.NameSpace;

    protected override IEnumerable<string> LanguageCodes =>
    [
        "EN",
        "zh-CN",
        "zh-TW",
        "ru-RU"
    ];

    protected override void BuildLocaleData()
    {
        Option("video.ctrl_to_expand",
            "Ctrl for more information", "Press Ctrl to show more information",
            "Ctrl 更多信息", "按下Ctrl才显示更多信息",
            "Ctrl 更多資訊", "按下Ctrl才顯示更多資訊",
            "Ctrl: подробнее", "Зажмите Ctrl для доп информации");
        Option("video.favourited_item_durability_exhaustion_alert",
            "Favourite Durability Alert Threshold", "Alert when favourited item durability falls below this ratio (0 = disabled)", 
            "收藏品耐久警报阈值", "当收藏的物品耐久度低于此比例时发出警报（0 = 关闭）",
            "收藏品耐久警報閾值", "當收藏的物品耐久度低於此比例時發出警報（0 = 關閉）",
            "Порог износа избранного", "Оповещать при износе ниже порога (0 = отключил)");

        // Config - Item - Gun
        Option("quantum.auto_rack",
            "Auto Rack", "If true, guns will automatically rack and stay racked when ammo is available", 
            "自动上膛", "如果开启，当有弹药时，枪械将自动拉栓并保持拉栓状态",
            "自動上膛", "如果開啟，當有彈藥時，槍械將自動拉栓並保持拉栓狀態",
            "Автодосыл", "Если вкл, оружие само досылает патрон при наличии");
        Option("quantum.indestructible_gun",
            "Indestructible Gun", "If true, guns will not be destroyed",
            "不毁枪械", "如果开启，枪械将不会损坏",
            "不毀槍械", "如果開啟，槍械將不會損壞",
            "Неуничтожимое оружие", "Если вкл, оружие не ломается");
        Option("quantum.infinite_ammunition",
            "Infinite Ammunition", "∞ INFINITE AMMUNITION ∞",
            "无限弹药", "∞ 无限子弹 ∞",
            "無限彈藥", "∞ 無限子彈 ∞",
            "♾ Беск патроны", "♾ Бесконечные патроны ♾");
        Option("quantum.never_jam",
            "Never Jam", "If true, guns will never jam", 
            "永不卡壳", "如果开启，枪械将不会卡壳",
            "永不卡殼", "如果開啟，槍械將不會卡殼",
            "Без заклиниваний", "Если вкл, оружие не клинит");
        Option("quantum.no_casing",
            "No Shell Case", "If true, guns will not eject the cartridge casing",
            "无弹壳", "如果开启，枪械将不会弹出弹壳",
            "無彈殼", "如果開啟，槍械將不會彈出彈殼",
            "Без гильз", "Если вкл, гильзы не вылетают");
        Option("quantum.recoilless",
            "Recoilless", "If true, guns will not have recoil", 
            "无后坐力", "如果开启，枪械将没有后坐力",
            "無後坐力", "如果開啟，槍械將沒有後坐力",
            "Без отдачи", "Если вкл, отдача отсутствует");

        // Mechanism
        Option("quantum.dont_shit",
            "Don't Shit", "If true, you won't shit yourself when unconscious",
            "不排泄", "如果开启，昏迷时不再排泄",
            "不排泄", "如果開啟，昏迷時不再排泄",
            "Не какать", "Если вкл, в отключке не испражняться");
        Option("quantum.dont_bite_lightbulb",
            "Don't Bite Lightbulb", "If true, you won't bite lightbulb", 
            "不咬灯泡", "如果开启，不会再咬灯泡",
            "不咬燈泡", "如果開啟，不會再咬燈泡",
            "Не грызть лампочки", "Если вкл, запрет на укус лампочки");

        // Misc
        Option("quantum.no_observer",
            "No Observer", "If true, the world will not have observers", 
            "无观察者", "如果开启，再无观察者",
            "無觀察者", "如果開啟，再無觀察者",
            "Нет наблюдателей", "Если вкл, наблюдатели не появляются");

        // UI
        Option("video.ammunition_ui",
            "Ammunition UI", "Display your ammunition in real time!",
            "弹药UI", "在原枪械菜单的上方显示枪械剩余弹量和最大弹量",
            "彈藥UI", "在原槍械菜單的上方顯示槍械剩餘彈量和最大彈量",
            "Безапаса UI", "Показывать остаток патронов над меню оружия");
        Option("video.bilingual_name",
            "Bilingual Name", "Appends a translation in the specified language (e.g. EN / zh-CN / zh-TW) to item names; leave empty for original only", 
            "双语名称", "设定后会在物品原名旁附加指定语言的翻译（如 EN / zh-CN / zh-TW），留空则只显示原名",
            "雙語名稱", "設定後會在物品原名旁附加指定語言的翻譯（如 EN / zh-CN / zh-TW），留空則只顯示原名",
            "Двуязычные имена", "Добавить перевод (EN / zh-CN / zh-TW / ru-RU) к названию, пусто: только оригинал");
        Option("input.console_parameter_switching_speed",
            "Console Parameter Switching Speed", "The speed of switching candidate parameters by long pressing the up and down keys in the console (ms)", 
            "控制台参数切换速度", "控制台中长按上下键切换候选参数的速度（毫秒）",
            "控制台參數切換速度", "控制台中長按上下鍵切換候選參數的速度（毫秒）",
            "Скорость перебора параметров", "Задержка при длит. нажатии ↑/↓ в консоли (миллисекунд)");
        Option("input.debug_screen",
            "Debug Screen", "Display debug info",
            "调试屏幕", "显示调试信息",
            "調試屏幕", "顯示調試信息",
            "Экран отладки", "Показывать отладочную информацию");
        Option("input.debug_screen_fps_graph",
            "Debug Screen FPS Graph", "Display FPS graph in Debug Screen", 
            "调试屏幕 FPS 图表", "在调试屏幕中显示帧生成时间图表",
            "調試屏幕 FPS 圖表", "在調試屏幕中顯示幀生成時間圖表",
            "График FPS на экран отладки", "График времени кадра в отладке");
        Option("input.hidden_hud",
            "Hidden HUD", "Hidden all HUD", 
            "隐藏 HUD", "隐藏所有 HUD",
            "隱藏 HUD", "隱藏所有 HUD",
            "Скрыть HUD", "Полное скрытие HUD");
        Option("video.debug_screen_speed",
            "Debug Screen Speed", "Speed of debug screen slide animation (ms)",
            "调试屏幕滑动速度", "调试屏幕滑动的速度（毫秒）",
            "調試屏幕滑動速度", "調試屏幕滑動的速度（毫秒）",
            "Скорость анимации отладки", "Скорость пролистывания отладки (миллисекунд)");
        Option("video.max_visible_candidates",
            "Max Candidates", "Maximum number of candidate lines displayed in console autocomplete",
            "最大候选数", "控制台参数候选列表最多显示的行数",
            "最大候選數", "控制台參數候選列表最多顯示的行數",
            "Макс кандидатов", "Макс строк автодополнения в консоли");
        Option("video.max_history_size",
            "History Size", "Maximum number of executed commands kept in console history", 
            "历史记录上限", "控制台历史命令最多保留的条数",
            "歷史記錄上限", "控制台歷史命令最多保留的條數",
            "Размер истории", "Макс команд в истории консоли");
        Option("video.no_demo_tips",
            "No Demo Tips", "Hides the demo-version tips text", 
            "隐藏试玩提示", "隐藏试玩版提示文字",
            "隱藏試玩提示", "隱藏試玩版提示文字",
            "Скрыть подсказки демо", "Скрыть текст подсказок демо-версии");
        Option("input.sort_key",
            "Sort Key", "Press to sort container items", 
            "整理按键", "按下整理容器物品",
            "整理按鍵", "按下整理容器物品",
            "Клавиша сортировки", "Нажмите для сортировки контейнера");

        // Other
        // Sort
        Other("ui.sort.mode.name",
            "Name",
            "名称",
            "名稱",
            "Имя");
        Other("ui.sort.mode",
            "Value",
            "价值",
            "價值",
            "Стоит");
        Other("ui.sort.mode.weight",
            "Weight",
            "重量",
            "重量",
            "Весит");
        Other("ui.sort.ascending",
            "⬆ Ascending", 
            "↑ 升序",
            "↑ 升序",
            "↑ По возр");
        Other("ui.sort.descending",
            "⬇ Descending", 
            "↓ 降序",
            "↓ 降序",
            "↓ По убыв");
        Other("ui.sort.mode_tip",
            "Sort Mode",
            "排序方式",
            "排序方式",
            "Сортировка");
        Other("ui.sort.mode_desc",
            "Sort by {0}", 
            "按{0}排序",
            "按{0}排序",
            "Сортировать по {0}");
        Other("ui.sort.completed",
            "Sorted: {0} {1}",
            "已整理: {0} {1}",
            "已整理: {0} {1}",
            "Отсортировано: {0} {1}");
        Other("ui.sort.no_change",
            "Already sorted", 
            "无需整理",
            "無需整理",
            "Уже отсортировано");
        Other("ui.sort.execute_tip",
            "Sort", 
            "整理",
            "整理",
            "Сортировать");
        Other("ui.sort.execute_desc",
            "Sort container items", 
            "整理容器物品",
            "整理容器物品",
            "Сортировка предметов в контейнере");
        
        // Hover Info
        Other("hover.info.usable.true",
            "Can be used directly", 
            "可直接使用",
            "可直接使用",
            "Можно использовать напрямую");
        Other("hover.info.usable.false",
            "Cannot be used directly", 
            "不可直接使用",
            "不可直接使用",
            "Нельзя использовать напрямую");
        Other("hover.info.usable_on_limb.true",
            "Can be used on limbs", 
            "可对肢体使用",
            "可對肢體使用",
            "Можно применить к конечности");
        Other("hover.info.usable_on_limb.false",
            "Cannot be used on limbs",
            "不可对肢体使用",
            "不可對肢體使用",
            "Нельзя применить к конечности");
        Other("hover.info.auto_attack",
            "Continuous use when long press", 
            "长按时持续使用",
            "長按時持續使用",
            "Долгое нажатие: постоянное использование");
        Other("hover.info.usable_with_lrb",
            "Can only be used with left click",
            "只能左键使用",
            "只能左鍵使用",
            "Только LMB");
        Other("hover.info.ignore_depression",
            "Ignore depression status", 
            "无视抑郁状态",
            "無視抑鬱狀態",
            "Игнорирует депрессию");
        Other("hover.info.recipe",
            "Recipes: ", 
            "合成配方: ",
            "合成配方: ",
            "Рецепты: ");

        // Key
        Other("key.shift_to_expand.down",
            "Release Shift to Fold", 
            "松开Shift折叠",
            "鬆開Shift折疊",
            "Отпустите Shift для сворачивания");
        Other("key.ctrl_to_expand.up",
            "Hold Ctrl to expand more information", 
            "按住Ctrl展开更多信息",
            "按住Ctrl展開更多信息",
            "Зажмите Ctrl для доп. информации");
        Other("key.ctrl_to_expand.down",
            "Release Ctrl to Fold More Info", 
            "松开Ctrl折叠更多信息",
            "鬆開Ctrl折疊更多信息",
            "Отпустите Ctrl для сворачивания");
                
        // DebugScreen
        Other("debug_screen.loading_mods",
            "Loaded Mods: {0}", 
            "加载模组: {0}",
            "載入模組: {0}",
            "Загружено модов: {0}");
        Other("debug_screen.profiler.render",
            "Render Time: {0}",
            "渲染时间: {0}",
            "渲染時間: {0}",
            "Время рендера: {0}");
        Other("debug_screen.profiler.frame",
            "Frame Time: {0}", 
            "帧时间: {0}",
            "幀時間: {0}",
            "Время кадра: {0}");
        Other("debug_screen.profiler.fps",
            "FPS: {0}", 
            "FPS: {0}",
            "FPS: {0}",
            "FPS: {0}");
        Other("debug_screen.world.position",
            "Position: ({0}, {1})",
            "坐标: ({0}, {1})",
            "坐標: ({0}, {1})",
            "Позиция: ({0}, {1})");
        Other("debug_screen.world.looking_position",
            "Look Pos: ({0}, {1})", 
            "看着的坐标: ({0}, {1})",
            "看著的坐標: ({0}, {1})",
            "Курсор: ({0}, {1})");
        Other("debug_screen.world.layer",
            "Layer: {0}", 
            "当前层级: {0}",
            "當前層級: {0}",
            "Слой: {0}");
        Other("debug_screen.world.target_block",
            "Target Block: {0} ({1})", 
            "目标物块: {0} ({1})",
            "目標物塊: {0} ({1})",
            "Целевой блок: {0} ({1})");

        // Log
        // PlayerCameraPatch
        Log("player_camera_patch.pinyin.library.not_found",
            "TinyPinyin library not loaded - pinyin search disabled", 
            "TinyPinyin 库未加载 — 拼音搜索已禁用",
            "TinyPinyin 庫未載入 — 拼音搜索已禁用",
            "Библиотека TinyPinyin не загружена, поиск по пиньинь отключён");
        Log("player_camera_patch.pinyin.api_not_found",
            "TinyPinyin API methods not found - pinyin search disabled",
            "TinyPinyin API 方法未找到 — 拼音搜索已禁用",
            "TinyPinyin API 方法未找到 — 拼音搜索已禁用",
            "Методы API TinyPinyin не найдены , поиск по пиньинь отключён");
        Log("player_camera_patch.pinyin.init_failed",
            "TinyPinyin init failed: {0} - pinyin search disabled",
            "TinyPinyin 初始化失败: {0} — 拼音搜索已禁用",
            "TinyPinyin 初始化失敗: {0} — 拼音搜索已禁用",
            "Ошибка инициализации TinyPinyin: {0} , поиск по пиньинь отключён");

        // FavouritedItemDurabilityExhaustionAlert
        Log("favourited_item_durability_exhaustion_alert.durability_exhaustion_alert",
            "{0} durability dropped to {1}%",
            "{0} 的耐久度已降至 {1}%",
            "{0} 的耐久度已降至 {1}%",
            "Прочность {0} упала до {1}%");
    }
}
