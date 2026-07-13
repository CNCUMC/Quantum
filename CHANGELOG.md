# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## v1.1.0

### Added

- **Debug Screen** (F3): Slide-out panels showing debug information
  - Left panel: Game version, profiler (FPS/frame time), world info (position, target block, layer)
  - Right panel: System info (CPU, GPU, OS)
  - Configurable slide speed (0~100ms)
  - Smooth animation with `Mathf.SmoothDamp`
  - Uses Unifont font for IMGUI rendering
  - Group-based architecture with `DebugInfoGroup` and `DebugInfo` classes
  - `TurnLeft()` / `TurnRight()` methods for runtime side switching
  - `DebugInfo.Side` overrides group side when set, inherits when null

- **Console Parameter Switching**: Hold Up/Down to cycle through autocomplete candidates
  - Configurable speed (0~100ms, default 50ms)
  - `0` = one press, one switch
  - Shift = 2x speed
  - Ctrl+Shift = 5x speed
  - Ctrl = no effect

### Changed

- Console scroll speed range: `0.0001f~0.01f` → `0f~0.1f` (0.1ms~100ms)
- Console scroll speed default: `0.01f` → `0.05f` (10ms → 50ms)
