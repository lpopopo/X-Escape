# X-Escape

一个Unity 2D逃亡游戏项目

## 游戏概述

这是一个2D逃亡游戏，玩家扮演一家劫匪，需要在车内和逃亡过程中做出决策，最终逃到边境获得胜利。

## 游戏特性

### 场景系统

1. **车内场景 (CarScene)**
   - 玩家可以查看车内所有人的情况
   - 点击后视镜可以切换视角查看车内所有人
   - 可以查看每个人的状态信息

2. **逃亡场景 (EscapeScene)**
   - 随机生成的地图系统
   - 只能看到当前节点周围2个节点范围的地点
   - 可以选择下一个逃亡地点
   - 到达城镇可以进行物资搜寻
   - 到达边境即获得胜利

### 资源系统

- **体力系统**: 人物体力会随时间消耗，可以通过物资恢复
- **油量系统**: 汽车油量在逃亡过程中消耗，可以通过物资恢复
- **物资系统**: 在城镇中搜寻可以获得物资，用于恢复体力和油量

### 游戏状态

- **InCar**: 车内场景
- **Escaping**: 逃亡场景
- **GameOver**: 游戏失败（体力或油量耗尽）
- **Victory**: 游戏胜利（到达边境）

## 2D项目设置

本项目已配置为2D项目，主要设置包括：

- **相机模式**: 所有相机自动设置为正交（Orthographic）模式
- **物理系统**: 使用Physics2D系统
- **编辑器默认模式**: 2D模式
- **相机组件**: 使用`Camera2DSetup`脚本自动配置相机为2D模式

### 相机设置

场景中的相机会自动配置为：
- 正交投影模式（Orthographic）
- 默认Orthographic Size: 5
- Z轴位置: -10（2D相机标准位置）

如果需要在场景中手动设置相机，可以：
1. 添加`Camera2DSetup`组件到相机对象
2. 或在Inspector中手动设置相机为Orthographic模式

## 项目结构

```
Assets/
├── Scripts/
│   ├── Managers/          # 管理器脚本
│   │   ├── GameManager.cs          # 游戏主管理器
│   │   ├── ResourceManager.cs      # 资源管理器
│   │   └── SceneTransitionManager.cs # 场景切换管理器
│   ├── CarScene/          # 车内场景脚本
│   │   ├── MirrorController.cs     # 后视镜控制器
│   │   └── CarOccupant.cs          # 车内人物
│   ├── EscapeScene/       # 逃亡场景脚本
│   │   ├── MapManager.cs           # 地图管理器
│   │   ├── MapNode.cs              # 地图节点
│   │   └── TownManager.cs          # 城镇管理器
│   ├── UI/                # UI脚本
│   │   ├── ResourceUI.cs           # 资源UI
│   │   └── GameOverUI.cs           # 游戏结束UI
│   └── Utilities/         # 工具类
│       ├── ClickableObject.cs      # 可点击对象基类
│       └── Camera2DSetup.cs        # 2D相机自动设置
├── Scenes/                # 场景文件
│   ├── CarScene.unity
│   └── EscapeScene.unity
├── Prefabs/               # 预制体
├── Sprites/               # 精灵图片
└── Materials/             # 材质
```

## 核心系统说明

### GameManager
游戏的核心管理器，负责：
- 管理游戏状态
- 协调各个系统
- 检查游戏结束和胜利条件

### ResourceManager
资源管理系统，负责：
- 管理体力和油量
- 资源消耗和恢复
- 资源变化事件通知

### MapManager
地图管理系统，负责：
- 随机生成地图
- 管理节点可见性
- 处理节点选择和移动

### TownManager
城镇管理系统，负责：
- 处理物资搜寻
- 资源获取

## 使用说明

1. 在Unity中打开项目
2. 创建两个场景：CarScene 和 EscapeScene
3. 在场景中设置相应的GameObject和组件
4. 配置各个管理器的参数
5. 运行游戏

## 开发计划

- [x] 项目架构初始化
- [ ] 场景搭建
- [ ] UI系统完善
- [ ] 音效和背景音乐
- [ ] 游戏平衡性调整
- [ ] 存档系统
