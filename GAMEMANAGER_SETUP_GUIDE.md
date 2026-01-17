# GameManager 设置指南

## GameManager 应该绑定的脚本

GameManager GameObject 需要绑定 **3个脚本组件**：

### 1. GameManager（主脚本）
- **文件位置**：`Assets/Scripts/Managers/GameManager.cs`
- **命名空间**：`XEscape.Managers`
- **作用**：游戏主管理器，管理游戏状态和流程

### 2. ResourceManager（资源管理器）
- **文件位置**：`Assets/Scripts/Managers/ResourceManager.cs`
- **命名空间**：`XEscape.Managers`
- **作用**：管理体力、油量等资源

### 3. SceneTransitionManager（场景切换管理器）
- **文件位置**：`Assets/Scripts/Managers/SceneTransitionManager.cs`
- **命名空间**：`XEscape.Managers`
- **作用**：管理场景之间的切换

## 在 Unity 编辑器中设置步骤

### 步骤 1：找到 GameManager GameObject

1. 打开 `CarScene.unity` 场景
2. 在 Hierarchy 窗口中找到 `GameManager` GameObject
3. 选中它

### 步骤 2：删除 Missing Script（如果有）

1. 在 Inspector 中，如果看到灰色的 "Missing (Script)" 组件
2. 点击该组件右侧的齿轮图标 → `Remove Component`
3. 或者直接点击组件右上角的 `×` 删除

### 步骤 3：添加 GameManager 脚本

1. 在 Inspector 中，点击 `Add Component` 按钮
2. 在搜索框中输入：`GameManager`
3. 选择 `GameManager`（应该显示路径：`XEscape.Managers`）
4. 点击添加

### 步骤 4：添加 ResourceManager 脚本

1. 继续在 Inspector 中，点击 `Add Component` 按钮
2. 在搜索框中输入：`ResourceManager`
3. 选择 `ResourceManager`（应该显示路径：`XEscape.Managers`）
4. 点击添加

### 步骤 5：添加 SceneTransitionManager 脚本

1. 继续在 Inspector 中，点击 `Add Component` 按钮
2. 在搜索框中输入：`SceneTransitionManager`
3. 选择 `SceneTransitionManager`（应该显示路径：`XEscape.Managers`）
4. 点击添加

### 步骤 6：配置 GameManager 组件

1. 在 Inspector 中找到 `GameManager` 组件
2. 配置以下字段：
   - **Current Game State**：设置为 `InCar`（默认值）
   - **Resource Manager**：可以留空（会自动查找），或者拖拽 `ResourceManager` 组件
   - **Scene Transition Manager**：可以留空（会自动查找），或者拖拽 `SceneTransitionManager` 组件

### 步骤 7：配置 SceneTransitionManager 组件

1. 在 Inspector 中找到 `SceneTransitionManager` 组件
2. 配置场景名称：
   - **Car Scene Name**：`CarScene`
   - **Escape Scene Name**：`EscapeScene`

### 步骤 8：保存场景

1. 按 `Ctrl+S`（Windows）或 `Cmd+S`（Mac）保存场景
2. 或者点击菜单：`File` → `Save`

## 最终效果

GameManager GameObject 的 Inspector 应该显示：

```
GameManager (GameObject)
├── Transform
├── GameManager (Script)
│   ├── Current Game State: InCar
│   ├── Resource Manager: ResourceManager (Script)
│   └── Scene Transition Manager: SceneTransitionManager (Script)
├── ResourceManager (Script)
│   ├── Max Stamina: 100
│   ├── Current Stamina: 100
│   ├── Max Fuel: 100
│   ├── Current Fuel: 100
│   └── ...
└── SceneTransitionManager (Script)
    ├── Car Scene Name: CarScene
    └── Escape Scene Name: EscapeScene
```

## 验证设置

1. 运行游戏（点击 Play 按钮）
2. 检查 Console 是否有错误
3. 如果没有错误，说明设置成功

## 常见问题

### 问题 1：找不到脚本

**解决方案**：
- 确保脚本文件存在于 `Assets/Scripts/Managers/` 文件夹中
- 等待 Unity 重新编译脚本（查看 Unity 右下角的状态）
- 如果脚本有编译错误，先修复错误

### 问题 2：脚本添加后还是显示 Missing

**解决方案**：
1. 关闭 Unity
2. 删除 `Library` 文件夹（Unity 会自动重建）
3. 重新打开 Unity
4. 等待脚本重新编译
5. 重新添加脚本

### 问题 3：脚本命名空间错误

**解决方案**：
- 确保所有脚本都在 `XEscape.Managers` 命名空间下
- 检查脚本文件开头的 `namespace XEscape.Managers` 声明

## 脚本依赖关系

```
GameManager
├── 需要 ResourceManager（会自动查找）
└── 需要 SceneTransitionManager（会自动查找）

ResourceManager
└── 依赖 GameManager（通过 GameManager.Instance 访问）

SceneTransitionManager
└── 依赖 GameManager（通过 GameManager.Instance 访问）
```

## 注意事项

1. **单例模式**：GameManager 使用单例模式，确保场景中只有一个实例
2. **DontDestroyOnLoad**：GameManager 会在场景切换时保持存在
3. **自动初始化**：如果未手动指定 ResourceManager 和 SceneTransitionManager，GameManager 会在 Awake 时自动查找同 GameObject 上的组件
