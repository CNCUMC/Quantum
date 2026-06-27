# Quantum

A quality-of-life mod for *Casualties Unknown* that adds crafting search, container sorting, gun modifiers, and gameplay tweaks — all configurable from the in-game **Quantum** options tab.

## Features

### Crafting & Info
- **Ctrl + Shift hover** — expanded item info (recipes, usable flags, ignore depression)
- **Pinyin search** — search crafting recipes by pinyin initials (`bd` → 绷带) or full pinyin

### Inventory
- **Container sort** — sort by name, value, or weight (ascending / descending)
- **Total value** — displayed next to weight in the inventory bar

### HUD
- **Ammunition display** — shows current / max ammo above the gun menu (auto-hides in menus / pause)

### Gun Modifiers
- Auto rack, indestructible gun, infinite ammo, never jam, no casing, recoilless

### Gameplay
- **Favourite durability alert** — alerts every 5% durability drop for favourited items
- **Bilingual item names** — appends a translation from any installed language to item names
- **No observer** — disables observers
- **Don't shit** — no defecation while unconscious
- **Tab / Esc** — close all open UI panels

### Console
- **Tab auto-complete** with scrollable candidate list
- **Ctrl+Z** history undo
- **History limit** configurable

## Settings

All options are in the **Quantum** tab of the game's settings menu (powered by CUCoreLib). Changes apply immediately.

## Requirements

- [BepInEx 5](https://github.com/BepInEx/BepInEx)
- [CUCoreLib](https://github.com/CNCUMC/CUCoreLib) ≥ 1.0.1
- [Bark](https://github.com/CNCUMC/Bark) ≥ 1.0.0

## Installation

1. Install BepInEx, CUCoreLib, and Bark to your game directory.
2. Copy `Quantum.dll` and `TinyPinyin.dll` into `BepInEx/plugins/Quantum/`.
3. Launch the game. The **Quantum** tab appears in Settings.

## Localization

English and Simplified Chinese are built in. To add more languages:
1. Run `createLocale` in the in-game console to generate `EN.json` in `BepInEx/config/CUCoreLib/Locales/`.
2. Copy `EN.json` → `{languageCode}.json`, translate the values, and restart.

## License

MIT
