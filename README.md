![Logo](Logo.png)

# Quantum

[中文指南](README_ZH.md)

[GitHub](https://github.com/CNCUMC/Quantum) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/365)

_A quality-of-life mod for [Casualties Unknown](https://store.steampowered.com/app/4576490/) —
crafting search, container sorting, gun modifiers, and gameplay tweaks, all configurable from the in-game settings
menu._

**This mod supports Russian (thanks @Crescia1949!)**

_He is not a native speaker of Russian. If you have any suggestions, please report them promptly in the `NexusMods post` or `GitHub issues`._

**Этот мод поддерживает русский язык (спасибо @Crescia1949!)**

_Он не является носителем русского языка. Если у вас есть какие-либо предложения, пожалуйста, незамедлительно сообщите о них в разделе `NexusMods post` или `GitHub issues`._

---

## Features

| Feature                         | Description                                                                                         |
|---------------------------------|-----------------------------------------------------------------------------------------------------|
| **Ctrl + Shift Hover**          | Expanded item info panel (recipes, usable flags, ignore depression)                                 |
| **Pinyin Recipe Search**        | Search crafting recipes by pinyin initials (`bd` → 绷带) or full pinyin                               |
| **Container Sort**              | Sort inventory by name / value / weight (ascending / descending) with sort buttons                  |
| **Total Value Display**         | Shows total item value next to weight in the inventory bar                                          |
| **Ammunition HUD**              | Real-time current / max ammo display above the gun menu (auto-hides in menus / pause)               |
| **Gun Modifiers**               | Auto rack, indestructible gun, infinite ammo, never jam, no casing, recoilless                      |
| **Durability Alert**            | Alerts every 5% durability drop for favourited items                                                |
| **Bilingual Item Names**        | Appends a translation from any installed language to item names                                     |
| **Console Enhancements**        | Tab auto-complete with scrollable candidates, Up/Down navigation, Ctrl+Z undo, history limit        |
| **Console Parameter Switching** | Hold Up/Down to cycle through autocomplete candidates (configurable speed, Shift 2x, Ctrl+Shift 5x) |
| **Fuzzy Dot Match**             | Type `xX` to match `xxx.XXX` in parameter autocomplete                                              |
| **No Observer**                 | Disables observers entirely                                                                         |
| **Don't Shit**                  | No defecation while unconscious                                                                     |
| **Tab / Esc Close All**         | One key closes all open UI panels (crafting, wound view, trade)                                     |
| **Settings Menu**               | ESC to close, Tab / Shift+Tab to switch tabs (including CUCoreLib custom tabs)                      |
| **Hide Demo Tips**              | Hides the demo-version tips panel                                                                   |
| **Debug Screen (F3)**           | Slide-out panels with game info, profiler, world data, and system info. Configurable speed.         |
| **FPS Graph (F3+F)**            | Real-time frame time chart (120 frames, color-coded, 60/30 FPS reference lines)                     |
| **Auto Sandbox**                | Skip tutorial course selection and enter sandbox mode directly                                      |

## Requirements

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx)
- [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) ≥ 1.0.3
- [Bark](https://github.com/CNCUMC/Bark) ≥ 1.1.1

## Installation

1. Install BepInEx 5.x for Casualties Unknown.
2. Install [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) — place `CUCoreLib.dll` into `BepInEx/plugins/`.
3. Install [Bark](https://github.com/CNCUMC/Bark) — place `Bark.dll` into `BepInEx/plugins/Bark/`.
4. Install `Quantum`.
5. Launch the game. The **Quantum** tab appears in Settings.

## Settings

All options are in the game's settings menu (powered by [CUCoreLib](https://github.com/jimmyking9999999/CUCoreLib) & [Bark](https://github.com/CNCUMC/Bark)):

- **Quantum** tab — gun modifiers, gameplay tweaks, UI toggles
- **Video** tab — info display, candidate limits, demo tips
- **Input** tab — sort key, console scroll speed, debug screen speed

## License

LGPL v3
