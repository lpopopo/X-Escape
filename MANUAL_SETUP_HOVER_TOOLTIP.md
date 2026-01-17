# 鼠标悬停显示人物状态 - 手动设置指南

## 完整设置步骤

### 步骤 1：设置 TooltipManager（显示提示的UI管理器）

1. **创建空物体**
   - 在 Hierarchy 窗口右键 → `Create Empty`
   - 命名为 `TooltipManager`

2. **添加 SimpleHoverTooltip 脚本**
   - 选中 `TooltipManager`
   - Inspector → `Add Component`
   - 搜索 `SimpleHoverTooltip`
   - 点击添加

3. **验证**
   - 运行游戏
   - 查看 Console，应该能看到初始化信息
   - 如果没有错误，继续下一步

### 步骤 2：为 Father_0 添加 Collider2D

1. **选中 Father_0**
   - 在 Hierarchy 中找到 `Father_0` GameObject
   - 选中它

2. **添加 Box Collider 2D**
   - Inspector → `Add Component`
   - 搜索 `Box Collider 2D`
   - 点击添加

3. **调整 Collider 大小**
   - 在 Inspector 中找到 `Box Collider 2D` 组件
   - 找到 `Size` 参数
   - 调整 X 和 Y 值，使 Collider 覆盖人物图像
   - **建议值**：如果人物图像大小未知，先设置为 `X: 1, Y: 1`
   - 如果太小，增加到 `X: 2, Y: 2` 或更大

4. **确保 Is Trigger 未勾选**
   - 在 `Box Collider 2D` 组件中
   - 确保 `Is Trigger` 复选框**未勾选**（重要！）

5. **验证 Collider**
   - 在 Scene 视图中选中 `Father_0`
   - 应该能看到绿色边框（Collider 的显示）
   - 确保绿色边框覆盖人物图像

### 步骤 3：为 Mather_0 添加 Collider2D

1. **选中 Mather_0**
   - 在 Hierarchy 中找到 `Mather_0` GameObject
   - 选中它

2. **添加 Box Collider 2D**
   - Inspector → `Add Component`
   - 搜索 `Box Collider 2D`
   - 点击添加

3. **调整 Collider 大小**
   - 在 Inspector 中找到 `Box Collider 2D` 组件
   - 调整 `Size` 的 X 和 Y 值
   - 使 Collider 覆盖人物图像

4. **确保 Is Trigger 未勾选**
   - 确保 `Is Trigger` 复选框**未勾选**

5. **验证 Collider**
   - 在 Scene 视图中查看绿色边框
   - 确保覆盖人物图像

### 步骤 4：检查 CarOccupant 组件设置

**对 Father_0 和 Mather_0 都检查：**

1. **选中人物**
   - 选中 `Father_0` 或 `Mather_0`

2. **检查 CarOccupant 组件**
   - 在 Inspector 中找到 `CarOccupant` 组件
   - 检查 `Occupant Name` 字段：
     - Father_0 应该设置为：`Father_0`
     - Mather_0 应该设置为：`Mather_0`

3. **检查状态值（可选）**
   - `Satiety`（饱腹度）：0-100，建议设置为 50
   - `Disguise`（伪装度）：0-100，建议设置为 50

### 步骤 5：测试

1. **保存场景**
   - 按 `Ctrl+S`（Windows）或 `Cmd+S`（Mac）

2. **运行游戏**
   - 点击 Play 按钮

3. **测试鼠标悬停**
   - 将鼠标移动到 `Father_0` 上
   - 应该能看到提示显示：
     - 名称：Father_0
     - 饱腹度：正常 (50%)
     - 伪装度：可疑 (50%)
   - 将鼠标移动到 `Mather_0` 上
   - 应该能看到类似的提示

## 检查清单

在运行游戏前，确保：

- [ ] TooltipManager 已创建并添加了 `SimpleHoverTooltip` 组件
- [ ] Father_0 有 `CarOccupant` 组件，`Occupant Name` = `Father_0`
- [ ] Father_0 有 `Box Collider 2D` 组件
- [ ] Father_0 的 Collider `Size` 覆盖人物图像
- [ ] Father_0 的 Collider `Is Trigger` 未勾选
- [ ] Mather_0 有 `CarOccupant` 组件，`Occupant Name` = `Mather_0`
- [ ] Mather_0 有 `Box Collider 2D` 组件
- [ ] Mather_0 的 Collider `Size` 覆盖人物图像
- [ ] Mather_0 的 Collider `Is Trigger` 未勾选

## 常见问题

### 问题 1：Collider 大小不知道设置多少

**解决方案**：
1. 在 Scene 视图中选中人物
2. 查看人物图像的大小
3. 如果人物图像大约 1 个单位，设置 `Size: X=1, Y=1`
4. 如果人物图像更大，相应增加
5. **技巧**：先设置大一点（如 X=2, Y=2），确保能检测到，然后慢慢缩小到合适大小

### 问题 2：在 Scene 视图中看不到 Collider 的绿色边框

**解决方案**：
1. 确保选中了人物 GameObject
2. 在 Scene 视图的右上角，点击 `Gizmos` 按钮
3. 确保 `Gizmos` 已启用
4. 或者按快捷键 `G` 切换 Gizmos 显示

### 问题 3：鼠标悬停没有反应

**检查步骤**：
1. 查看 Console 窗口（`Window` → `General` → `Console`）
2. 运行游戏，查看是否有错误信息
3. 检查 `SimpleHoverTooltip` 的 Start 输出：
   - 应该显示找到多少个 CarOccupant
   - 应该显示每个 CarOccupant 是否有 Collider

### 问题 4：提示显示但位置不对

**解决方案**：
1. 选中 TooltipManager
2. 在 `SimpleHoverTooltip` 组件中
3. 调整 `Offset X` 和 `Offset Y` 值

## 详细操作截图说明

### 添加 Box Collider 2D：

```
Inspector 面板
└── Father_0 (GameObject)
    ├── Transform
    ├── CarOccupant (Script)
    └── [点击 Add Component]
        └── Physics 2D
            └── Box Collider 2D  ← 点击这里
```

### 调整 Collider Size：

```
Box Collider 2D 组件
├── Edit Collider (按钮)
├── Size
│   ├── X: 1  ← 调整这个值
│   └── Y: 1  ← 调整这个值
├── Offset
│   ├── X: 0
│   └── Y: 0
└── Is Trigger: ☐  ← 确保未勾选！
```

## 完成后的效果

运行游戏后：
- 鼠标移到 Father_0 上 → 显示提示
- 鼠标移到 Mather_0 上 → 显示提示
- 鼠标移开 → 提示消失

提示内容：
```
Father_0
饱腹度: 正常 (50%)
伪装度: 可疑 (50%)
```
