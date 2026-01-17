# Console 没有输出的排查指南

## 问题：运行游戏后 Console 窗口没有任何内容

### 步骤 1：确认 Unity Console 窗口已打开

1. 在 Unity 编辑器中，点击菜单：`Window` → `General` → `Console`
2. 或者按快捷键：`Ctrl+Shift+C` (Windows) 或 `Cmd+Shift+C` (Mac)
3. 确保 Console 窗口是可见的

### 步骤 2：检查 Console 过滤设置

在 Console 窗口顶部，有三个按钮：
- **Clear**（清除）
- **Collapse**（折叠）
- **Clear on Play**（播放时清除）

**重要**：检查 Console 窗口右上角的过滤按钮：
- 确保所有类型的日志都显示：
  - ✅ **Info**（信息）- 蓝色圆圈
  - ✅ **Warning**（警告）- 黄色三角形  
  - ✅ **Error**（错误）- 红色圆圈

如果某个按钮被点击了（变灰），点击它恢复显示。

### 步骤 3：测试最简单的脚本

1. 在场景中创建一个空物体：
   - Hierarchy 右键 → `Create Empty`
   - 命名为 "TestObject"

2. 添加最简单的测试脚本：
   - 选中 TestObject
   - Inspector → `Add Component`
   - 搜索 `SimpleDebugTest`
   - 添加组件

3. 运行游戏（点击 Play 按钮）

4. 查看 Console，应该能看到：
   ```
   SimpleDebugTest: Awake 被调用！
   SimpleDebugTest: Start 被调用！
   SimpleDebugTest: 游戏对象名称: TestObject
   SimpleDebugTest: Update 第一次被调用！
   ```

**如果仍然没有输出**，继续下一步。

### 步骤 4：检查脚本编译状态

1. 查看 Unity 编辑器右下角的状态栏
2. 如果有编译错误，会显示红色错误图标
3. 点击错误图标查看错误信息
4. 修复所有编译错误

### 步骤 5：检查 GameObject 是否激活

1. 选中包含脚本的 GameObject
2. 在 Inspector 顶部，确保 GameObject 名称旁边的复选框是**勾选的**
3. 如果未勾选，点击勾选它

### 步骤 6：检查脚本组件是否启用

1. 选中包含脚本的 GameObject
2. 在 Inspector 中找到脚本组件（如 `SimpleDebugTest`）
3. 确保脚本组件名称旁边的复选框是**勾选的**
4. 如果未勾选，点击勾选它

### 步骤 7：手动测试 Debug.Log

在 Unity 编辑器中：

1. 打开菜单：`Window` → `General` → `Console`
2. 在 Console 窗口底部，有一个输入框
3. 输入：`Debug.Log("测试消息");`
4. 按回车
5. 应该能看到 "测试消息" 出现在 Console 中

如果这个都不行，说明 Console 窗口有问题。

### 步骤 8：检查 Unity 版本和设置

1. 检查 Unity 版本：`Help` → `About Unity`
2. 确保使用的是支持的 Unity 版本（2020.3 或更高）

### 步骤 9：重启 Unity

如果以上都不行：
1. 保存项目（`Ctrl+S` 或 `Cmd+S`）
2. 关闭 Unity
3. 重新打开 Unity 和项目
4. 再次测试

## 如果 SimpleDebugTest 有输出，但 OccupantHoverTooltip 没有输出

这说明 `OccupantHoverTooltip` 脚本可能没有正确添加到场景中。

### 检查 OccupantHoverTooltip

1. 在 Hierarchy 中搜索 "TooltipManager" 或包含 `OccupantHoverTooltip` 的物体
2. 如果没有找到：
   - 创建空物体，命名为 "TooltipManager"
   - 添加 `OccupantHoverTooltip` 组件
   - 勾选 `Enable Debug Log`
3. 运行游戏，应该能看到初始化信息

## 快速验证清单

- [ ] Unity Console 窗口已打开
- [ ] Console 过滤设置正确（Info/Warning/Error 都显示）
- [ ] SimpleDebugTest 脚本已添加到场景中的 GameObject
- [ ] GameObject 已激活（Inspector 顶部复选框勾选）
- [ ] 脚本组件已启用（Inspector 中脚本旁边的复选框勾选）
- [ ] 没有编译错误（Unity 状态栏没有红色错误）
- [ ] 已运行游戏（点击了 Play 按钮）

## 如果以上都不行

请提供以下信息：
1. Unity 版本号
2. Console 窗口的截图
3. Hierarchy 窗口的截图（显示场景中的物体）
4. Inspector 窗口的截图（显示脚本组件的设置）
