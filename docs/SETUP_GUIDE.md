# X-Escape 项目启动指南

## 项目状态
✅ 代码完整
✅ 基础场景已创建
⚠️ 需要在Unity编辑器中进行场景配置

---

## 快速启动步骤

### 1. 打开项目
1. 启动 **Unity Hub**
2. 点击 **"打开"** 按钮
3. 选择项目根目录：`/Users/wangcheng/games/X-Escape`
4. 等待Unity导入资源（首次打开需要几分钟）

### 2. 打开场景
- 在 **Project** 窗口中，导航到 `Assets/Scenes/`
- 双击 **CarScene.unity** 或 **EscapeScene.unity** 打开场景

### 3. 当前项目可运行性
**状态：部分可运行**

#### ✅ 已完成的部分
- 所有C#脚本（游戏逻辑完整）
- 基础场景文件（CarScene、EscapeScene）
- 项目目录结构

#### ⚠️ 需要配置的部分（在Unity编辑器中）
要让项目完整运行，需要在Unity中添加以下内容：

---

## 必需的Unity场景配置

### CarScene 配置清单

1. **创建GameManager对象**
   - 在 Hierarchy 中右键 → Create Empty
   - 命名为 "GameManager"
   - 添加脚本：`GameManager`、`ResourceManager`、`SceneTransitionManager`

2. **创建后视镜控制器**
   - 创建空对象命名为 "MirrorController"
   - 添加 `MirrorController.cs` 脚本
   - 在Inspector中配置：
     - Main Camera: 拖入主摄像机
     - Mirror Camera: （可选）创建第二个摄像机用于后视镜视角
     - Car Occupants: 创建几个空对象代表车内人物位置

3. **添加UI元素**
   - 创建 Canvas（GameObject → UI → Canvas）
   - 添加 ResourceUI 显示体力和油量条
   - 添加 GameOverUI 面板

### EscapeScene 配置清单

1. **GameManager**（跨场景持久化对象）
   - 如果是从CarScene切换过来，GameManager会自动保留
   - 否则需要重新创建

2. **MapManager对象**
   - Create Empty → 命名为 "MapManager"
   - 添加 `MapManager.cs` 脚本
   - 配置：
     - **Node Prefab**: 需要创建节点预制体（见下方）
     - **Node Parent**: 创建空对象用于存放所有节点

3. **TownManager对象**
   - Create Empty → 命名为 "TownManager"
   - 添加 `TownManager.cs` 脚本

4. **创建节点预制体（重要！）**
   - 在 Hierarchy 中创建空对象
   - 添加组件：
     - `SpriteRenderer`（用于显示节点图形）
     - `CircleCollider2D`（用于点击检测）
     - `MapNode.cs` 脚本
   - 拖到 `Assets/Prefabs/` 文件夹创建预制体
   - 在MapManager的Inspector中引用这个预制体

---

## 最小化运行步骤（快速测试）

如果你想快速看到项目运行效果，可以：

### 方案A：使用EscapeScene测试地图系统
```
1. 打开 EscapeScene.unity
2. 创建 GameObject → 空对象 → 命名为 "MapManager"
3. 添加 MapManager.cs 脚本
4. 创建简单的节点预制体：
   - 创建 GameObject → 2D Object → Sprite → Circle
   - 添加 CircleCollider2D
   - 添加 MapNode.cs
   - 拖入Prefabs文件夹
5. 在MapManager中引用节点预制体
6. 点击 Play 按钮
```

### 方案B：创建测试脚本
创建一个简单的测试脚本来验证游戏逻辑：

```csharp
// Assets/Scripts/TestRunner.cs
using UnityEngine;
using XEscape.Managers;

public class TestRunner : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== X-Escape 测试开始 ===");

        // 测试ResourceManager
        var rm = FindObjectOfType<ResourceManager>();
        if (rm != null)
        {
            Debug.Log($"当前体力: {rm.GetStamina()}");
            Debug.Log($"当前油量: {rm.GetFuel()}");
        }
        else
        {
            Debug.LogWarning("ResourceManager未找到！");
        }
    }
}
```

---

## 项目架构说明

### 核心系统
- **GameManager**: 游戏总控制器（单例，跨场景）
- **ResourceManager**: 资源系统（体力、油量）
- **SceneTransitionManager**: 场景切换

### 场景系统
- **CarScene**: 车内观察场景
  - MirrorController: 后视镜交互
  - CarOccupant: 车内人物

- **EscapeScene**: 逃亡地图场景
  - MapManager: 地图生成和管理
  - MapNode: 地图节点
  - TownManager: 城镇交互

### UI系统
- **ResourceUI**: 显示资源条
- **GameOverUI**: 游戏结束界面

---

## 常见问题

### Q: 为什么点击Play后场景是空的？
A: 需要在场景中手动添加GameObject并挂载脚本。场景文件只有摄像机，其他对象需要你创建。

### Q: 脚本报错"找不到某个组件"？
A: 检查Inspector中的脚本引用是否正确配置。很多脚本需要通过Inspector拖入其他对象的引用。

### Q: MapManager生成不了节点？
A: 确保：
1. 创建了节点预制体
2. 预制体包含 SpriteRenderer（用于显示）
3. 预制体包含 Collider2D（用于点击）
4. 在MapManager的Inspector中引用了预制体

### Q: 游戏逻辑不工作？
A: 检查：
1. GameManager是否存在（Hierarchy中查找）
2. 所有Manager对象的脚本是否正确挂载
3. Console窗口是否有报错信息

---

## 下一步开发建议

### 立即需要的内容
1. **美术资源**
   - 车内场景背景图
   - 后视镜图标
   - 地图节点图标（城镇、道路、边境、危险）
   - UI元素（按钮、血条、油量条）

2. **预制体创建**
   - 地图节点预制体（4种类型）
   - UI预制体

3. **场景布局**
   - 在Unity编辑器中摆放UI元素
   - 配置Canvas和UI层级

### 功能扩展方向
1. 添加音效和音乐
2. 添加更多随机事件
3. 实现存档系统
4. 添加更多车内互动元素
5. 增加剧情对话系统

---

## 学习资源

如果你是Unity新手，建议学习：
1. **Unity基础操作**
   - Hierarchy、Inspector、Project窗口的使用
   - 如何创建GameObject并添加组件
   - 如何创建和使用预制体

2. **Unity脚本**
   - MonoBehaviour生命周期
   - 如何在Inspector中配置引用
   - 使用Debug.Log调试

3. **Unity UI系统**
   - Canvas和UI元素
   - Button点击事件
   - Text/TextMeshPro文本显示

---

## 技术支持

项目代码位置：
- **脚本**: `Assets/Scripts/`
- **场景**: `Assets/Scenes/`
- **预制体**: `Assets/Prefabs/`（需要手动创建）

代码已经写好，主要工作是在Unity编辑器中进行可视化配置和美术资源导入。
