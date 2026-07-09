# 更新日志

本文件记录本项目所有重要变更。

格式基于 [Keep a Changelog](https://keepachangelog.com/)，本项目遵循 [语义化版本控制](https://semver.org/)。

---

## v1.1.0

### 新增

- **拼音搜索** — 通过 TinyPinyin 用拼音首字母（`bd` → 绷带）或全拼搜索合成配方
- **容器整理按钮** — 按名称 / 价值 / 重量排序（升序 / 降序），带游戏内切换按钮
- **总价值显示** — 在物品栏重量旁显示当前物品总价值
- **弹药显示** — 枪械菜单上方实时显示当前/最大弹量（菜单/暂停时自动隐藏）
- **枪械修改** — 自动上膛、不毁枪械、无限弹药、永不卡壳、无弹壳、无后座力
- **收藏品耐久警报** — 收藏物品耐久每下降 5% 提醒一次
- **双语物品名** — 通过设置下拉菜单在物品原名后附加指定语言的翻译
- **控制台自动补全** — Tab 自动补全（可滚动候选列表）、上/下切换参数、Ctrl+Z 撤销、可配置历史上限
- **模糊点匹配** — 输入 `xX` 可匹配 `xxx.XXX` 之类用 `.` 分隔的控制台参数
- **候选滚动速度设置** — 可配置的上/下切换候选参数的节流速度（Input 标签页）
- **无观察者** — 禁用观察者
- **不排泄** — 昏迷时不再排泄
- **Tab / Esc 关闭全部** — 一键关闭所有打开的 UI 面板（制作、医疗、交易）
- **隐藏试玩提示** — 隐藏试玩版提示面板
- **Bark 本地化** — 通过 `Lang/EnLangGenerator` 和 `Lang/ZhCnLangGenerator` 注册 EN/zh-CN 回退值
- **CCL 设置系统** — 所有选项通过 `ModOptionsRegistry` 注册到游戏内 **Quantum** 设置标签页
- **TMP 方向键修复** — 补丁 `TMP_InputField.KeyPressed` 阻止自动补全时 Up/Down 光标闪烁

### 变更

- **配置系统** — 从 BepInEx `ConfigEntry<T>` 迁移至 CUCoreLib `ModOptionsRegistry` + `BetterOptions`
- **本地化** — 从 MossLib 生成器迁移至 Bark `BetterLocale` + CCL 本地化文件
- **项目结构** — `Patch/` 内容按功能拆分至 `Info/`、`ItemChange/`、`Misc/`、`Mechanism/`、`UI/`
- **本地化键格式** — 统一为 `log.{class_name}.{specific_key}` 模式
- **耐久检查** — 节流间隔改为帧 + 2 秒两级检测
- **依赖** — 新增 Bark（`org.cncumc.bark` ≥ 1.0.2）
- **依赖** — CUCoreLib ≥ 1.0.2
