![Logo](Logo.png)

# Quantum

[中文指南](README_ZH.md)

[GitHub](https://github.com/CNCUMC/Quantum) | [NexusMods](https://www.nexusmods.com/scavprototype/mods/365)

_A quality-of-life mod for [Casualties Unknown](https://store.steampowered.com/app/3624440/Casualties_Unknown_Demo/) —
crafting search, container sorting, gun modifiers, and gameplay tweaks, all configurable from the in-game settings
menu._

---

## Features

| Feature                  | Description                                                                                  |
|--------------------------|----------------------------------------------------------------------------------------------|
| **Ctrl + Shift Hover**   | Expanded item info panel (recipes, usable flags, ignore depression)                          |
| **Pinyin Recipe Search** | Search crafting recipes by pinyin initials (`bd` → 绷带) or full pinyin                        |
| **Container Sort**       | Sort inventory by name / value / weight (ascending / descending) with sort buttons           |
| **Total Value Display**  | Shows total item value next to weight in the inventory bar                                   |
| **Ammunition HUD**       | Real-time current / max ammo display above the gun menu (auto-hides in menus / pause)        |
| **Gun Modifiers**        | Auto rack, indestructible gun, infinite ammo, never jam, no casing, recoilless               |
| **Durability Alert**     | Alerts every 5% durability drop for favourited items                                         |
| **Bilingual Item Names** | Appends a translation from any installed language to item names                              |
| **Console Enhancements** | Tab auto-complete with scrollable candidates, Up/Down navigation, Ctrl+Z undo, history limit |
| **Fuzzy Dot Match**      | Type `xX` to match `xxx.XXX` in parameter autocomplete                                       |
| **No Observer**          | Disables observers entirely                                                                  |
| **Don't Shit**           | No defecation while unconscious                                                              |
| **Tab / Esc Close All**  | One key closes all open UI panels (crafting, wound view, trade)                              |
| **Hide Demo Tips**       | Hides the demo-version tips panel                                                            |

## Requirements

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx)
- [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) ≥ 1.0.2
- [Bark](https://github.com/CNCUMC/Bark) ≥ 1.0.2

## Installation

1. Install BepInEx 5.x for Casualties Unknown.
2. Install [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) — place `CUCoreLib.dll` into `BepInEx/plugins/`.
3. Install [Bark](https://github.com/CNCUMC/Bark) — place `Bark.dll` into `BepInEx/plugins/`.
4. Download the latest `Quantum-v*.zip` from [Releases](https://github.com/CNCUMC/Quantum/releases).
5. Extract to your game directory. `Quantum.dll` and `TinyPinyin.dll` go into `BepInEx/plugins/Quantum/`.
6. Launch the game. The **Quantum** tab appears in Settings.

## Settings

All options are in the game's settings menu (powered by [CUCoreLib](https://github.com/CNCUMC/CUCoreLib)):

- **Quantum** tab — gun modifiers, gameplay tweaks, UI toggles
- **Video** tab — info display, candidate limits, demo tips
- **Input** tab — sort key, console scroll speed

Changes apply immediately.

## Localization

English and Simplified Chinese are built in. To add more languages:

1. Run `createLocale` in the in-game console to generate `EN.json` in `BepInEx/config/CUCoreLib/Locales/`.
2. Copy `EN.json` → `{languageCode}.json`, translate the values, and restart.

## Project Structure

```
Quantum/
├── Plugin.cs                      # Entry point + CCL settings registration
├── Info/                          # Ctrl expand, durability alert
├── ItemChange/                    # Item description + bilingual names
│   └── Gun/                       # Gun modifier patches
├── Lang/                          # Localization generators (EN / zh-CN)
├── Mechanism/                     # Don't Shit
├── Misc/                          # No Observer
├── Patch/                         # Console autocomplete, pinyin search
├── UI/                            # Ammo HUD, sort buttons, weight, input handling
├── BepInEx/plugins/               # TinyPinyin.dll dependency
├── README.md / README_ZH.md       # Documentation
└── CHANGELOG.md / CHANGELOG_ZH.md
```

## License

MIT
