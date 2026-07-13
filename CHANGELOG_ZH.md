# 更新日志

本文件记录本项目所有重要变更。

格式基于 [Keep a Changelog](https://keepachangelog.com/)，本项目遵循 [语义化版本控制](https://semver.org/)。

---

## v1.1.0

### 新增

- **调试屏幕**（F3）：滑出式面板显示调试信息
  - 左侧面板：游戏版本、性能分析（FPS/帧时间）、世界信息（坐标、目标方块、层级）
  - 右侧面板：系统信息（CPU、GPU、操作系统）
  - 可配置滑动速度（0~100ms）
  - 使用 `Mathf.SmoothDamp` 平滑动画
  - 使用 Unifont 字体渲染 IMGUI
  - 基于组的架构：`DebugInfoGroup` 和 `DebugInfo` 类
  - `TurnLeft()` / `TurnRight()` 方法用于运行时切换显示位置
  - `DebugInfo.Side` 设置时覆盖组设置，未设置时继承组设置

- **控制台参数切换**：按住上下箭头循环切换自动补全候选
  - 可配置速度（0~100ms，默认 50ms）
  - `0` = 按一下切换一次
  - Shift = 2倍速
  - Ctrl+Shift = 5倍速
  - Ctrl = 无效果

### 变更

- 控制台滚动速度范围：`0.0001f~0.01f` → `0f~0.1f`（0.1ms~100ms）
- 控制台滚动速度默认值：`0.01f` → `0.05f`（10ms → 50ms）
