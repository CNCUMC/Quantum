using MossLib.Base;

namespace Quantum.Lang;

public class ZhTwLangGenerator : ModLangGenBase
{
    protected override string LanguageCode => "zh-TW";

    protected override void BuildLocaleData()
    {
        Add("hover.info.usable.true", "可直接使用");
        Add("hover.info.usable.false", "不可直接使用");
        Add("hover.info.usable_on_limb.true", "可對肢體使用");
        Add("hover.info.usable_on_limb.false", "不可對肢體使用");
        Add("hover.info.auto_attack", "長按時持續使用");
        Add("hover.info.usable_with_lrb", "只能左鍵使用");
        Add("hover.info.ignore_depression", "無視抑鬱狀態");
        Add("hover.info.recipe", "合成配方: ");

        Add("key.shift_to_expand.down", "松開Shift摺疊");
        Add("key.ctrl_to_expand.up", "按住Ctrl展開更多資訊");
        Add("key.ctrl_to_expand.down", "松開Ctrl摺疊更多資訊");

        Add("config.ctrl_to_expand.description", "按下Ctrl才顯示更多資訊");
    }
}