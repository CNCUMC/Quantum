![Logo](Logo.png)

# Quantum（量子）

[English Guide](README.md)

[GitHub](https://github.com/CNCUMC/Quantum) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/365)

_《[Casualties Unknown](https://store.steampowered.com/app/4576490/)》的便捷功能模组 —
合成搜索、容器整理、枪械修改和玩法调整，全部可在游戏内设置菜单中配置。_

---

## 功能

| 功能                  | 说明                                                |
|---------------------|---------------------------------------------------|
| **Ctrl + Shift 悬停** | 展开物品详细信息（合成配方、可用标志、无视抑郁等）                         |
| **拼音搜索**            | 用拼音首字母（`bd` → 绷带）或全拼搜索合成配方                        |
| **容器整理**            | 按名称 / 价值 / 重量排序（升序 / 降序），带整理按钮                    |
| **总价值显示**           | 在物品栏重量旁显示当前物品总价值                                  |
| **弹药显示**            | 枪械菜单上方实时显示当前/最大弹量（菜单/暂停时自动隐藏）                     |
| **枪械修改**            | 自动上膛、不毁枪械、无限弹药、永不卡壳、无弹壳、无后座力                      |
| **耐久警报**            | 收藏物品耐久每下降 5% 提醒一次                                 |
| **双语物品名**           | 在物品原名后附加指定语言的翻译                                   |
| **控制台增强**           | Tab 自动补全（可滚动候选列表）、上/下切换参数、Ctrl+Z 撤销、历史记录上限        |
| **控制台参数切换**         | 按住上下箭头循环切换自动补全候选（可配置速度，Shift 2倍速，Ctrl+Shift 5倍速）  |
| **模糊点匹配**           | 输入 `xX` 可匹配 `xxx.XXX` 之类用 `.` 分隔的参数               |
| **无观察者**            | 禁用观察者                                             |
| **不排泄**             | 昏迷时不再排泄                                           |
| **Tab / Esc 关闭全部**  | 一键关闭所有打开的 UI 面板（制作、医疗、交易）                         |
| **设置菜单**            | ESC 关闭，Tab / Shift+Tab 切换标签页（包含 CUCoreLib 自定义标签页） |
| **隐藏试玩提示**          | 隐藏试玩版提示面板                                         |
| **调试屏幕（F3）**        | 滑出式面板显示游戏信息、性能分析、世界数据和系统信息。可配置速度。                 |
| **FPS 图表（F3+F）**    | 实时帧时间图表（120 帧，颜色编码，60/30 FPS 参考线）                 |
| **自动沙盒**            | 跳过教程课程选择，直接进入沙盒模式                                 |

## 依赖

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx)
- [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) ≥ 1.0.3
- [Bark](https://github.com/CNCUMC/Bark) ≥ 1.1.1

## 安装

1. 为 Casualties Unknown 安装 BepInEx 5.x。
2. 安装 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) — 将 `CUCoreLib.dll` 放入 `BepInEx/plugins/`。
3. 安装 [Bark](https://github.com/CNCUMC/Bark) — 将 `Bark.dll` 放入 `BepInEx/plugins/Bark/`。
4. 安装 `Quantum`。
5. 启动游戏，设置菜单中出现 **Quantum** 标签页。

## 设置

所有选项位于游戏设置菜单中（由 [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) 驱动）：

- **Quantum** 标签页 — 枪械修改、玩法调整、UI 开关
- **Video** 标签页 — 信息显示、候选数量、试玩提示
- **Input** 标签页 — 整理按键、控制台滚动速度、调试屏幕速度

## 许可

LGPL v3
