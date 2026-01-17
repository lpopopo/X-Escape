# 鼠标悬停提示故障排除指南

## 问题：鼠标悬停没有显示文字

### 检查清单

请按照以下步骤逐一检查：

#### ✅ 步骤 1：检查 OccupantHoverTooltip 组件是否存在

1. 在 Hierarchy 中查找是否有包含 `OccupantHoverTooltip` 组件的物体
2. 如果没有，创建一个空物体，命名为 "TooltipManager"
3. 添加 `OccupantHoverTooltip` 组件

**检查方法：**
- 在 Hierarchy 中搜索 "TooltipManager" 或 "OccupantHoverTooltip"
- 选中该物体，在 Inspector 中应该能看到 `OccupantHoverTooltip` 组件

#### ✅ 步骤 2：检查人物是否有 CarOccupant 组件

1. 选中 Father 或 Mother 物体
2. 在 Inspector 中检查是否有 `CarOccupant` 组件
3. 如果没有，添加 `CarOccupant` 组件

**检查方法：**
- Inspector → Add Component → 搜索 "CarOccupant"

#### ✅ 步骤 3：检查人物名称设置

1. 选中人物物体
2. 在 `CarOccupant` 组件中找到 `Occupant Name` 字段
3. 确保名称包含以下关键词之一：
   - "father" 或 "Father"
   - "mother" 或 "Mother"
   - "爸爸"
   - "妈妈"

**检查方法：**
- Inspector → CarOccupant → Occupant Name
- 名称不区分大小写，但必须包含关键词

#### ✅ 步骤 4：检查 Collider2D 组件

**这是最常见的问题！**

1. 选中人物物体
2. 在 Inspector 中检查是否有 `Collider2D` 组件（BoxCollider2D 或 CircleCollider2D）
3. 如果没有，添加一个：
   - Inspector → Add Component → Physics 2D → Box Collider 2D（或 Circle Collider 2D）
4. **重要**：调整 Collider 的大小，使其覆盖人物图像区域
5. **重要**：确保 Collider 的 `Is Trigger` 选项**不要勾选**（必须是 false）

**检查方法：**
- Inspector → Box Collider 2D → Size（调整大小）
- Inspector → Box Collider 2D → Is Trigger（确保未勾选）

#### ✅ 步骤 5：启用调试日志

1. 选中 TooltipManager 物体
2. 在 `OccupantHoverTooltip` 组件中
3. 勾选 `Enable Debug Log` 选项
4. 运行游戏，移动鼠标到人物上
5. 查看 Console 窗口的输出信息

**调试信息会显示：**
- 鼠标世界坐标
- 检测到的 Collider 名称
- 人物名称和是否为目标人物
- 是否显示提示

#### ✅ 步骤 6：检查 Canvas 和 UI

1. 在 Hierarchy 中查找 "TooltipCanvas" 或 "Canvas"
2. 如果没有，脚本会自动创建
3. 检查 Canvas 的 Render Mode：
   - 应该是 `Screen Space - Overlay`
4. 检查是否有 "TooltipPanel" 物体
5. 检查 TooltipPanel 是否激活（Inspector 中勾选）

**检查方法：**
- Hierarchy → TooltipCanvas → Canvas → Render Mode
- Hierarchy → TooltipCanvas → TooltipPanel（应该存在）

#### ✅ 步骤 7：检查相机设置

1. 选中 Main Camera
2. 检查 `Projection` 是否为 `Orthographic`（正交）
3. 检查相机是否启用（Inspector 中 Camera 组件勾选）

**检查方法：**
- Inspector → Camera → Projection → Orthographic

## 常见问题解决方案

### 问题 A：Console 显示 "未找到相机"

**解决方案：**
- 确保场景中有 Main Camera
- 或者给相机添加 Tag "MainCamera"

### 问题 B：Console 显示 "检测到Collider: 无"

**解决方案：**
- 人物没有 Collider2D 组件 → 添加 Collider2D
- Collider2D 太小 → 调整 Size
- Collider2D 位置不对 → 检查 Transform 位置
- 人物在错误的 Layer → 检查 Layer 设置

### 问题 C：Console 显示人物名称，但 "是否为目标: false"

**解决方案：**
- 人物名称不包含关键词 → 修改 `Occupant Name` 字段
- 或者在 `OccupantHoverTooltip` 的 `Target Names` 数组中添加该名称

### 问题 D：提示显示但位置不对

**解决方案：**
- 调整 `Offset X` 和 `Offset Y` 值
- 检查 Canvas 的 Render Mode

### 问题 E：提示显示但文字是空的

**解决方案：**
- 检查 `CarOccupant` 组件的 `Satiety` 和 `Disguise` 值是否设置
- 检查 Text 组件是否正确连接到 `OccupantHoverTooltip` 组件

## 快速测试方法

1. **创建测试人物：**
   - 创建 Sprite GameObject，命名为 "Father"
   - 添加 `CarOccupant` 组件
   - 设置 `Occupant Name` = "Father"
   - 添加 `BoxCollider2D` 组件
   - 调整 Collider Size 覆盖 Sprite

2. **创建 TooltipManager：**
   - 创建空物体，命名为 "TooltipManager"
   - 添加 `OccupantHoverTooltip` 组件
   - 勾选 `Enable Debug Log`

3. **运行测试：**
   - 运行游戏
   - 移动鼠标到 Father 上
   - 查看 Console 输出
   - 应该能看到提示

## 如果仍然无法工作

请提供以下信息：

1. Console 中的调试输出（启用 Enable Debug Log 后）
2. 人物物体的 Inspector 截图（显示所有组件）
3. TooltipManager 的 Inspector 截图
4. Hierarchy 窗口截图（显示场景结构）
