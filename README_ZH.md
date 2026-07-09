![Logo](Logo.png)

# Quantum（量子）

[English Guide](README.md)

[GitHub](https://github.com/CNCUMC/Quantum) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/365)

_《[Casualties Unknown](https://store.steampowered.com/app/3624440/Casualties_Unknown_Demo/)》的便捷功能模组 —
合成搜索、容器整理、枪械修改和玩法调整，全部可在游戏内 **Quantum** 设置标签页中配置。_

---

## 功能

| 功能                  | 说明                                         |
|---------------------|--------------------------------------------|
| **Ctrl + Shift 悬停** | 展开物品详细信息（合成配方、可用标志、无视抑郁等）                  |
| **拼音搜索**            | 用拼音首字母（`bd` → 绷带）或全拼搜索合成配方                 |
| **容器整理**            | 按名称 / 价值 / 重量排序（升序 / 降序），带整理按钮             |
| **总价值显示**           | 在物品栏重量旁显示当前物品总价值                           |
| **弹药显示**            | 枪械菜单上方实时显示当前/最大弹量（菜单/暂停时自动隐藏）              |
| **枪械修改**            | 自动上膛、不毁枪械、无限弹药、永不卡壳、无弹壳、无后座力               |
| **耐久警报**            | 收藏物品耐久每下降 5% 提醒一次                          |
| **双语物品名**           | 在物品原名后附加指定语言的翻译                            |
| **控制台增强**           | Tab 自动补全（可滚动候选列表）、上/下切换参数、Ctrl+Z 撤销、历史记录上限 |
| **模糊点匹配**           | 输入 `xX` 可匹配 `xxx.XXX` 之类用 `.` 分隔的参数        |
| **无观察者**            | 禁用观察者                                      |
| **不排泄**             | 昏迷时不再排泄                                    |
| **Tab / Esc 关闭全部**  | 一键关闭所有打开的 UI 面板（制作、医疗、交易）                  |
| **隐藏试玩提示**          | 隐藏试玩版提示面板                                  |

## 依赖

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx)
- [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) ≥ 1.0.2
- [Bark](https://github.com/CNCUMC/Bark) ≥ 1.0.2

## 安装

1. 为 Casualties Unknown 安装 BepInEx 5.x。
2. 安装 [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) — 将 `CUCoreLib.dll` 放入 `BepInEx/plugins/`。
3. 安装 [Bark](https://github.com/CNCUMC/Bark) — 将 `Bark.dll` 放入 `BepInEx/plugins/`。
4. 从 [Releases](https://github.com/CNCUMC/Quantum/releases) 下载最新 `Quantum-v*.zip`。
5. 解压到游戏目录。`Quantum.dll` 和 `TinyPinyin.dll` 放入 `BepInEx/plugins/Quantum/`。
6. 启动游戏，设置菜单中出现 **Quantum** 标签页。

## 设置

所有选项位于游戏设置菜单的 **Quantum** 标签页（由 [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) 驱动），修改即时生效。

## 本地化

内置简体中文和英文。添加更多语言：

1. 在游戏内控制台运行 `createLocale` 生成 `EN.json`（位于 `BepInEx/config/CUCoreLib/Locales/`）。
2. 复制 `EN.json` → `{语言代码}.json`，翻译其中的值，重启游戏即可。

## 项目结构

```
Quantum/
├── Plugin.cs                      # 入口 + CCL 设置注册
├── Info/                          # Ctrl 展开、耐久警报
├── ItemChange/                    # 物品描述 + 双语名称
│   └── Gun/                       # 枪械修改补丁
├── Lang/                          # 本地化生成器（EN / zh-CN）
├── Mechanism/                     # 不排泄
├── Misc/                          # 无观察者
├── Patch/                         # 控制台自动补全、拼音搜索
├── UI/                            # 弹药 HUD、整理按钮、重量显示、按键处理
├── BepInEx/plugins/               # TinyPinyin.dll 依赖
├── README.md / README_ZH.md       # 文档
└── CHANGELOG.md / CHANGELOG_ZH.md
```

## 许可

MIT
