# X-Escape 项目结构说明

## 文件夹结构

```
X-Escape/
├── Assets/
│   ├── Scripts/                    # 所有C#脚本
│   │   ├── Managers/               # 管理器脚本
│   │   │   ├── GameManager.cs      # 游戏主管理器
│   │   │   ├── ResourceManager.cs  # 资源管理器
│   │   │   └── SceneTransitionManager.cs # 场景切换管理器
│   │   ├── CarScene/               # 车内场景相关脚本
│   │   │   ├── MirrorController.cs # 后视镜控制器
│   │   │   └── CarOccupant.cs      # 车内人物
│   │   ├── EscapeScene/            # 逃亡场景相关脚本
│   │   │   ├── MapManager.cs       # 地图管理器
│   │   │   ├── MapNode.cs          # 地图节点
│   │   │   └── TownManager.cs      # 城镇管理器
│   │   ├── UI/                     # UI相关脚本
│   │   │   ├── ResourceUI.cs      # 资源UI显示
│   │   │   └── GameOverUI.cs      # 游戏结束UI
│   │   └── Utilities/              # 工具类
│   │       └── ClickableObject.cs  # 可点击对象基类
│   ├── Scenes/                     # Unity场景文件
│   │   ├── CarScene.unity          # 车内场景
│   │   └── EscapeScene.unity       # 逃亡场景
│   ├── Prefabs/                    # 预制体
│   │   ├── MapNode.prefab          # 地图节点预制体
│   │   ├── CarOccupant.prefab      # 车内人物预制体
│   │   └── UI/                     # UI预制体
│   ├── Sprites/                    # 精灵图片资源
│   │   ├── Car/                    # 车内相关图片
│   │   ├── Map/                    # 地图相关图片
│   │   └── UI/                     # UI相关图片
│   ├── Materials/                  # 材质文件
│   ├── Audio/                      # 音频文件
│   │   ├── Music/                  # 背景音乐
│   │   └── SFX/                    # 音效
│   └── Fonts/                      # 字体文件
├── ProjectSettings/                # Unity项目设置（自动生成）
├── Packages/                       # Unity包管理（自动生成）
├── README.md                       # 项目说明
├── PROJECT_STRUCTURE.md           # 项目结构说明（本文件）
└── .gitignore                     # Git忽略文件
```

## 核心系统说明

### 1. GameManager（游戏管理器）
- **职责**: 管理游戏整体流程和状态
- **功能**:
  - 游戏状态管理（InCar, Escaping, GameOver, Victory）
  - 协调各个子系统
  - 检查游戏结束和胜利条件

### 2. ResourceManager（资源管理器）
- **职责**: 管理游戏资源（体力、油量）
- **功能**:
  - 资源消耗和恢复
  - 资源变化事件通知
  - 资源耗尽检测

### 3. SceneTransitionManager（场景切换管理器）
- **职责**: 管理场景之间的切换
- **功能**:
  - 加载车内场景
  - 加载逃亡场景
  - 场景切换时的状态管理

### 4. MirrorController（后视镜控制器）
- **职责**: 控制车内视角切换
- **功能**:
  - 点击后视镜切换视角
  - 查看车内所有人
  - 相机平滑移动

### 5. MapManager（地图管理器）
- **职责**: 管理地图生成和节点
- **功能**:
  - 随机生成地图
  - 管理节点可见性
  - 处理节点选择和移动
  - 节点事件处理

### 6. TownManager（城镇管理器）
- **职责**: 处理城镇中的物资搜寻
- **功能**:
  - 打开城镇菜单
  - 物资搜寻逻辑
  - 资源获取

## 使用指南

### 在Unity中设置项目

1. **创建场景**
   - 创建 `CarScene` 场景
   - 创建 `EscapeScene` 场景

2. **设置GameManager**
   - 在场景中创建空GameObject，命名为 "GameManager"
   - 添加 `GameManager` 组件
   - 添加 `ResourceManager` 组件
   - 添加 `SceneTransitionManager` 组件

3. **设置车内场景**
   - 创建后视镜GameObject，添加 `MirrorController` 组件
   - 创建车内人物GameObject，添加 `CarOccupant` 组件
   - 设置相机和UI

4. **设置逃亡场景**
   - 创建空GameObject，命名为 "MapManager"，添加 `MapManager` 组件
   - 创建地图节点预制体，添加 `MapNode` 组件
   - 创建城镇管理器，添加 `TownManager` 组件
   - 设置UI

5. **设置UI**
   - 创建Canvas
   - 添加资源显示UI（体力、油量）
   - 添加游戏结束UI
   - 添加城镇菜单UI

## 开发注意事项

1. **命名空间**: 所有脚本都使用了 `XEscape` 命名空间，便于管理
2. **单例模式**: GameManager、MapManager、TownManager 使用单例模式
3. **事件系统**: ResourceManager 使用事件通知资源变化
4. **可扩展性**: 代码结构便于后续扩展功能

## 后续开发建议

1. 添加音效和背景音乐
2. 完善UI设计和动画
3. 添加存档系统
4. 添加更多游戏机制（随机事件、选择分支等）
5. 优化地图生成算法
6. 添加教程系统

