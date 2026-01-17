# ViewSwitcher 字段说明

## 📋 字段必要性分析

### ✅ 必须设置的字段：

1. **Interior View** (`interiorView`)
   - **作用**：指定车内场景的 GameObject（包含背景和人物的父对象）
   - **必须**：是
   - **说明**：如果未设置，代码会尝试自动查找，但可能找不到

2. **Front Window View** (`frontWindowView`)
   - **作用**：指定车前窗场景的 GameObject
   - **必须**：是
   - **说明**：如果未设置，代码会自动创建，但最好手动设置

### ❌ 不需要设置的字段（代码会自动处理）：

3. **Interior View Script** (`interiorViewScript`)
   - **作用**：用于自动查找 `CarInteriorView` 组件
   - **必须**：否
   - **说明**：代码会自动查找，不需要手动设置
   - **何时需要**：如果 `Interior View` 未设置，代码会用这个字段自动查找

4. **Front Window Background** (`frontWindowBackground`)
   - **作用**：直接设置车前窗背景的 Sprite
   - **必须**：否
   - **说明**：如果 `Front Window View` 已经设置了背景图，就不需要这个
   - **何时需要**：如果想让代码自动设置车前窗背景图

5. **Front Window Resource Path** (`frontWindowResourcePath`)
   - **作用**：从 Resources 文件夹加载车前窗背景图
   - **必须**：否
   - **说明**：默认值是 "background-back"，如果图片在 Resources 文件夹中会自动加载
   - **何时需要**：如果想让代码自动从 Resources 加载背景图

### ⚠️ 可选字段（建议设置）：

6. **Switch Button** (`switchButton`)
   - **作用**：切换视角的按钮
   - **必须**：否（代码会自动查找）
   - **说明**：建议手动设置，确保连接正确

7. **Button Text** (`buttonText`)
   - **作用**：按钮的文本组件，用于更新按钮文字
   - **必须**：否（代码会自动查找）
   - **说明**：建议手动设置，确保按钮文字能正确更新

8. **Main Camera** (`mainCamera`)
   - **作用**：主相机引用
   - **必须**：否（代码会自动查找 `Camera.main`）
   - **说明**：通常不需要手动设置

---

## 🎯 推荐设置

### 最小配置（必须）：
- ✅ `Interior View`：车内场景对象
- ✅ `Front Window View`：车前窗场景对象

### 推荐配置（最佳体验）：
- ✅ `Interior View`：车内场景对象
- ✅ `Front Window View`：车前窗场景对象
- ✅ `Switch Button`：切换按钮
- ✅ `Button Text`：按钮文本组件

### 可选配置（如果需要自动加载背景）：
- `Front Window Resource Path`：如果想让代码自动从 Resources 加载车前窗背景

---

## 📝 总结

**必须设置的字段只有 2 个**：
1. `Interior View`
2. `Front Window View`

**其他字段都是可选的**，代码会自动处理或查找。
