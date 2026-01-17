# 快速修复视角切换问题

## 🔍 问题分析

从日志中看到：
1. ✅ 按钮已连接：`CarViewSystem: 按钮已连接: ViewSwitchButton`
2. ❌ 车内场景未设置：`车内场景: null`
3. ❌ 点击按钮没有输出：说明按钮点击事件没有触发

## 🚀 快速修复步骤

### 步骤1：找到车内场景对象

1. 在 Unity 编辑器中，创建一个空 GameObject，命名为 `SceneFinder`
2. 添加 `FindInteriorScene` 组件
3. 右键点击组件，选择 `查找车内场景对象`
4. 查看 Console，找到车内场景对象的名称

### 步骤2：设置 CarViewSystem 的车内场景

**方法A：使用工具（推荐）**
1. 在 `SceneFinder` 的 `FindInteriorScene` 组件上
2. 右键点击，选择 `设置 CarViewSystem 的车内场景`
3. 查看 Console 确认设置成功

**方法B：手动设置**
1. 在 Hierarchy 中找到包含 `CarInteriorView` 组件的对象（可能是 `background-0_0` 或类似名称）
2. 选中 `CarViewSystem` GameObject
3. 在 Inspector 的 `Car View System` 组件中：
   - 将找到的对象拖拽到 `Interior Scene` 字段

### 步骤3：设置 ViewSwitcher 的车内场景（如果使用 ViewSwitcher）

**方法A：使用工具（推荐）**
1. 在 `SceneFinder` 的 `FindInteriorScene` 组件上
2. 右键点击，选择 `设置 ViewSwitcher 的车内场景`
3. 查看 Console 确认设置成功

**方法B：手动设置**
1. 选中 `ViewSwitcher` GameObject
2. 在 Inspector 的 `View Switcher` 组件中：
   - 将车内场景对象拖拽到 `Interior View` 字段

### 步骤4：检查按钮连接（最重要！）

1. 选中 `ViewSwitchButton`
2. 在 Inspector 的 `Button` 组件中：
   - 查看 `OnClick()` 事件列表
   - **确保有事件项，且连接到正确的系统**

#### 如果使用 CarViewSystem：
   - 事件应该连接到 `CarViewSystem` → `ToggleView()`

#### 如果使用 ViewSwitcher：
   - 事件应该连接到 `ViewSwitcher` → `ToggleView()`

3. **如果事件列表为空或连接错误**：
   - 点击 `+` 添加新事件
   - 将对应的 GameObject（`CarViewSystem` 或 `ViewSwitcher`）拖拽到对象字段
   - 选择 `ToggleView()` 方法

### 步骤5：选择使用哪个系统（重要！）

**场景中同时存在两个系统**：
- `CarViewSystem`（新的，使用相机切换）
- `ViewSwitcher`（旧的，使用场景切换）

**建议**：
- 如果只需要切换背景图，使用 `ViewSwitcher`（更简单）
- 如果需要真正的视角切换（相机位置不同），使用 `CarViewSystem`

**如果决定只使用一个**：
1. 禁用或删除另一个系统
2. 确保按钮只连接到一个系统

## ✅ 验证修复

运行游戏后，应该看到：

1. **启动时**：
   ```
   CarViewSystem: Start 被调用
     车内场景: [场景名称]  ← 不再是 null
   ```

2. **点击按钮时**：
   ```
   CarViewSystem: ToggleView 被调用，当前视角: Interior
   CarViewSystem: 准备切换到车前窗视角
   CarViewSystem: SwitchCameras 被调用，目标视角: FrontWindow
   ...
   ```

3. **画面应该切换**：
   - 点击"车前窗"按钮后，应该看到车前窗背景
   - 点击"车内"按钮后，应该看到车内场景

## 🔧 如果还是不行

1. **检查 Console 输出**：
   - 点击按钮时是否有 `ToggleView 被调用` 的输出？
   - 如果没有，说明按钮未连接

2. **检查按钮是否可交互**：
   - 选中按钮，查看 `Button` 组件的 `Interactable` 是否为 true
   - 检查按钮的 `Raycast Target` 是否被其他UI遮挡

3. **检查是否有多个按钮**：
   - 在 Hierarchy 中搜索 `ViewSwitchButton`
   - 确保只有一个按钮，且连接到正确的系统

4. **提供以下信息**：
   - Console 中点击按钮后的完整输出
   - `CarViewSystem` 或 `ViewSwitcher` 组件的 Inspector 截图
   - `ViewSwitchButton` 的 `OnClick` 事件配置截图
