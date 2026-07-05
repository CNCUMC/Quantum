## 1.1.0

### 新增

* 不排泄：昏迷时不再排泄

### 更改

* 拆分 `Patch/` 内容到功能分类目录：
    - `Info/` — 配置-信息（Ctrl 展开、耐久警报）
    - `ItemChange/` — 物品描述增强
    - `ItemChange/Gun/` — 枪械功能
    - `Misc/` — 杂项（无观察者）
    - `Mechanism/` — 机制（不排泄）
    - `UI/` — 用户界面（弹药UI、整理按钮、重量显示、按键处理）
    - `Patch/PlayerCameraPatch.cs` — 仅保留拼音搜索
* 统一日志本地化格式：`log.{class_name}.{specific_key}`
* Item.Update 耐久检查改为两级节流（帧 + 2 秒间隔）
* 耐久警报改为每下降 5% 提醒一次（而非仅首次）
