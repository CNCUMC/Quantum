# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/), and this project adheres to [Semantic Versioning](https://semver.org/).

---

## v1.1.1

### Added

- **Camera Zoom**: Hold ZoomKey (default: `\`) + scroll wheel to zoom in/out
    - Smooth zoom animation with exponential interpolation
    - Zoom ratio UI display (follows HiddenHud visibility)
    - Double-tap ZoomKey to reset zoom to 1x
    - SightLimiter sync for zoom mask consistency
    - Configurable settings: ZoomSensitivity (0.1~2.0), SmoothZoom toggle
    - Persistent zoom multiplier across sessions

### Changed

- **Project Structure**: Renamed `UI/` folder to `Video/` for better organization
