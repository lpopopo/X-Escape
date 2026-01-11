# 快速添加图片到场景 - 操作指南

## 第一步：确保图片设置正确

### 1. 选中图片文件
在Project窗口：
```
Assets → Resources → Snipaste_2026-01-11_18-00-03.png
点击选中
```

### 2. 检查Inspector设置
确保以下设置正确：
```
Texture Type: Sprite (2D and UI)  ← 必须是这个！
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Bilinear（平滑）或 Point（像素风格）
```

### 3. 点击Apply按钮
如果修改了设置，必须点击Inspector底部的 **Apply** 按钮。

---

## 第二步：在场景中添加图片（3种方法）

### 方法1：使用我创建的脚本（推荐）✨

#### 步骤1：创建GameObject
```
1. 打开场景：Assets/Scenes/EscapeScene.unity 或 CarScene.unity
2. Hierarchy窗口 → 右键 → Create Empty
3. 重命名为 "ImageDisplay"
```

#### 步骤2：添加脚本
```
1. 从Project窗口找到：Assets/Scripts/SimpleImageDisplay.cs
2. 拖拽到 Hierarchy 中的 ImageDisplay 对象上
```

#### 步骤3：运行游戏
```
点击顶部的 Play ▶️ 按钮
图片会自动显示在场景中心！
```

#### 步骤4：调整图片（可选）
在运行状态下，选中ImageDisplay对象，在Inspector中可以调整：
- **Position**: 图片位置（X, Y, Z）
- **Scale**: 图片大小（1.0 = 原始大小，2.0 = 两倍大小）
- **Sorting Order**: 图片层级（数字越大越在前面）

---

### 方法2：直接拖拽（最简单）

```
1. 在Project窗口找到图片
2. 直接拖到 Scene 视图 或 Hierarchy 窗口
3. Unity会自动创建GameObject并显示图片
```

**优点**：超级快速
**缺点**：不能通过代码控制

---

### 方法3：手动创建Sprite对象

```
1. Hierarchy → 右键 → 2D Object → Sprite
2. 选中创建的Sprite对象
3. Inspector → Sprite Renderer → Sprite字段
4. 点击右边的圆圈图标，选择你的图片
```

---

## 第三步：调整图片显示

### 调整位置
**在Scene视图中**：
- 选中图片对象
- 使用移动工具（快捷键W）拖动

**在Inspector中**：
- Transform → Position → 输入坐标值
- 例如：X=0, Y=0 表示场景中心

### 调整大小
**在Scene视图中**：
- 选中图片对象
- 使用缩放工具（快捷键R）拖动

**在Inspector中**：
- Transform → Scale → 输入缩放值
- 例如：X=2, Y=2 表示放大两倍

### 调整层级（前后顺序）
如果图片被其他对象遮挡：
- Inspector → Sprite Renderer → Sorting Order
- 增大数值（如10、20）让图片显示在前面

---

## 常见问题解决

### ❌ 图片不显示？

**检查清单**：
1. ☐ 图片在Resources文件夹中
2. ☐ 图片设置为Sprite (2D and UI)类型
3. ☐ 点击了Apply按钮
4. ☐ 图片名称拼写正确（不要包含.png）
5. ☐ 摄像机能看到图片位置

**调试方法**：
```
1. 查看Console窗口（Window → General → Console）
2. 应该有绿色的 ✓ 成功消息
3. 如果有红色的 ❌ 错误，按提示修复
```

### ❌ 图片太大或太小？

**解决方案1**：修改Scale
```
SimpleImageDisplay脚本 → Scale字段
- 0.1 = 缩小到十分之一
- 1.0 = 原始大小
- 2.0 = 放大两倍
```

**解决方案2**：修改Pixels Per Unit
```
选中图片 → Inspector
Pixels Per Unit: 数值越大，图片越小
- 100 = 正常大小
- 200 = 缩小一半
- 50 = 放大两倍
```

### ❌ 图片被遮挡看不见？

**检查相机设置**：
```
选中Main Camera → Inspector
确保：
- Projection: Orthographic（2D游戏）
- Size: 5-10（能看到足够大的区域）
```

**调整图片层级**：
```
Sprite Renderer → Sorting Order
设置为较大的值（如10）
```

### ❌ 图片位置不对？

**快速居中**：
```
选中图片对象 → Inspector
Transform → Position 设置为：
X = 0
Y = 0
Z = 0
```

---

## SimpleImageDisplay 脚本功能说明

### Inspector可调参数

1. **Image Name**（图片名称）
   - 默认值：`Snipaste_2026-01-11_18-00-03`
   - 填写Resources中的图片名称
   - **注意**：不要写扩展名（.png）

2. **Position**（位置）
   - 默认值：(0, 0, 0) - 场景中心
   - 修改可移动图片位置

3. **Scale**（缩放）
   - 默认值：1.0 - 原始大小
   - 0.5 = 缩小一半
   - 2.0 = 放大两倍

4. **Sorting Order**（显示层级）
   - 默认值：0
   - 数字越大，越显示在前面

### 运行时调整

**实时预览**：
1. 点击Play运行游戏
2. 选中ImageDisplay对象
3. 在Inspector中修改参数
4. 图片会**立即更新**，无需重启！

---

## 高级用法

### 加载不同的图片

修改脚本的 `imageName` 字段：
```csharp
[SerializeField] private string imageName = "你的图片名称";
```

### 通过代码控制

```csharp
// 获取脚本
SimpleImageDisplay display = GetComponent<SimpleImageDisplay>();

// 显示/隐藏
display.SetVisible(true);   // 显示
display.SetVisible(false);  // 隐藏

// 改变位置
display.SetPosition(new Vector3(5, 3, 0));

// 改变大小
display.SetScale(2.0f);  // 放大两倍
```

---

## 完整测试步骤（5分钟）

### 1. 准备场景（1分钟）
```
✓ 打开 Assets/Scenes/EscapeScene.unity
✓ 确保Scene视图能看到场景
```

### 2. 添加显示对象（1分钟）
```
✓ Hierarchy → 右键 → Create Empty → 命名"TestImage"
✓ 拖拽 SimpleImageDisplay.cs 到 TestImage
```

### 3. 运行测试（1分钟）
```
✓ 点击 Play ▶️ 按钮
✓ 查看Scene视图，应该能看到图片
✓ 查看Console，应该有绿色的成功消息
```

### 4. 调整显示（2分钟）
```
✓ 在运行状态下选中TestImage
✓ 修改Position、Scale、Sorting Order
✓ 实时预览效果
✓ 满意后点击Stop，参数会保存
```

---

## 下一步建议

### 1. 整理Resources文件夹
```
建议创建子文件夹：
Resources/
  Backgrounds/    # 背景图
  Sprites/        # 游戏元素
  UI/            # UI图标
```

### 2. 重命名图片
将图片重命名为有意义的名称：
- `map_background.png` - 地图背景
- `car_interior.png` - 车内背景
- `node_town.png` - 城镇节点

### 3. 应用到游戏
- 作为地图背景：放在地图场景底层
- 作为节点图标：应用到MapNode
- 作为车内背景：放在CarScene中

---

## 需要帮助？

### 检查Console消息
```
Window → General → Console
查看脚本输出的提示信息
```

### 常用快捷键
```
Play/Stop游戏：Ctrl+P (Windows) / Cmd+P (Mac)
移动工具：W
旋转工具：E
缩放工具：R
查看全部：F（选中对象后按F）
```

### 保存场景
```
File → Save Scene
或按 Ctrl+S (Windows) / Cmd+S (Mac)
```

---

## 总结

使用 **SimpleImageDisplay.cs** 脚本的优势：
- ✓ 可以通过Inspector实时调整
- ✓ 运行时可以看到立即效果
- ✓ 可以通过代码控制显示/隐藏
- ✓ 自动处理错误和调试信息
- ✓ 适合快速测试和原型开发

现在去试试吧！🚀
