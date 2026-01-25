using UnityEngine;
using UnityEditor;

namespace XEscape.Editor
{
    /// <summary>
    /// 自动查找并应用自定义贴图
    /// </summary>
    public class ApplyCustomSprites : EditorWindow
    {
        [MenuItem("Tools/🎨 应用自定义贴图 (carPlayer & stage-pre)")]
        public static void ApplySprites()
        {
            if (EditorUtility.DisplayDialog("应用自定义贴图",
                "将自动查找 Assets/Art 中的贴图并应用到场景\n\n" +
                "- carPlayer → 替换Player（车辆）\n" +
                "- stage-pre → 创建Background（房子背景，最底层）\n" +
                "- Ground保持原位（碰撞地面）\n\n" +
                "确定继续?",
                "确定", "取消"))
            {
                ExecuteApply();
            }
        }

        private static void ExecuteApply()
        {
            Debug.Log("🔍 开始查找贴图...");

            // 查找 Sprite
            Sprite carSprite = FindSpriteByName("carPlayer");
            Sprite stageSprite = FindSpriteByName("stage-pre");

            if (carSprite == null && stageSprite == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "未找到任何贴图!\n\n请确保图片在 Assets/Art 文件夹中，并且名称为:\n" +
                    "- carPlayer\n" +
                    "- stage-pre",
                    "确定");
                return;
            }

            // 配置 Sprite 设置
            if (carSprite != null)
            {
                ConfigureSpriteForPixelArt(carSprite);
                Debug.Log("✅ 找到并配置 carPlayer");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到 carPlayer");
            }

            if (stageSprite != null)
            {
                ConfigureSpriteForPixelArt(stageSprite);
                Debug.Log("✅ 找到并配置 stage-pre");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到 stage-pre");
            }

            // 查找场景中的对象
            GameObject player = GameObject.Find("Player");
            GameObject existingBackground = GameObject.Find("Background");

            bool appliedAny = false;

            // 应用到玩家
            if (player != null && carSprite != null)
            {
                ApplySpriteToObject(player, carSprite, "Player");
                ConfigurePlayerForCar(player);
                appliedAny = true;
            }
            else if (player == null)
            {
                Debug.LogWarning("⚠️ 场景中未找到 Player 对象");
            }

            // 创建或更新背景
            if (stageSprite != null)
            {
                GameObject background;
                if (existingBackground != null)
                {
                    background = existingBackground;
                    Debug.Log("✅ 更新现有 Background");
                }
                else
                {
                    background = new GameObject("Background");
                    Debug.Log("✅ 创建新 Background");
                }

                ApplySpriteToObject(background, stageSprite, "Background");
                ConfigureBackground(background);
                appliedAny = true;
            }

            if (appliedAny)
            {
                // 调整相机
                AdjustCamera();

                EditorUtility.DisplayDialog("完成",
                    "贴图应用成功!\n\n" +
                    "✅ carPlayer已替换Player（车辆）\n" +
                    "✅ stage-pre已创建为Background（房子背景，最底层）\n" +
                    "✅ Ground保持原位（碰撞地面）\n" +
                    "✅ 已配置为像素艺术风格\n" +
                    "✅ 已调整图层顺序和相机\n\n" +
                    "现在可以点击播放测试!",
                    "开始游戏");

                Debug.Log("✅ 所有贴图应用完成!");
            }
            else
            {
                EditorUtility.DisplayDialog("提示",
                    "请先使用 Tools → Setup Pickup Scene 创建场景,\n" +
                    "然后再运行此工具应用贴图。",
                    "确定");
            }
        }

        /// <summary>
        /// 根据名称查找 Sprite
        /// </summary>
        private static Sprite FindSpriteByName(string spriteName)
        {
            // 查找所有匹配名称的资源
            string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite != null)
                {
                    Debug.Log($"找到 Sprite: {sprite.name} at {path}");
                    return sprite;
                }
            }

            // 如果没找到，尝试作为 Texture2D 查找
            guids = AssetDatabase.FindAssets(spriteName + " t:Texture2D");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);

                // 将 Texture 转换为 Sprite
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.maxTextureSize = 2048;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                    Debug.Log($"✅ 已将 {spriteName} 转换为 Sprite");
                }

                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return null;
        }

        /// <summary>
        /// 配置 Sprite 为像素艺术风格
        /// </summary>
        private static void ConfigureSpriteForPixelArt(Sprite sprite)
        {
            if (sprite == null) return;

            string path = AssetDatabase.GetAssetPath(sprite);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                bool needsReimport = false;

                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    needsReimport = true;
                }

                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    needsReimport = true;
                }

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    needsReimport = true;
                    Debug.Log($"✅ 设置 {sprite.name} 为 Point Filter (像素风格)");
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    needsReimport = true;
                }

                if (importer.maxTextureSize < 2048)
                {
                    importer.maxTextureSize = 2048;
                    needsReimport = true;
                }

                if (needsReimport)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    Debug.Log($"✅ 已配置 {sprite.name} 为像素艺术风格");
                }
            }
        }

        /// <summary>
        /// 将 Sprite 应用到对象
        /// </summary>
        private static void ApplySpriteToObject(GameObject obj, Sprite sprite, string objName)
        {
            // 移除旧的渲染器
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Object.DestroyImmediate(meshRenderer);
            }

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Object.DestroyImmediate(meshFilter);
            }

            // 添加或更新 SpriteRenderer
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = obj.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = sprite;

            Debug.Log($"✅ 已将 {sprite.name} 应用到 {objName}");
        }

        /// <summary>
        /// 配置玩家车辆
        /// </summary>
        private static void ConfigurePlayerForCar(GameObject player)
        {
            // 设置位置和大小
            player.transform.position = new Vector3(0, -2, 0);
            player.transform.localScale = new Vector3(2f, 2f, 1);

            // 配置 SpriteRenderer
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 1;
            }

            // 调整碰撞体（车辆是横向的）
            BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = new Vector2(1.2f, 0.6f);
                collider.offset = Vector2.zero;
            }

            Debug.Log("✅ 已配置 Player 为车辆样式（横向，碰撞体已调整）");
        }

        /// <summary>
        /// 配置背景（装饰用，无碰撞）
        /// </summary>
        private static void ConfigureBackground(GameObject background)
        {
            // 设置位置和大小
            background.transform.position = new Vector3(0, 0, 0);
            background.transform.localScale = new Vector3(10f, 10f, 1);

            // 配置 SpriteRenderer
            SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = -20; // 最底层，在Ground之后
            }

            // 移除碰撞体（背景不需要碰撞）
            BoxCollider2D collider = background.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
                Debug.Log("✅ 已移除 Background 的碰撞体（仅作装饰）");
            }

            Debug.Log("✅ 已配置 Background 为装饰背景（房子场景，最底层）");
        }

        /// <summary>
        /// 调整相机
        /// </summary>
        private static void AdjustCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.position = new Vector3(0, 0, -10);
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 6f;
                mainCamera.backgroundColor = new Color(0.53f, 0.81f, 0.92f); // 天空蓝

                Debug.Log("✅ 已调整相机（正交视图，天空蓝背景）");
            }
        }
    }
}
