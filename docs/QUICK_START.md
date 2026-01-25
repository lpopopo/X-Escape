# X-Escape 快速开始指南

## 前置要求

- Unity 2020.3 或更高版本（推荐 Unity 2021.3 LTS）
- 2D 项目模板

## 步骤 1: 在Unity中打开项目

1. 打开 Unity Hub
2. 点击 "打开" 或 "Add"
3. 选择 `X-Escape` 文件夹
4. 等待 Unity 导入资源

## 步骤 2: 创建场景

### 创建车内场景 (CarScene)

1. 在 `Assets/Scenes/` 文件夹中右键 → Create → Scene
2. 命名为 `CarScene`
3. 保存场景

### 创建逃亡场景 (EscapeScene)

1. 同样方式创建 `EscapeScene`
2. 保存场景

## 步骤 3: 设置GameManager

### 在CarScene中设置

1. 在 Hierarchy 中右键 → Create Empty，命名为 "GameManager"
2. 选中 GameManager，在 Inspector 中添加以下组件：
   - `GameManager` (Script)
   - `ResourceManager` (Script)
   - `SceneTransitionManager` (Script)
3. 在 GameManager 组件中：
   - 将 ResourceManager 和 SceneTransitionManager 拖拽到对应字段
   - 设置场景名称：CarScene 和 EscapeScene

### 在EscapeScene中设置

1. 同样创建 GameManager GameObject
2. 添加相同的组件
3. 确保场景名称设置正确

## 步骤 4: 设置车内场景

### 创建后视镜

1. 创建 Sprite GameObject，命名为 "Mirror"
2. 添加 `MirrorController` 组件
3. 设置后视镜的点击区域（可以添加 Collider2D）
4. 在 MirrorController 中：
   - 设置 Main Camera（主相机）
   - 设置 Mirror Camera（后视镜相机，可选）
   - 设置车内人物数组（Car Occupants）

### 创建车内人物

1. 创建多个 Sprite GameObject，代表车内不同的人
2. 每个添加 `CarOccupant` 组件
3. 设置人物名称和头像
4. 将这些人物拖拽到 MirrorController 的 Car Occupants 数组中

### 设置相机

1. 确保场景中有 Main Camera
2. 如果需要后视镜视角，创建第二个 Camera，命名为 "MirrorCamera"
3. 在 MirrorController 中设置这两个相机

## 步骤 5: 设置逃亡场景

### 创建地图管理器

1. 创建空 GameObject，命名为 "MapManager"
2. 添加 `MapManager` 组件
3. 设置地图参数：
   - Map Width: 10
   - Map Height: 10
   - Total Nodes: 20
   - Visible Node Range: 2

### 创建地图节点预制体

1. 创建 Sprite GameObject，命名为 "MapNode"
2. 添加以下组件：
   - `MapNode` (Script)
   - `SpriteRenderer` (用于显示)
   - `Collider2D` (用于点击检测，使用 BoxCollider2D 或 CircleCollider2D)
3. 将 MapNode 拖拽到 `Assets/Prefabs/` 文件夹
4. 在 MapManager 中设置 Node Prefab 为刚创建的预制体
5. 创建空 GameObject 作为 Node Parent，拖拽到 MapManager 的 Node Parent 字段

### 创建城镇管理器

1. 创建空 GameObject，命名为 "TownManager"
2. 添加 `TownManager` 组件
3. 设置搜寻时间和资源获取范围

## 步骤 6: 设置UI

### 创建Canvas

1. 右键 Hierarchy → UI → Canvas
2. 这会自动创建 Canvas、EventSystem 和 GraphicRaycaster

### 创建资源显示UI

1. 在 Canvas 下创建空 GameObject，命名为 "ResourceUI"
2. 添加 `ResourceUI` 组件
3. 创建体力显示：
   - 创建 Slider，命名为 "StaminaSlider"
   - 创建 Text，命名为 "StaminaText"
   - 在 ResourceUI 组件中设置这些引用
4. 创建油量显示：
   - 同样方式创建 FuelSlider 和 FuelText
   - 在 ResourceUI 组件中设置引用

### 创建游戏结束UI

1. 在 Canvas 下创建 Panel，命名为 "GameOverPanel"
2. 添加 `GameOverUI` 组件
3. 在 GameOverPanel 下创建：
   - Text 显示 "游戏失败"
   - Button "重新开始"
   - Button "退出"
4. 在 GameOverUI 组件中设置这些引用
5. 创建 VictoryPanel，同样设置

### 创建城镇菜单UI

1. 在 Canvas 下创建 Panel，命名为 "TownMenu"
2. 添加 Button "搜寻物资"
3. 在 TownManager 中设置 Town Menu UI 引用

## 步骤 7: 配置资源管理器

在 GameManager 的 ResourceManager 组件中设置：
- Max Stamina: 100
- Current Stamina: 100
- Max Fuel: 100
- Current Fuel: 100
- Stamina Consumption Rate: 1（每秒消耗）
- Fuel Consumption Rate: 2（每秒消耗）

## 步骤 8: 测试游戏

1. 打开 CarScene
2. 点击 Play 按钮
3. 测试后视镜点击功能
4. 切换到 EscapeScene 测试地图系统

## 常见问题

### Q: 地图节点不显示？
A: 确保 MapNode 预制体有 SpriteRenderer 组件，并且设置了 Sprite。

### Q: 点击后视镜没有反应？
A: 确保后视镜 GameObject 有 Collider2D 组件，并且相机设置正确。

### Q: 资源UI不更新？
A: 确保 ResourceUI 组件正确引用了 Slider 和 Text，并且 GameManager 已正确设置。

### Q: 场景切换不工作？
A: 确保 SceneTransitionManager 中的场景名称与实际的场景文件名一致（不包括 .unity 扩展名）。

## 下一步

完成基础设置后，可以：
1. 添加美术资源（Sprites、UI图片）
2. 调整游戏参数平衡性
3. 添加音效和背景音乐
4. 完善UI设计
5. 添加更多游戏机制

