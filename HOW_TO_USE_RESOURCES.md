# Unity Resources 文件夹使用完全指南

## Resources 文件夹是什么？

**Resources** 是Unity的特殊文件夹，用于存放可以通过代码动态加载的资源（图片、音频、预制体等）。

### 特点
- ✓ 可以通过 `Resources.Load()` 在代码中加载
- ✓ 资源会被打包到游戏中
- ✓ 适合动态加载的内容（如角色头像、道具图标）
- ✗ 会增加初始加载时间和内存占用

---

## 方法1：直接在场景中添加图片（推荐新手）

### 步骤1：创建 Resources 文件夹
```
在Project窗口：
1. Assets文件夹上右键
2. Create → Folder
3. 命名为 "Resources"（必须是这个名字！）
```

### 步骤2：导入图片
```
1. 将图片文件拖到 Assets/Resources/ 文件夹
2. 或者：Assets/Resources → 右键 → Import New Asset
```

**支持的图片格式**：
- `.png` - 推荐（支持透明）
- `.jpg` / `.jpeg` - 适合背景图
- `.psd` - Photoshop文件
- `.tga` - 游戏常用格式

### 步骤3：设置图片为Sprite
```
1. 选中导入的图片
2. 在Inspector中：
   - Texture Type: 选择 "Sprite (2D and UI)"
   - Pixels Per Unit: 100（默认）
   - Filter Mode: Point (no filter) - 像素风格
                  Bilinear - 平滑（推荐）
3. 点击 Apply 按钮
```

### 步骤4：添加到场景中

#### 方式A：拖拽法（最简单）
```
1. 从Project窗口直接拖图片到 Hierarchy 或 Scene 视图
2. Unity会自动创建GameObject并添加SpriteRenderer
3. 调整位置和大小
```

#### 方式B：手动创建
```
1. Hierarchy → 右键 → 2D Object → Sprite
2. 选中创建的Sprite对象
3. 在Inspector的Sprite Renderer组件中：
   - Sprite字段：点击圆圈图标，选择你的图片
   - 或者从Project拖图片到Sprite字段
```

---

## 方法2：通过代码加载Resources中的图片

### 目录结构示例
```
Assets/
  Resources/
    Sprites/
      Characters/
        player.png
        enemy.png
      UI/
        icon_health.png
        icon_fuel.png
    Prefabs/
      Node.prefab
```

### 代码示例1：加载并显示图片

```csharp
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 方法1：加载Resources根目录的图片
        Sprite sprite = Resources.Load<Sprite>("player");

        // 方法2：加载子文件夹中的图片
        Sprite characterSprite = Resources.Load<Sprite>("Sprites/Characters/player");

        // 方法3：加载UI图片
        Sprite iconSprite = Resources.Load<Sprite>("Sprites/UI/icon_health");

        // 应用到SpriteRenderer
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("加载图片失败或SpriteRenderer未赋值！");
        }
    }
}
```

### 代码示例2：动态创建带图片的GameObject

```csharp
using UnityEngine;

public class DynamicSpriteCreator : MonoBehaviour
{
    void Start()
    {
        CreateSpriteObject("Sprites/Characters/player", new Vector3(0, 0, 0));
    }

    GameObject CreateSpriteObject(string spritePath, Vector3 position)
    {
        // 加载图片
        Sprite sprite = Resources.Load<Sprite>(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"找不到图片: {spritePath}");
            return null;
        }

        // 创建GameObject
        GameObject obj = new GameObject(sprite.name);
        obj.transform.position = position;

        // 添加SpriteRenderer并设置图片
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        return obj;
    }
}
```

### 代码示例3：加载多张图片（图集）

```csharp
using UnityEngine;

public class MultiSpriteLoader : MonoBehaviour
{
    void Start()
    {
        // 加载所有角色图片
        LoadAllCharacters();
    }

    void LoadAllCharacters()
    {
        // 加载文件夹中所有Sprite
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/Characters");

        foreach (Sprite sprite in allSprites)
        {
            Debug.Log($"加载了图片: {sprite.name}");
        }

        // 使用第一张图片
        if (allSprites.Length > 0)
        {
            GetComponent<SpriteRenderer>().sprite = allSprites[0];
        }
    }
}
```

---

## 方法3：在UI中使用Resources图片

### UI Image 组件

```csharp
using UnityEngine;
using UnityEngine.UI;

public class UIImageLoader : MonoBehaviour
{
    [SerializeField] private Image uiImage;  // UI的Image组件

    void Start()
    {
        // 加载图片
        Sprite sprite = Resources.Load<Sprite>("Sprites/UI/icon_health");

        // 应用到UI Image
        if (uiImage != null && sprite != null)
        {
            uiImage.sprite = sprite;
        }
    }

    // 动态切换图片
    public void ChangeIcon(string iconName)
    {
        Sprite newSprite = Resources.Load<Sprite>($"Sprites/UI/{iconName}");
        if (newSprite != null)
        {
            uiImage.sprite = newSprite;
        }
    }
}
```

---

## X-Escape 项目实战示例

### 1. 地图节点图标

```csharp
// 在MapNode.cs中添加
using UnityEngine;

namespace XEscape.EscapeScene
{
    public class MapNode : MonoBehaviour
    {
        [SerializeField] private NodeType nodeType;
        private SpriteRenderer nodeRenderer;

        private void Start()
        {
            nodeRenderer = GetComponent<SpriteRenderer>();
            LoadNodeSprite();
        }

        private void LoadNodeSprite()
        {
            string spritePath = "";

            switch (nodeType)
            {
                case NodeType.Town:
                    spritePath = "Sprites/Nodes/node_town";
                    break;
                case NodeType.Road:
                    spritePath = "Sprites/Nodes/node_road";
                    break;
                case NodeType.Border:
                    spritePath = "Sprites/Nodes/node_border";
                    break;
                case NodeType.Danger:
                    spritePath = "Sprites/Nodes/node_danger";
                    break;
            }

            if (!string.IsNullOrEmpty(spritePath))
            {
                Sprite sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null && nodeRenderer != null)
                {
                    nodeRenderer.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning($"未找到节点图片: {spritePath}");
                }
            }
        }
    }
}
```

### 2. 车内人物头像

```csharp
// 在CarOccupant.cs中添加
using UnityEngine;

namespace XEscape.CarScene
{
    public class CarOccupant : MonoBehaviour
    {
        [SerializeField] private string characterName;
        private SpriteRenderer portraitRenderer;

        private void Start()
        {
            LoadPortrait();
        }

        private void LoadPortrait()
        {
            // 从Resources加载头像
            Sprite portrait = Resources.Load<Sprite>($"Sprites/Characters/{characterName}");

            if (portrait != null)
            {
                portraitRenderer = GetComponent<SpriteRenderer>();
                if (portraitRenderer != null)
                {
                    portraitRenderer.sprite = portrait;
                }
            }
            else
            {
                Debug.LogWarning($"未找到角色头像: {characterName}");
            }
        }
    }
}
```

### 3. UI资源图标

```csharp
// 新建脚本：Assets/Scripts/UI/ResourceIconLoader.cs
using UnityEngine;
using UnityEngine.UI;

namespace XEscape.UI
{
    public class ResourceIconLoader : MonoBehaviour
    {
        [SerializeField] private Image staminaIcon;
        [SerializeField] private Image fuelIcon;

        private void Start()
        {
            LoadIcons();
        }

        private void LoadIcons()
        {
            // 加载资源图标
            Sprite staminaSprite = Resources.Load<Sprite>("Sprites/UI/icon_stamina");
            Sprite fuelSprite = Resources.Load<Sprite>("Sprites/UI/icon_fuel");

            if (staminaIcon != null && staminaSprite != null)
            {
                staminaIcon.sprite = staminaSprite;
            }

            if (fuelIcon != null && fuelSprite != null)
            {
                fuelIcon.sprite = fuelSprite;
            }
        }
    }
}
```

---

## 推荐的Resources文件夹结构

```
Assets/
  Resources/
    Sprites/           # 2D图片
      Characters/      # 角色图片
        player.png
        npc_01.png
        enemy_01.png
      Nodes/          # 地图节点
        node_town.png
        node_road.png
        node_border.png
        node_danger.png
      UI/             # UI图标
        icon_health.png
        icon_fuel.png
        icon_stamina.png
        button_normal.png
      Backgrounds/    # 背景图
        car_interior.png
        map_bg.png

    Audio/            # 音频文件
      BGM/
        menu_music.mp3
        game_music.mp3
      SFX/
        click.wav
        success.wav

    Prefabs/          # 预制体
      NodePrefab.prefab
      ParticleEffect.prefab
```

---

## Resources vs 其他方法对比

### Resources（动态加载）
```csharp
// 优点：灵活，按需加载
// 缺点：路径是字符串，容易出错
Sprite sprite = Resources.Load<Sprite>("path/to/sprite");
```

### SerializeField（Inspector引用）
```csharp
// 优点：安全，可视化
// 缺点：必须手动拖拽
[SerializeField] private Sprite mySprite;
```

### Addressables（推荐大项目）
```csharp
// 优点：高效、支持远程加载
// 缺点：需要额外配置
Addressables.LoadAssetAsync<Sprite>("sprite_key");
```

---

## 常见问题解答

### Q1: Resources.Load 返回 null？

**可能原因**：
1. 路径错误（不要包含 "Assets/Resources/" 前缀）
2. 文件扩展名不要写（写 "player" 而不是 "player.png"）
3. 文件夹名称拼写错误
4. 图片未设置为Sprite类型

**调试方法**：
```csharp
Sprite sprite = Resources.Load<Sprite>("path/to/sprite");
if (sprite == null)
{
    Debug.LogError("加载失败！检查路径和文件是否存在");
}
```

### Q2: 如何加载子文件夹中的资源？

```csharp
// 正确：使用 / 分隔
Resources.Load<Sprite>("Sprites/Characters/player");

// 错误：不要使用 \
Resources.Load<Sprite>("Sprites\\Characters\\player");  // ❌
```

### Q3: 图片导入后看不到？

**检查Import Settings**：
1. 选中图片
2. Inspector → Texture Type → Sprite (2D and UI)
3. 点击 Apply

### Q4: 图片显示模糊或像素化？

**解决方法**：
```
选中图片 → Inspector：
- Filter Mode: Point (像素风格) 或 Bilinear (平滑)
- Compression: None (最清晰) 或 Low Quality (压缩)
- Max Size: 2048 或更高
```

### Q5: Resources加载很慢？

**优化建议**：
1. 只加载需要的资源
2. 在Start/Awake中预加载
3. 考虑使用对象池
4. 大项目使用Addressables代替

---

## 完整示例：资源管理器

创建一个统一管理Resources的脚本：

```csharp
// Assets/Scripts/Managers/ResourceLoader.cs
using UnityEngine;
using System.Collections.Generic;

namespace XEscape.Managers
{
    /// <summary>
    /// 资源加载管理器，统一管理Resources资源
    /// </summary>
    public class ResourceLoader : MonoBehaviour
    {
        public static ResourceLoader Instance { get; private set; }

        // 缓存已加载的资源
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 加载Sprite（带缓存）
        /// </summary>
        public Sprite LoadSprite(string path)
        {
            // 检查缓存
            if (spriteCache.ContainsKey(path))
            {
                return spriteCache[path];
            }

            // 加载资源
            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite != null)
            {
                // 添加到缓存
                spriteCache[path] = sprite;
            }
            else
            {
                Debug.LogError($"无法加载Sprite: {path}");
            }

            return sprite;
        }

        /// <summary>
        /// 预加载一组资源
        /// </summary>
        public void PreloadSprites(string[] paths)
        {
            foreach (string path in paths)
            {
                LoadSprite(path);
            }
            Debug.Log($"预加载了 {paths.Length} 个Sprite资源");
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            spriteCache.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}
```

**使用示例**：
```csharp
// 在其他脚本中使用
Sprite sprite = ResourceLoader.Instance.LoadSprite("Sprites/Characters/player");
spriteRenderer.sprite = sprite;
```

---

## 快速上手步骤（针对X-Escape项目）

### 1. 创建文件夹结构
```
Assets/
  Resources/
    Sprites/
      Nodes/
      Characters/
      UI/
    Audio/
      BGM/
      SFX/
```

### 2. 导入测试图片
- 下载或创建几张PNG图片
- 拖到对应文件夹
- 设置为Sprite类型

### 3. 在场景中测试
```
方法1：直接拖拽到Scene视图
方法2：创建测试脚本加载
```

### 4. 应用到项目
- MapNode使用节点图标
- CarOccupant使用角色头像
- UI使用资源图标

---

## 下一步

1. ✅ 创建Resources文件夹结构
2. ✅ 导入项目所需的图片资源
3. ✅ 修改MapNode等脚本，动态加载图片
4. ✅ 测试场景中的图片显示

需要我帮你创建测试用的图片加载脚本，或者生成一些占位图片吗？
