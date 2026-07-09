# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## v1.1.0

### Added

- **Pinyin recipe search** — search crafting recipes by pinyin initials (`bd` → 绷带) or full pinyin via TinyPinyin
- **Container sort buttons** — sort by name / value / weight (ascending / descending) with in-game toggle buttons
- **Total value display** — total item value shown next to weight in the inventory bar
- **Ammunition HUD** — real-time current / max ammo display above the gun menu (auto-hides in menus / pause)
- **Gun modifiers** — auto rack, indestructible gun, infinite ammo, never jam, no casing, recoilless
- **Favourite durability alert** — alerts every 5% durability drop for favourited items
- **Bilingual item names** — appends a translation from any installed language to item names via settings dropdown
- **Console autocomplete** — Tab auto-complete with scrollable candidate list, Up/Down parameter navigation, Ctrl+Z undo, configurable history limit
- **Fuzzy dot match** — type `xX` to match `xxx.XXX` in console parameter autocomplete
- **Candidate scroll speed setting** — configurable throttle speed for Up/Down candidate switching (Input tab)
- **No observer** — disables observers
- **Don't shit** — no defecation while unconscious
- **Tab / Esc close all** — one key closes all open UI panels (crafting, wound view, trade)
- **Hide demo tips** — hides the demo-version tips panel
- **Bark localization** — EN/zh-CN fallbacks registered via `Lang/EnLangGenerator` and `Lang/ZhCnLangGenerator`
- **CCL settings** — all options registered via `ModOptionsRegistry` in the in-game **Quantum** settings tab
- **TMP arrow key fix** — patches `TMP_InputField.KeyPressed` to prevent Up/Down cursor flicker during autocomplete

### Changed

- **Config system** — migrated from BepInEx `ConfigEntry<T>` to CUCoreLib `ModOptionsRegistry` + `BetterOptions`
- **Localization** — migrated from MossLib generators to Bark `BetterLocale` + CCL locale files
- **Project structure** — `Patch/` contents split into functional directories: `Info/`, `ItemChange/`, `Misc/`, `Mechanism/`, `UI/`
- **Locale key format** — unified to `log.{class_name}.{specific_key}` pattern
- **Durability check** — throttle interval reduced to frame + 2-second two-level check
- **Dependency** — added Bark (`org.cncumc.bark` ≥ 1.0.2)
- **Dependency** — CUCoreLib ≥ 1.0.2
