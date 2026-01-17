# 鼠标悬停显示人物状态使用指南

## 功能说明

当鼠标移动到"Father"（爸爸）或"Mother"（妈妈）人物上时，会显示他们的状态信息：
- **饱腹度**：饱腹、正常、饥饿、饥荒
- **伪装度**：判若两人、难以察觉、可疑、一模一样

## 设置步骤

### 步骤 1：确保人物有 CarOccupant 组件

1. 在 Hierarchy 中选中"Father"或"Mother"物体
2. 在 Inspector 中添加 `CarOccupant` 组件（如果还没有）
3. 设置人物名称（Name）：
   - Father 或 爸爸
   - Mother 或 妈妈
4. 在 `CarOccupant` 组件中设置：
   - **Satiety**（饱腹度）：0-100，数值越高越饱
   - **Disguise**（伪装度）：0-100，数值越高伪装越好

### 步骤 2：添加 Collider2D 组件

1. 选中人物物体
2. 在 Inspector 中添加 `Collider2D` 组件（BoxCollider2D 或 CircleCollider2D）
3. 调整 Collider 大小，使其覆盖人物图像区域

### 步骤 3：设置 UI 提示系统

#### 方法 A：自动创建（推荐）

1. 在场景中创建一个空物体，命名为 "TooltipManager"
2. 添加 `OccupantHoverTooltip` 组件
3. 脚本会自动创建 Canvas 和 UI 元素

#### 方法 B：手动设置 UI

1. 创建 Canvas：
   - Hierarchy 右键 → UI → Canvas
   - 命名为 "TooltipCanvas"

2. 创建提示面板：
   - 在 Canvas 下创建空物体，命名为 "TooltipPanel"
   - 添加 `RectTransform` 和 `Image` 组件
   - 设置 Image 颜色为半透明黑色（如：RGBA 0,0,0,200）

3. 创建文本组件：
   - 在 TooltipPanel 下创建三个 Text 对象：
     - NameText（显示人物名称）
     - SatietyText（显示饱腹度）
     - DisguiseText（显示伪装度）

4. 在 TooltipManager 的 `OccupantHoverTooltip` 组件中：
   - 将 TooltipPanel 拖到 `Tooltip Panel` 字段
   - 将三个 Text 组件分别拖到对应字段

## 状态说明

### 饱腹度状态

| 状态 | 条件 | 说明 |
|------|------|------|
| 饱腹 | Satiety ≥ 75 | 人物很饱，状态良好 |
| 正常 | 50 ≤ Satiety < 75 | 人物正常，状态稳定 |
| 饥饿 | 25 ≤ Satiety < 50 | 人物饥饿，需要食物 |
| 饥荒 | Satiety < 25 | 人物极度饥饿，危险状态 |

### 伪装度状态

| 状态 | 条件 | 说明 |
|------|------|------|
| 一模一样 | Disguise ≥ 75 | 伪装完美，难以识别 |
| 难以察觉 | 50 ≤ Disguise < 75 | 伪装良好，不易被发现 |
| 可疑 | 25 ≤ Disguise < 50 | 伪装一般，可能引起怀疑 |
| 判若两人 | Disguise < 25 | 伪装很差，容易被认出 |

## 自定义设置

### 修改检测的人物名称

在 `OccupantHoverTooltip` 组件的 `Target Names` 数组中添加或修改关键词：
- 默认：`["father", "mother", "爸爸", "妈妈"]`
- 可以添加其他名称，如：`["dad", "mom", "父亲", "母亲"]`

### 调整阈值

在 `CarOccupant` 组件中可以调整状态阈值：
- **Satiety Full Threshold**：饱腹阈值（默认 75）
- **Satiety Normal Threshold**：正常阈值（默认 50）
- **Satiety Hungry Threshold**：饥饿阈值（默认 25）
- **Disguise Identical Threshold**：一模一样阈值（默认 75）
- **Disguise Hard To Detect Threshold**：难以察觉阈值（默认 50）
- **Disguise Suspicious Threshold**：可疑阈值（默认 25）

### 调整提示位置

在 `OccupantHoverTooltip` 组件中：
- **Offset X**：水平偏移（默认 10）
- **Offset Y**：垂直偏移（默认 10）

## 代码示例

### 通过代码修改状态

```csharp
using XEscape.CarScene;

// 获取人物组件
CarOccupant occupant = GetComponent<CarOccupant>();

// 设置饱腹度
occupant.SetSatiety(80f); // 设置为80，会显示"饱腹"

// 设置伪装度
occupant.SetDisguise(60f); // 设置为60，会显示"难以察觉"

// 获取状态文本
string satietyStatus = occupant.GetSatietyStatusText(); // "饱腹"
string disguiseStatus = occupant.GetDisguiseStatusText(); // "难以察觉"
```

## 故障排除

### 问题：鼠标悬停没有显示提示

**解决方案：**
1. 检查人物是否有 `CarOccupant` 组件
2. 检查人物名称是否包含关键词（father/mother/爸爸/妈妈）
3. 检查人物是否有 `Collider2D` 组件
4. 检查 Collider2D 是否覆盖人物图像
5. 检查 `OccupantHoverTooltip` 组件是否添加到场景中

### 问题：提示显示在错误位置

**解决方案：**
1. 检查 Canvas 的 Render Mode 是否为 `Screen Space - Overlay`
2. 调整 `Offset X` 和 `Offset Y` 值

### 问题：状态文本显示不正确

**解决方案：**
1. 检查 `CarOccupant` 组件中的 Satiety 和 Disguise 值
2. 检查阈值设置是否正确
3. 确保值在 0-100 范围内

## 注意事项

1. **Collider2D 必需**：人物必须有 Collider2D 组件才能检测鼠标悬停
2. **名称匹配**：人物名称必须包含设置的关键词（不区分大小写）
3. **Canvas 层级**：确保 Tooltip Canvas 在最上层，不会被其他UI遮挡
4. **性能**：如果场景中有很多人物，建议只给需要显示提示的人物添加 Collider2D
