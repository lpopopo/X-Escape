# 背包系统设置指南

## 系统概述

背包系统包含以下功能：
- **物品管理**：存储食物、伪装物品和其他物品
- **物品堆叠**：相同物品可以堆叠（食物可堆叠，伪装物品不可堆叠）
- **物品使用**：使用食物恢复饱腹度，使用伪装物品增加伪装度
- **UI显示**：显示背包内容，支持点击使用物品

## 已创建的脚本

### 核心脚本（已完成）
1. `ItemType.cs` - 物品类型枚举
2. `Item.cs` - 物品基类
3. `FoodItem.cs` - 食物物品类
4. `DisguiseItem.cs` - 伪装物品类
5. `InventoryManager.cs` - 背包管理器（单例）
6. `ItemDatabase.cs` - 物品数据库（ScriptableObject）

### UI脚本（已完成）
1. `InventoryUI.cs` - 背包UI控制器
2. `InventorySlot.cs` - 背包槽位UI

## Unity编辑器设置步骤

### 步骤 1：创建 InventoryManager

1. **在场景中创建空物体**
   - Hierarchy 右键 → `Create Empty`
   - 命名为 `InventoryManager`

2. **添加 InventoryManager 脚本**
   - 选中 `InventoryManager`
   - Inspector → `Add Component`
   - 搜索 `InventoryManager`
   - 添加组件

3. **配置设置**
   - `Max Slots`：最大背包槽位（默认 20）
   - `Enable Debug Log`：启用调试日志（可选）

### 步骤 2：创建物品数据库（可选，用于预设物品）

1. **创建 ScriptableObject**
   - Project 窗口右键 → `Create` → `X-Escape` → `Item Database`
   - 命名为 `ItemDatabase`

2. **配置预设物品**
   - 选中 `ItemDatabase`
   - 在 Inspector 中：
     - `Food Items`：添加预设食物
     - `Disguise Items`：添加预设伪装物品

### 步骤 3：创建背包UI（需要在Unity编辑器中手动创建）

#### 3.1 创建 Canvas

1. Hierarchy 右键 → `UI` → `Canvas`
2. 命名为 `InventoryCanvas`
3. 设置 `Render Mode` 为 `Screen Space - Overlay`

#### 3.2 创建背包面板

1. 在 `InventoryCanvas` 下右键 → `UI` → `Panel`
2. 命名为 `InventoryPanel`
3. 设置大小和位置（建议居中）
4. 设置背景颜色（半透明黑色）

#### 3.3 创建槽位容器

1. 在 `InventoryPanel` 下右键 → `UI` → `Panel`
2. 命名为 `SlotsContainer`
3. 添加 `Grid Layout Group` 组件：
   - `Cell Size`: X=80, Y=80
   - `Spacing`: X=10, Y=10
   - `Start Corner`: Upper Left
   - `Start Axis`: Horizontal
   - `Constraint`: Fixed Column Count
   - `Constraint Count`: 5

#### 3.4 创建槽位预制体

1. 在 `SlotsContainer` 下右键 → `UI` → `Button`
2. 命名为 `SlotPrefab`
3. 删除 `Button` 组件，添加 `Image` 组件（作为背景）
4. 在 `SlotPrefab` 下创建子对象：
   - `Icon`（Image）：显示物品图标
     - 设置 `Image Type` 为 `Simple`
     - 设置颜色为白色
   - `Quantity`（Text）：显示数量
     - 设置字体大小：14
     - 设置颜色：白色
     - 设置对齐：右下角
5. 添加 `InventorySlot` 脚本组件
6. 将 `SlotPrefab` 拖到 `Assets/Prefabs/` 文件夹创建预制体
7. 删除 Hierarchy 中的 `SlotPrefab`（保留预制体）

#### 3.5 创建物品信息显示区域

在 `InventoryPanel` 下创建：

1. **物品名称文本**
   - 右键 → `UI` → `Text`
   - 命名为 `ItemNameText`
   - 设置字体大小：18，加粗

2. **物品描述文本**
   - 右键 → `UI` → `Text`
   - 命名为 `ItemDescriptionText`
   - 设置字体大小：14

3. **使用按钮**
   - 右键 → `UI` → `Button`
   - 命名为 `UseButton`
   - 设置文本：`使用`

4. **关闭按钮**
   - 右键 → `UI` → `Button`
   - 命名为 `CloseButton`
   - 设置文本：`关闭`

#### 3.6 配置 InventoryUI 脚本

1. **创建空物体**
   - Hierarchy 右键 → `Create Empty`
   - 命名为 `InventoryUIController`

2. **添加 InventoryUI 脚本**
   - 选中 `InventoryUIController`
   - Inspector → `Add Component`
   - 搜索 `InventoryUI`
   - 添加组件

3. **配置引用**
   - `Inventory Panel`：拖拽 `InventoryPanel`
   - `Slot Prefab`：拖拽 `SlotPrefab` 预制体
   - `Slots Parent`：拖拽 `SlotsContainer`
   - `Item Name Text`：拖拽 `ItemNameText`
   - `Item Description Text`：拖拽 `ItemDescriptionText`
   - `Use Button`：拖拽 `UseButton`
   - `Close Button`：拖拽 `CloseButton`

4. **配置 SlotPrefab**
   - 选中 `SlotPrefab` 预制体
   - 在 `InventorySlot` 组件中：
     - `Icon Image`：拖拽 `Icon` 子对象
     - `Quantity Text`：拖拽 `Quantity` 子对象
     - `Background Image`：拖拽 `SlotPrefab` 自身

### 步骤 4：测试背包系统

#### 4.1 创建测试物品

在代码中创建测试物品：

```csharp
// 在某个脚本的 Start 方法中
FoodItem bread = new FoodItem
{
    itemName = "面包",
    description = "恢复20点饱腹度",
    foodType = FoodType.Bread,
    satietyRestore = 20f,
    quantity = 5
};

DisguiseItem hat = new DisguiseItem
{
    itemName = "帽子",
    description = "增加15点伪装度",
    disguiseType = DisguiseType.Hat,
    disguiseBonus = 15f,
    quantity = 1
};

InventoryManager.Instance.AddItem(bread);
InventoryManager.Instance.AddItem(hat);
```

#### 4.2 测试功能

1. 运行游戏
2. 按 `I` 键打开背包
3. 点击物品查看信息
4. 点击"使用"按钮使用物品
5. 检查人物状态是否更新

## 快速设置（简化版）

如果不想手动创建UI，可以使用代码动态创建：

### 创建简化的背包UI脚本

我已经在 `InventoryUI.cs` 中添加了自动创建功能，但需要你手动设置：

1. 创建 `InventoryUIController` GameObject
2. 添加 `InventoryUI` 组件
3. 至少设置 `Inventory Panel`（其他可以为空，脚本会尝试自动创建）

## 物品使用逻辑

### 食物使用
- 恢复所有车内人物的饱腹度
- 可选：恢复体力（如果设置了 `staminaRestore`）

### 伪装物品使用
- 增加所有车内人物的伪装度

## API 使用示例

### 添加物品
```csharp
FoodItem food = new FoodItem
{
    itemName = "罐头",
    description = "恢复30点饱腹度",
    satietyRestore = 30f,
    quantity = 3
};
InventoryManager.Instance.AddItem(food);
```

### 使用物品
```csharp
// 通过UI点击使用，或代码调用：
InventoryManager.Instance.UseItem(itemId);
```

### 获取物品列表
```csharp
List<Item> allItems = InventoryManager.Instance.GetAllItems();
List<Item> foodItems = InventoryManager.Instance.GetItemsByType(ItemType.Food);
```

## 注意事项

1. **InventoryManager 是单例**：确保场景中只有一个实例
2. **DontDestroyOnLoad**：InventoryManager 会在场景切换时保持存在
3. **物品ID**：每个物品都有唯一的ID，用于识别和操作
4. **堆叠限制**：食物可以堆叠（默认10个），伪装物品不能堆叠

## 下一步

1. 在Unity中创建UI（按照步骤3）
2. 创建物品图标（Sprite）
3. 在游戏中添加获取物品的逻辑（如搜索城镇时获得物品）
4. 完善UI样式和动画
