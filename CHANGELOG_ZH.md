# 更新日志

本文件记录本项目所有重要变更。

格式基于 [Keep a Changelog](https://keepachangelog.com/)，本项目遵循 [语义化版本控制](https://semver.org/)。

---

## v1.1.1

### 新增

- **相机缩放**：按住 ZoomKey（默认：`\`）+ 滚轮缩放
    - 指数插值平滑缩放动画
    - 缩放倍率 UI 显示（跟随 HiddenHud 显隐）
    - 双击 ZoomKey 重置缩放至 1x
    - SightLimiter 同步缩放遮罩
    - 可配置设置：ZoomSensitivity（0.1~2.0）、SmoothZoom 开关
    - 跨会话持久化缩放倍率

### 变更

- **项目结构**：`UI/` 文件夹重命名为 `Video/`，优化目录组织
