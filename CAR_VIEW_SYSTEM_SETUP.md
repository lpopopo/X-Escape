# 车内视角切换系统设置指南

## 📋 系统概述

这个系统实现了两种视角的切换：

1. **车内视角**：
   - 显示车内场景（人物、背景）
   - 可以给不同人分配物资
   - 可以查看人物属性
   - 显示背包UI、人物属性面板等

2. **车前窗视角**：
   - 显示车前窗外的场景
   - 可以看到前窗内的物品
   - 可以进行互动（点击物品等）
   - 显示物品交互提示UI

## 🔧 实现原理

### 核心机制：启用/禁用相机

使用**方式1（启用/禁用相机）**实现视角切换：

- **性能最好**：禁用的相机完全不参与渲染
- **逻辑清晰**：一个视角只有一个相机激活
- **适合需求**：两个视角功能完全不同，不需要同时看到

### 切换流程

```
切换视角
  ↓
启用目标相机，禁用当前相机
  ↓
显示/隐藏对应的场景对象
  ↓
显示/隐藏对应的UI
  ↓
更新按钮文本
  ↓
触发事件（通知其他系统）
```

## 🚀 快速设置

### 方法1：使用编辑器工具（推荐）

1. 在 Unity 编辑器中：
   - 菜单：`Tools` → `X-Escape` → `创建视角切换系统`
   - 会自动创建：
     - `CarViewSystem` 控制器
     - `FrontWindowCamera` 相机
     - 切换按钮UI

2. 配置系统：
   - 选中 `CarViewSystem` GameObject
   - 在 Inspector 中配置：
     - `Interior Camera`：拖拽主相机（MainCamera）
     - `Front Window Camera`：拖拽 `FrontWindowCamera`
     - `Interior Scene`：拖拽车内场景对象（包含 `CarInteriorView` 的对象）
     - `Front Window Scene`：创建或拖拽车前窗场景对象
     - `Interior UI`：拖拽包含背包UI的Canvas
     - `Front Window UI`：可选，车前窗专用的UI Canvas

3. 创建车前窗场景：
   - 创建空 GameObject，命名为 `FrontWindowScene`
   - 添加背景图（使用 `SpriteRenderer`）
   - 添加可交互的物品（使用 `FrontWindowItem` 组件）

### 方法2：手动设置

#### 步骤1：创建 CarViewSystem

1. Hierarchy 右键 → `Create Empty` → 命名为 `CarViewSystem`
2. 添加 `CarViewSystem` 组件

#### 步骤2：设置相机

1. **车内相机**（主相机）：
   - 确保场景中有 `MainCamera`
   - 设置 `Interior Camera` 字段

2. **车前窗相机**：
   - 创建新 GameObject，命名为 `FrontWindowCamera`
   - 添加 `Camera` 组件
   - 复制主相机的设置：
     ```csharp
     frontWindowCamera.CopyFrom(interiorCamera);
     ```
   - 设置 `Depth` 比主相机高（例如：主相机=0，车前窗相机=1）
   - 初始状态：`Enabled = false`
   - 设置 `Front Window Camera` 字段

#### 步骤3：设置场景对象

1. **车内场景**：
   - 找到包含 `CarInteriorView` 的对象
   - 设置 `Interior Scene` 字段

2. **车前窗场景**：
   - 创建空 GameObject，命名为 `FrontWindowScene`
   - 添加背景图：
     - 添加 `SpriteRenderer` 组件
     - 设置背景图片
     - 设置 `Sorting Order = -10`（背景层）
   - 添加可交互物品：
     - 创建子对象，添加 `FrontWindowItem` 组件
     - 设置物品信息（名称、描述、图标）
     - 添加 `Collider2D`（用于检测点击）
   - 设置 `Front Window Scene` 字段

#### 步骤4：设置UI

1. **车内UI**：
   - 找到包含背包UI的 Canvas
   - 设置 `Interior UI` 字段

2. **车前窗UI**（可选）：
   - 创建新 Canvas，命名为 `FrontWindowUI`
   - 可以添加物品交互提示UI
   - 设置 `Front Window UI` 字段

#### 步骤5：创建切换按钮

1. 在车内UI Canvas下创建按钮：
   - 右键 Canvas → `UI` → `Button`
   - 命名为 `ViewSwitchButton`
   - 设置位置（右下角）：
     - Anchor：右下角（Bottom Right）
     - Position：X=-20, Y=20
     - Size：Width=120, Height=40

2. 设置按钮文本：
   - 展开按钮 → 选中 `Text` 子对象
   - 设置文本：`车前窗`
   - 设置字体大小：16

3. 连接按钮：
   - 选中 `CarViewSystem` GameObject
   - 在 `CarViewSystem` 组件中：
     - `Switch Button`：拖拽按钮
     - `Button Text`：拖拽 `Text` 子对象

## 🎮 使用车前窗物品

### 创建可交互物品

1. 在 `FrontWindowScene` 下创建物品对象：
   ```
   FrontWindowScene
   └── Item_1 (添加 FrontWindowItem 组件)
   └── Item_2 (添加 FrontWindowItem 组件)
   ```

2. 配置 `FrontWindowItem`：
   - `Item Name`：物品名称
   - `Item Description`：物品描述
   - `Item Icon`：物品图标（可选）
   - `Can Interact`：是否可以交互
   - `Hover Color`：鼠标悬停时的颜色
   - `Hover Scale`：鼠标悬停时的缩放

3. 添加 Collider：
   - 如果对象没有 `Collider2D`，会自动添加 `BoxCollider2D`
   - 调整 Collider 大小以匹配物品

### 物品交互事件

`FrontWindowItem` 提供以下事件：

- `On Item Clicked`：物品被点击时触发
- `On Item Hover Enter`：鼠标悬停进入时触发
- `On Item Hover Exit`：鼠标悬停离开时触发

可以在 Inspector 中为这些事件添加回调函数。

### 代码示例：处理物品点击

```csharp
using UnityEngine;
using XEscape.CarScene;

public class ItemInteractionHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // 订阅视角切换事件
        CarViewSystem viewSystem = FindFirstObjectByType<CarViewSystem>();
        if (viewSystem != null)
        {
            // 使用反射或添加公共事件接口
        }
    }

    public void OnItemClicked(FrontWindowItem item)
    {
        Debug.Log($"物品被点击: {item.GetItemName()}");
        // 处理物品交互逻辑
        // 例如：拾取物品、打开详情面板等
    }
}
```

## 📝 配置检查清单

- [ ] `CarViewSystem` GameObject 已创建
- [ ] `Interior Camera` 已设置（主相机）
- [ ] `Front Window Camera` 已创建并设置
- [ ] `Interior Scene` 已设置（车内场景）
- [ ] `Front Window Scene` 已创建并设置
- [ ] `Interior UI` 已设置（包含背包UI的Canvas）
- [ ] `Front Window UI` 已设置（可选）
- [ ] 切换按钮已创建并连接
- [ ] 车前窗场景中有可交互物品（使用 `FrontWindowItem`）

## 🔍 调试

### 检查当前视角

1. 运行游戏
2. 查看 Console 输出：
   ```
   CarViewSystem: 从 Interior 切换到 FrontWindow 视角
   ```

3. 使用 `CameraViewChecker` 工具：
   - 添加 `CameraViewChecker` 组件到场景
   - 右键组件 → `检查相机状态`
   - 查看当前激活的相机

### 常见问题

1. **切换后看不到画面**：
   - 检查相机是否启用
   - 检查场景对象是否激活
   - 检查相机位置和视野设置

2. **UI不显示**：
   - 检查UI Canvas是否激活
   - 检查Canvas的 `Render Mode` 设置
   - 检查UI元素的 `Active` 状态

3. **物品无法点击**：
   - 检查 `FrontWindowItem` 的 `Can Interact` 是否为 true
   - 检查是否有 `Collider2D` 组件
   - 检查物品是否在车前窗场景中

## 🎯 总结

这个系统通过**启用/禁用相机**的方式实现视角切换，性能最好，逻辑清晰。

**关键点**：
- 两个相机：车内相机和车前窗相机
- 切换时启用一个，禁用另一个
- 同时切换场景对象和UI
- 提供事件系统供其他系统响应视角切换

**适用场景**：
- ✅ 两个视角功能完全不同
- ✅ 不需要同时看到两个视角
- ✅ 需要最佳性能
