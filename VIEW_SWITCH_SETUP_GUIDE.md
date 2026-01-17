# 视角切换系统设置指南

## 📋 系统概述

这个系统实现了两个视角的切换：
1. **车内视角**：显示车内场景（人物、背景图）
2. **车前窗视角**：显示车前窗外的场景（背景图）

右下角有一个按钮可以切换视角。

---

## 🚀 快速设置步骤

### 步骤1：确保场景中有 ViewSwitcher

1. 在 Unity 编辑器中打开 `CarScene` 场景
2. 在 Hierarchy 中查找 `ViewSwitcher` GameObject
3. 如果没有，创建：
   - Hierarchy 右键 → `Create Empty` → 命名为 `ViewSwitcher`
   - 选中 `ViewSwitcher`，在 Inspector 中点击 `Add Component`
   - 搜索并添加 `View Switcher` 组件

### 步骤2：设置车内场景

车内场景应该包含：
- 背景图（使用 `CarInteriorView` 组件）
- 人物对象（使用 `CarOccupant` 组件）

#### 2.1 设置车内背景

1. 在 Hierarchy 中找到或创建车内背景对象（例如：`background-0_0`）
2. 如果对象还没有 `CarInteriorView` 组件：
   - 选中对象
   - 在 Inspector 中点击 `Add Component`
   - 搜索并添加 `Car Interior View` 组件
3. 在 `CarInteriorView` 组件中：
   - 设置 `Interior Sprite`（车内背景图片）
   - 或设置 `Resource Path`（从 Resources 文件夹加载）

#### 2.2 设置人物

1. 在 Hierarchy 中找到人物对象（例如：`Father_0`、`Mather_0`）
2. 确保人物对象有：
   - `CarOccupant` 组件（人物信息）
   - `SpriteRenderer` 组件（显示人物图片）
   - `Collider2D` 组件（用于鼠标悬停检测）
3. 在 `CarOccupant` 组件中设置：
   - `Occupant Name`：人物名称
   - `Satiety`：饱腹度（0-100）
   - `Disguise`：伪装度（0-100）

#### 2.3 将车内场景连接到 ViewSwitcher

1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 找到 `Interior View` 字段
   - 将包含 `CarInteriorView` 组件的对象拖拽到该字段
   - **注意**：如果车内场景包含多个对象（背景+人物），可以：
     - 创建一个父对象包含所有车内对象
     - 或将背景对象拖拽到 `Interior View` 字段（人物会自动跟随显示/隐藏）

### 步骤3：设置车前窗场景

#### 3.1 创建车前窗背景

`ViewSwitcher` 会自动创建 `FrontWindowView` 对象，但你可以手动设置：

1. 在 Hierarchy 中查找 `FrontWindowView` 对象
2. 如果没有，`ViewSwitcher` 会在运行时自动创建
3. 手动创建（推荐）：
   - Hierarchy 右键 → `Create Empty` → 命名为 `FrontWindowView`
   - 添加 `SpriteRenderer` 组件
   - 设置背景图片：
     - 在 `ViewSwitcher` 组件中设置 `Front Window Background` Sprite
     - 或设置 `Front Window Resource Path`（例如：`background-back`）

#### 3.2 将车前窗场景连接到 ViewSwitcher

1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 找到 `Front Window View` 字段
   - 将 `FrontWindowView` 对象拖拽到该字段

### 步骤4：设置切换按钮

#### 4.1 创建按钮（如果还没有）

1. 在包含 UI 的 Canvas 下：
   - 右键 Canvas → `UI` → `Button` → 命名为 `ViewSwitchButton`
2. 设置按钮位置（右下角）：
   - 选中按钮，在 Inspector 的 `RectTransform` 中：
     - `Anchor`: 右下角（Bottom Right）
     - `Pos X`: -20
     - `Pos Y`: 20
     - `Width`: 120
     - `Height`: 40
3. 设置按钮文本：
   - 展开按钮，选中 `Text` 子对象
   - 设置文本为 `车前窗`
   - 设置字体大小：16

#### 4.2 连接按钮到 ViewSwitcher

**方法1：在 ViewSwitcher 中设置（推荐）**
1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 将 `ViewSwitchButton` 拖拽到 `Switch Button` 字段
   - 将按钮下的 `Text` 子对象拖拽到 `Button Text` 字段

**方法2：在按钮的 OnClick 事件中设置**
1. 选中 `ViewSwitchButton`
2. 在 Inspector 的 `Button` 组件中：
   - 查看 `OnClick()` 事件列表
   - 如果为空，点击 `+` 添加新事件
   - 将 `ViewSwitcher` GameObject 拖拽到对象字段
   - 在下拉菜单中选择 `ViewSwitcher` → `ToggleView()`

---

## 📐 场景结构建议

推荐的 Hierarchy 结构：

```
CarScene
├── Main Camera
├── ViewSwitcher (ViewSwitcher 组件)
├── InteriorScene (父对象，包含所有车内内容)
│   ├── Background (CarInteriorView 组件)
│   ├── Father_0 (CarOccupant 组件)
│   └── Mather_0 (CarOccupant 组件)
├── FrontWindowView (车前窗场景)
│   └── Background (SpriteRenderer)
└── Canvas
    └── ViewSwitchButton
```

---

## ✅ 检查清单

在运行游戏前，确保：

- [ ] `ViewSwitcher` GameObject 存在于场景中
- [ ] `ViewSwitcher` 组件已添加
- [ ] `Interior View` 字段已设置（车内场景对象）
- [ ] `Front Window View` 字段已设置（车前窗场景对象）
- [ ] `Switch Button` 字段已设置（切换按钮）
- [ ] `Button Text` 字段已设置（按钮文本组件）
- [ ] 车内场景对象有 `CarInteriorView` 组件
- [ ] 人物对象有 `CarOccupant` 组件和 `Collider2D` 组件
- [ ] 车前窗场景对象有 `SpriteRenderer` 组件并设置了背景图

---

## 🎮 测试步骤

1. **运行游戏**
2. **检查初始状态**：
   - 应该看到车内场景（背景+人物）
   - 右下角有"车前窗"按钮
3. **点击"车前窗"按钮**：
   - 应该切换到车前窗视角
   - 按钮文本变为"车内"
   - 车内场景隐藏，车前窗场景显示
4. **点击"车内"按钮**：
   - 应该切换回车内视角
   - 按钮文本变为"车前窗"
   - 车前窗场景隐藏，车内场景显示

---

## 🔧 常见问题

### 问题1：点击按钮没有反应

**原因**：按钮未连接到 ViewSwitcher

**解决**：
1. 检查 `ViewSwitcher` 的 `Switch Button` 字段是否已设置
2. 检查按钮的 `OnClick` 事件是否连接到 `ViewSwitcher.ToggleView()`
3. 检查按钮的 `Interactable` 是否为 true

### 问题2：切换后看不到画面

**原因**：场景对象未设置或未激活

**解决**：
1. 检查 `Interior View` 和 `Front Window View` 字段是否已设置
2. 检查场景对象是否激活（`Active` 勾选）
3. 检查场景对象是否有正确的组件（`CarInteriorView`、`SpriteRenderer`）

### 问题3：人物在切换后消失

**原因**：人物对象不是 `Interior View` 的子对象

**解决**：
1. 将人物对象设置为 `Interior View` 的子对象
2. 或创建一个父对象包含所有车内对象，将该父对象设置为 `Interior View`

### 问题4：车前窗背景不显示

**原因**：背景图未设置

**解决**：
1. 检查 `FrontWindowView` 对象是否有 `SpriteRenderer` 组件
2. 在 `ViewSwitcher` 组件中设置 `Front Window Background` Sprite
3. 或设置 `Front Window Resource Path`（例如：`background-back`）
4. 确保图片在 `Resources` 文件夹中（如果使用 Resource Path）

---

## 📝 总结

视角切换系统的核心是：
1. **ViewSwitcher** 组件控制切换逻辑
2. **Interior View** 包含车内场景（背景+人物）
3. **Front Window View** 包含车前窗场景（背景）
4. **切换按钮** 触发切换

确保所有字段都正确设置，系统就能正常工作！
