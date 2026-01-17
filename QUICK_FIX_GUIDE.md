# 快速修复指南

## 🔍 问题诊断

### 问题1：点击按钮没有反应

**可能原因**：
1. `ViewSwitcher` 的 `Interior View` 字段未设置
2. `ViewSwitcher` 的 `Switch Button` 字段未设置
3. 按钮的 `OnClick` 事件未连接

**解决方法**：

#### 步骤1：检查 ViewSwitcher 设置

1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中检查：
   - `Interior View` 字段：应该显示你创建的 `CarInternalView`（或类似名称）对象
   - `Switch Button` 字段：应该显示 `ViewSwitchButton`
   - `Button Text` 字段：应该显示按钮下的 `Text` 子对象

#### 步骤2：如果字段未设置

1. **设置 Interior View**：
   - 在 Hierarchy 中找到你创建的 `CarInternalView`（包含 background、mother、father 的父对象）
   - 将该对象拖拽到 `ViewSwitcher` 的 `Interior View` 字段

2. **设置 Switch Button**：
   - 在 Hierarchy 中找到 `ViewSwitchButton`
   - 将该按钮拖拽到 `ViewSwitcher` 的 `Switch Button` 字段

3. **设置 Button Text**（可选）：
   - 展开 `ViewSwitchButton`，找到 `Text` 子对象
   - 将该 `Text` 对象拖拽到 `ViewSwitcher` 的 `Button Text` 字段

#### 步骤3：验证按钮连接

运行游戏后，点击按钮时，Console 应该显示：
```
ViewSwitcher: ToggleView 被调用，当前视角: Interior
ViewSwitcher: 切换到车前窗视角
ViewSwitcher: SwitchView 被调用，目标视角: FrontWindow
...
```

如果没有这些输出，说明按钮未正确连接。

---

### 问题2：鼠标悬停不显示人物状态

**可能原因**：
1. 人物对象没有 `Collider2D` 组件
2. `SimpleHoverTooltip` 的 UI 组件未设置
3. 人物对象没有 `CarOccupant` 组件

**解决方法**：

#### 步骤1：检查人物对象

1. 选中 `Father_0` 或 `Mather_0` 对象
2. 在 Inspector 中检查是否有：
   - ✅ `CarOccupant` 组件
   - ✅ `Collider2D` 组件（`BoxCollider2D` 或 `CircleCollider2D`）
   - ✅ `SpriteRenderer` 组件

#### 步骤2：如果没有 Collider2D

1. 选中人物对象
2. 在 Inspector 中点击 `Add Component`
3. 搜索并添加 `Box Collider 2D`
4. 调整 Collider 的大小，使其覆盖人物图像区域
5. **重要**：确保 `Is Trigger` 未勾选（或根据你的需求设置）

#### 步骤3：检查 SimpleHoverTooltip

1. 在 Hierarchy 中找到包含 `SimpleHoverTooltip` 组件的对象
2. 在 Inspector 的 `Simple Hover Tooltip` 组件中：
   - `Tooltip Panel`、`Name Text`、`Satiety Text`、`Disguise Text` 字段可以为空
   - 脚本会在运行时自动创建这些UI元素

#### 步骤4：验证设置

运行游戏后，鼠标悬停在人物上时，应该看到：
- 一个半透明的黑色面板
- 显示人物名称、饱腹度、伪装度

---

## 🛠️ 使用诊断工具

### 快速诊断

1. 在场景中创建空 GameObject，命名为 `DiagnosticTool`
2. 添加 `DiagnoseViewSystem` 组件
3. 右键点击组件，选择 `诊断所有问题`
4. 查看 Console 输出，会显示所有问题

### 单独诊断

- `诊断 ViewSwitcher`：只检查视角切换系统
- `诊断悬停提示系统`：只检查悬停提示系统

---

## ✅ 检查清单

运行游戏前，确保：

### ViewSwitcher 设置：
- [ ] `Interior View` 字段已设置（不是 `None`）
- [ ] `Switch Button` 字段已设置（不是 `None`）
- [ ] `Button Text` 字段已设置（可选）

### 人物对象设置：
- [ ] `Father_0` 和 `Mather_0` 有 `CarOccupant` 组件
- [ ] `Father_0` 和 `Mather_0` 有 `Collider2D` 组件
- [ ] `Father_0` 和 `Mather_0` 有 `SpriteRenderer` 组件
- [ ] Collider 的大小覆盖人物图像区域

### SimpleHoverTooltip 设置：
- [ ] 场景中有 `SimpleHoverTooltip` 组件（UI组件会自动创建）

---

## 🎮 测试步骤

1. **运行游戏**
2. **测试按钮**：
   - 点击右下角的"车前窗"按钮
   - 查看 Console 是否有 `ToggleView 被调用` 的输出
   - 查看画面是否切换

3. **测试悬停提示**：
   - 将鼠标移动到 `Father_0` 或 `Mather_0` 上
   - 应该看到人物状态提示框
   - 查看 Console 是否有相关输出

---

## 📝 如果还是不行

1. **运行诊断工具**：
   - 使用 `DiagnoseViewSystem` 组件诊断问题
   - 查看 Console 输出的详细错误信息

2. **检查 Console 输出**：
   - 运行游戏时查看所有警告和错误
   - 点击按钮时查看是否有相关输出

3. **提供以下信息**：
   - 诊断工具的 Console 输出
   - `ViewSwitcher` 组件的 Inspector 截图
   - 人物对象的 Inspector 截图（显示组件列表）
