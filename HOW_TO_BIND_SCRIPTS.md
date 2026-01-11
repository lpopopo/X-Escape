# Unity脚本绑定完全指南

## 方法一：通过Inspector面板绑定（最常用）

### 步骤1：创建GameObject
1. 打开Unity编辑器
2. 在 **Hierarchy** 窗口中右键点击
3. 选择 **Create Empty**（创建空对象）
4. 将对象重命名（比如 "GameManager"）

### 步骤2：添加脚本组件
有3种方式：

#### 方式A：拖拽法（推荐）
1. 在 **Project** 窗口找到脚本文件
   - 例如：`Assets/Scripts/Managers/GameManager.cs`
2. **直接拖拽**脚本文件到 Hierarchy 中的GameObject上
3. 或者拖拽到Inspector窗口的空白处

#### 方式B：Add Component按钮
1. 在 **Hierarchy** 中选中GameObject
2. 在 **Inspector** 窗口底部点击 **Add Component** 按钮
3. 输入脚本名称（如 "GameManager"）
4. 点击搜索结果中的脚本

#### 方式C：通过Component菜单
1. 选中GameObject
2. 点击 **Component → Scripts → [你的脚本名]**

### 步骤3：验证绑定成功
- 在Inspector中应该能看到脚本组件
- 脚本名称会显示在组件标题栏
- 可以看到脚本中所有 `[SerializeField]` 的字段

---

## 方法二：直接从Project窗口创建

1. 在Project窗口找到脚本
2. 将脚本**直接拖到Hierarchy窗口的空白处**
3. Unity会自动创建一个GameObject并绑定该脚本
4. GameObject的名称会自动设置为脚本名

---

## X-Escape项目具体操作步骤

### 场景1：配置CarScene

#### 1. 创建GameManager
```
步骤：
1. Hierarchy → 右键 → Create Empty
2. 重命名为 "GameManager"
3. 拖拽以下脚本到GameManager对象：
   - Assets/Scripts/Managers/GameManager.cs
   - Assets/Scripts/Managers/ResourceManager.cs
   - Assets/Scripts/Managers/SceneTransitionManager.cs
```

**关键配置**：
- 在GameManager的Inspector中：
  - **Resource Manager** 字段：拖入同一对象上的ResourceManager组件
  - **Scene Transition Manager** 字段：拖入同一对象上的SceneTransitionManager组件

#### 2. 创建MirrorController
```
步骤：
1. Create Empty → 命名为 "MirrorController"
2. 拖拽 Assets/Scripts/CarScene/MirrorController.cs
3. 配置Inspector中的字段：
   - Main Camera: 拖入Hierarchy中的Main Camera
   - Mirror Camera: (暂时留空，或创建第二个相机)
   - Car Occupants: 点击"+"添加车内人物位置
```

### 场景2：配置EscapeScene

#### 1. 创建MapManager
```
步骤：
1. Create Empty → 命名为 "MapManager"
2. 拖拽 Assets/Scripts/EscapeScene/MapManager.cs
3. 创建一个空对象命名为 "NodesParent"
4. 在MapManager的Inspector中：
   - Node Prefab: 拖入节点预制体（需要先创建）
   - Node Parent: 拖入NodesParent对象
```

#### 2. 创建TownManager
```
步骤：
1. Create Empty → 命名为 "TownManager"
2. 拖拽 Assets/Scripts/EscapeScene/TownManager.cs
```

---

## 配置Inspector中的引用字段

很多脚本有 `[SerializeField]` 字段需要配置：

### 示例：MirrorController脚本

```csharp
[SerializeField] private Camera mainCamera;           // 需要拖入相机
[SerializeField] private Camera mirrorCamera;         // 需要拖入另一个相机
[SerializeField] private Transform[] carOccupants;    // 需要添加数组元素
```

**配置方法**：
1. 选中有该脚本的GameObject
2. 在Inspector中找到对应字段
3. **拖拽引用**：
   - 从Hierarchy拖GameObject到字段框
   - 从Project拖资源到字段框
4. **数组字段**：
   - 修改Size数值（如 Size = 3）
   - 点击每个Element的小圆圈选择对象
   - 或直接拖拽对象到Element槽位

---

## 常见问题解答

### Q1: 脚本拖不上去？
**可能原因**：
- ✅ 脚本有编译错误（检查Console窗口）
- ✅ 脚本文件名和类名不一致
- ✅ 脚本没有继承MonoBehaviour

**解决方法**：
```csharp
// 确保类名和文件名一致
// 文件名：GameManager.cs
public class GameManager : MonoBehaviour  // ✓ 正确
{
}
```

### Q2: 如何配置单例Manager？
对于 `GameManager`、`MapManager` 等单例：
1. **只创建一个实例**
2. 设置DontDestroyOnLoad（代码已实现）
3. 其他脚本通过 `GameManager.Instance` 访问

### Q3: 如何引用其他GameObject上的组件？
```
方法1：通过Inspector拖拽
- 拖Hierarchy中的对象到Inspector字段

方法2：通过代码查找（不推荐）
- FindObjectOfType<ComponentType>()
- GameObject.Find("名称")
```

### Q4: 字段在Inspector中不显示？
确保字段声明正确：
```csharp
[SerializeField] private Camera myCamera;  // ✓ 会显示
private Camera myCamera;                    // ✗ 不显示
public Camera myCamera;                     // ✓ 会显示（但不推荐）
```

---

## 快速测试脚本是否绑定成功

### 方法1：使用Debug.Log
在脚本的 `Start()` 方法中添加：
```csharp
void Start()
{
    Debug.Log($"{gameObject.name} 已启动！");
}
```
运行游戏，在Console窗口应该能看到输出。

### 方法2：检查Inspector
- 选中GameObject
- Inspector中应该显示脚本组件
- 没有红色的"Missing Script"警告

### 方法3：查看脚本图标
- 绑定成功的脚本在Inspector中有C#文件图标
- 显示"Missing"说明脚本丢失或有错误

---

## 推荐工作流程

### 配置一个完整的GameObject：

1. **创建对象**
   ```
   Hierarchy → Create Empty → 重命名
   ```

2. **添加脚本**
   ```
   从Project拖拽脚本到GameObject
   ```

3. **配置字段**
   ```
   在Inspector中拖拽引用对象
   ```

4. **测试运行**
   ```
   点击Play按钮，检查Console输出
   ```

5. **保存场景**
   ```
   File → Save Scene 或 Ctrl+S
   ```

---

## 当前项目的脚本清单

### 需要绑定的核心脚本：

#### Managers（管理器）
- ✅ `GameManager.cs` → 绑定到"GameManager"对象
- ✅ `ResourceManager.cs` → 绑定到"GameManager"对象（同一个对象）
- ✅ `SceneTransitionManager.cs` → 绑定到"GameManager"对象

#### CarScene（车内场景）
- ✅ `MirrorController.cs` → 绑定到"MirrorController"对象
- ✅ `CarOccupant.cs` → 绑定到每个车内人物对象

#### EscapeScene（逃亡场景）
- ✅ `MapManager.cs` → 绑定到"MapManager"对象
- ✅ `MapNode.cs` → 绑定到节点预制体
- ✅ `TownManager.cs` → 绑定到"TownManager"对象

#### UI
- ✅ `GameOverUI.cs` → 绑定到UI Canvas下的对象
- ✅ `ResourceUI.cs` → 绑定到UI Canvas下的对象

#### Utilities
- ✅ `ClickableObject.cs` → 作为基类，继承后使用

---

## 实战练习：配置GameManager

### 完整步骤演示：

1. **打开CarScene场景**
   - Project窗口 → Assets/Scenes/CarScene.unity
   - 双击打开

2. **创建GameObject**
   - Hierarchy → 右键 → Create Empty
   - 重命名为 "GameManager"

3. **添加第一个脚本**
   - Project窗口 → Assets/Scripts/Managers/GameManager.cs
   - 拖到Hierarchy中的GameManager对象上
   - 此时Inspector应该显示GameManager组件

4. **添加其他Manager脚本**
   - 重复步骤3，添加：
     - ResourceManager.cs
     - SceneTransitionManager.cs

5. **配置引用关系**
   - 点击GameManager对象
   - 在Inspector中找到GameManager组件
   - 看到两个字段：
     - Resource Manager: 拖入同对象上的ResourceManager组件
     - Scene Transition Manager: 拖入同对象上的SceneTransitionManager组件

6. **测试**
   - 点击Play按钮
   - 检查Console是否有错误

---

## 小技巧

### 技巧1：批量选择
- 按住Ctrl（Windows）或Cmd（Mac）多选GameObject
- 可以批量添加同一个脚本

### 技巧2：复制组件
- 在Inspector中右键组件标题 → Copy Component
- 选择另一个对象 → 右键 → Paste Component As New

### 技巧3：查找引用
- 选中脚本文件 → 右键 → Find References In Scene
- 可以看到场景中哪些对象使用了该脚本

### 技巧4：重置组件
- 右键组件标题 → Reset
- 恢复到默认值

---

## 下一步

配置完脚本绑定后：
1. 创建UI元素并绑定UI脚本
2. 创建节点预制体并配置MapNode脚本
3. 导入美术资源（Sprite、音效等）
4. 测试游戏逻辑

需要我帮你创建预制体模板或者配置UI吗？
