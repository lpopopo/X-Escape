# 视角切换系统设置检查清单

## ✅ 当前状态检查

根据场景文件分析，你已经设置了：

### 已完成的设置：
- ✅ `ViewSwitcher` GameObject 已创建
- ✅ `ViewSwitcher` 组件已添加
- ✅ `Switch Button` 已设置（ViewSwitchButton）
- ✅ `Button Text` 已设置
- ✅ 场景中有 `background-0_0`（车内背景）
- ✅ 场景中有 `Father_0`（人物）
- ✅ 场景中有 `Mather_0`（人物）

### ⚠️ 需要完成的设置：

1. **Interior View（车内场景）** - 未设置
   - 当前状态：`interiorView: {fileID: 0}`
   - 需要：将 `background-0_0` 或包含所有车内对象的父对象设置到这里

2. **Front Window View（车前窗场景）** - 未设置
   - 当前状态：`frontWindowView: {fileID: 0}`
   - 需要：创建或设置车前窗场景对象

---

## 🔧 需要执行的操作

### 操作1：设置车内场景（Interior View）

**方法A：直接使用背景对象（简单）**
1. 在 Unity 编辑器中，选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 找到 `Interior View` 字段
   - 在 Hierarchy 中找到 `background-0_0` 对象
   - 将 `background-0_0` 拖拽到 `Interior View` 字段

**方法B：创建父对象包含所有车内内容（推荐）**
1. 在 Hierarchy 中创建空 GameObject，命名为 `InteriorScene`
2. 将以下对象设置为 `InteriorScene` 的子对象：
   - `background-0_0`（背景）
   - `Father_0`（人物）
   - `Mather_0`（人物）
3. 选中 `ViewSwitcher` GameObject
4. 在 Inspector 的 `View Switcher` 组件中：
   - 将 `InteriorScene` 拖拽到 `Interior View` 字段

### 操作2：创建并设置车前窗场景（Front Window View）

**步骤1：创建车前窗背景对象**
1. Hierarchy 右键 → `Create Empty` → 命名为 `FrontWindowView`
2. 选中 `FrontWindowView`
3. 在 Inspector 中点击 `Add Component`
4. 搜索并添加 `Sprite Renderer` 组件

**步骤2：设置车前窗背景图**
有两种方式：

**方式A：使用 Resources 文件夹中的图片（推荐）**
1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 找到 `Front Window Resource Path` 字段
   - 设置为 `background-back`（如果图片在 Resources 文件夹中）
   - `ViewSwitcher` 会自动加载并设置到 `FrontWindowView`

**方式B：手动设置 Sprite**
1. 选中 `FrontWindowView`
2. 在 Inspector 的 `Sprite Renderer` 组件中：
   - 将车前窗背景图片（Sprite）拖拽到 `Sprite` 字段
3. 调整位置和大小：
   - 设置 `Sorting Order = -10`（背景层）
   - 调整 `Transform` 的位置和缩放，使其填满相机视野

**步骤3：连接到 ViewSwitcher**
1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 将 `FrontWindowView` 拖拽到 `Front Window View` 字段

### 操作3：验证设置

运行游戏前，检查以下内容：

- [ ] `ViewSwitcher` 的 `Interior View` 字段已设置
- [ ] `ViewSwitcher` 的 `Front Window View` 字段已设置
- [ ] `background-0_0` 对象有 `CarInteriorView` 组件（如果没有，需要添加）
- [ ] `Father_0` 和 `Mather_0` 有 `CarOccupant` 组件
- [ ] `Father_0` 和 `Mather_0` 有 `Collider2D` 组件（用于鼠标悬停）
- [ ] `FrontWindowView` 有 `SpriteRenderer` 组件并设置了背景图

---

## 🎮 测试步骤

1. **运行游戏**
2. **检查初始状态**：
   - 应该看到车内场景（`background-0_0` + 人物）
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

## 🔍 如果遇到问题

### 问题1：切换后看不到画面

**检查**：
1. `Interior View` 和 `Front Window View` 字段是否已设置
2. 场景对象是否激活（`Active` 勾选）
3. `background-0_0` 是否有 `CarInteriorView` 组件
4. `FrontWindowView` 是否有 `SpriteRenderer` 组件并设置了图片

### 问题2：人物在切换后消失

**原因**：人物对象不是 `Interior View` 的子对象

**解决**：
- 使用"方法B"创建 `InteriorScene` 父对象
- 将人物设置为 `InteriorScene` 的子对象

### 问题3：车前窗背景不显示

**检查**：
1. `FrontWindowView` 对象是否激活
2. `SpriteRenderer` 组件是否设置了 `Sprite`
3. `Sorting Order` 是否正确（建议 -10）
4. 如果使用 Resource Path，确保图片在 `Resources` 文件夹中

---

## 📝 快速检查命令

在 Unity 编辑器中：
1. 选中 `ViewSwitcher` GameObject
2. 查看 Inspector 中的 `View Switcher` 组件
3. 确认所有字段都已设置（不是 `None`）
