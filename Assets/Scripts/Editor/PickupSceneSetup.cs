using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

namespace XEscape.Editor
{
    /// <summary>
    /// 自动设置物资拾取场景的编辑器工具
    /// </summary>
    public class PickupSceneSetup : EditorWindow
    {
        // 自定义贴图引用
        private static Sprite playerSprite;
        private static Sprite groundSprite;

        [MenuItem("Tools/Setup Pickup Scene")]
        public static void ShowWindow()
        {
            GetWindow<PickupSceneSetup>("Pickup Scene Setup");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("点击下方按钮自动创建物资拾取场景的所有对象和配置", MessageType.Info);

            EditorGUILayout.Space(5);

            // 贴图设置区域
            EditorGUILayout.LabelField("自定义贴图 (可选):", EditorStyles.boldLabel);
            playerSprite = (Sprite)EditorGUILayout.ObjectField("玩家贴图:", playerSprite, typeof(Sprite), false);
            groundSprite = (Sprite)EditorGUILayout.ObjectField("地面/背景贴图:", groundSprite, typeof(Sprite), false);

            if (playerSprite != null || groundSprite != null)
            {
                EditorGUILayout.HelpBox("已设置自定义贴图，创建时将使用这些贴图", MessageType.Info);
            }

            EditorGUILayout.Space(5);

            // 自动检测按钮
            if (GUILayout.Button("🔍 自动检测 carPlayer & stage-pre", GUILayout.Height(30)))
            {
                AutoDetectSprites();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("⚡ 一键创建完整场景（推荐）", GUILayout.Height(50)))
            {
                SetupCompleteScene();
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("📦 导入 TextMeshPro Essential Resources", GUILayout.Height(30)))
            {
                ImportTMPResources();
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("或单独创建各个部分:", EditorStyles.boldLabel);

            if (GUILayout.Button("1. 创建地面 (Ground)"))
            {
                CreateGround();
            }

            if (GUILayout.Button("2. 创建玩家 (Player)"))
            {
                CreatePlayer();
            }

            if (GUILayout.Button("3. 创建物资预制体 (Item Prefab)"))
            {
                CreateItemPrefab();
            }

            if (GUILayout.Button("4. 创建物资生成器 (Spawner)"))
            {
                CreateSpawner();
            }

            if (GUILayout.Button("5. 创建背包系统 (Inventory)"))
            {
                CreateInventorySystem();
            }

            if (GUILayout.Button("6. 创建UI系统"))
            {
                CreateUISystem();
            }
        }

        /// <summary>
        /// 自动检测并加载 carPlayer 和 stage-pre
        /// </summary>
        private static void AutoDetectSprites()
        {
            playerSprite = FindSpriteByName("carPlayer");
            groundSprite = FindSpriteByName("stage-pre");

            if (playerSprite != null)
            {
                ConfigureSpriteForPixelArt(playerSprite);
                Debug.Log("✅ 找到并配置 carPlayer");
            }

            if (groundSprite != null)
            {
                ConfigureSpriteForPixelArt(groundSprite);
                Debug.Log("✅ 找到并配置 stage-pre");
            }

            if (playerSprite == null && groundSprite == null)
            {
                EditorUtility.DisplayDialog("未找到",
                    "未在 Assets 中找到 carPlayer 或 stage-pre\n\n" +
                    "请确保图片已导入到 Unity 项目中",
                    "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("检测完成",
                    (playerSprite != null ? "✅ 找到 carPlayer\n" : "❌ 未找到 carPlayer\n") +
                    (groundSprite != null ? "✅ 找到 stage-pre\n" : "❌ 未找到 stage-pre\n") +
                    "\n现在可以创建场景了!",
                    "确定");
            }
        }

        /// <summary>
        /// 根据名称查找 Sprite
        /// </summary>
        private static Sprite FindSpriteByName(string spriteName)
        {
            string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            guids = AssetDatabase.FindAssets(spriteName + " t:Texture2D");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
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

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    needsReimport = true;
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    needsReimport = true;
                }

                if (needsReimport)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        private static void SetupCompleteScene()
        {
            if (EditorUtility.DisplayDialog("创建完整场景",
                "这将创建物资拾取场景的所有对象并自动配置。确定继续?",
                "确定", "取消"))
            {
                // 先尝试自动检测贴图
                if (playerSprite == null && groundSprite == null)
                {
                    AutoDetectSprites();
                }

                // 先导入TMP资源
                ImportTMPResources();

                // 设置相机
                SetupCamera();

                // 创建场景对象
                GameObject ground = CreateGround();
                GameObject inventorySystem = CreateInventorySystem();
                GameObject player = CreatePlayer(inventorySystem);
                GameObject itemPrefab = CreateItemPrefab();
                GameObject spawner = CreateSpawner(itemPrefab);
                CreateUISystem(inventorySystem, player);

                // 创建调试器
                CreateDebugger();

                // 选中主要对象便于查看
                Selection.activeGameObject = player;

                Debug.Log("✅ 物资拾取场景创建完成! 可以直接点击播放按钮测试!");
                EditorUtility.DisplayDialog("完成",
                    "场景创建成功!\n\n控制说明:\n- 方向键左右 或 A/D 键移动\n- 接触物资自动拾取\n- 点击背包格子使用物资\n\n现在可以直接点击播放测试!",
                    "开始游戏");
            }
        }

        private static void ImportTMPResources()
        {
            try
            {
                // 使用反射调用TMP的资源导入
                var importerType = System.Type.GetType("TMPro.TMP_PackageResourceImporter, Unity.TextMeshPro.Editor");
                if (importerType != null)
                {
                    var method = importerType.GetMethod("ImportProjectResourcesMenu",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        method.Invoke(null, null);
                        Debug.Log("✅ TextMeshPro Essential Resources 导入完成");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("自动导入TMP资源失败，请手动导入: Window > TextMeshPro > Import TMP Essential Resources\n" + e.Message);
            }
        }

        private static void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
                cameraObj.AddComponent<AudioListener>();
            }

            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6;
            // 如果使用舞台背景，设置天空蓝背景
            if (groundSprite != null)
            {
                mainCamera.backgroundColor = new Color(0.53f, 0.81f, 0.92f); // 天空蓝
            }
            else
            {
                mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            }

            Debug.Log("✅ 相机设置完成");
        }

        private static GameObject CreateGround()
        {
            GameObject ground;

            // 如果有自定义贴图，使用Sprite方式创建
            if (groundSprite != null)
            {
                ground = new GameObject("Ground");
                SpriteRenderer spriteRenderer = ground.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = groundSprite;
                spriteRenderer.sortingOrder = -10; // 确保在最底层
            }
            else
            {
                // 使用默认方块
                ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
                ground.name = "Ground";

                // 设置颜色
                Renderer renderer = ground.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Sprites/Default"));
                    mat.color = new Color(0.6f, 0.4f, 0.2f); // 棕色
                    renderer.material = mat;
                }

                // 移除3D Collider
                Object.DestroyImmediate(ground.GetComponent<MeshCollider>());
            }

            // 确保Ground tag存在
            if (!TagExists("Ground"))
            {
                AddTag("Ground");
            }
            ground.tag = "Ground";

            Transform transform = ground.transform;
            // 如果使用舞台背景，调整位置和大小
            if (groundSprite != null)
            {
                transform.position = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(8, 8, 1);
            }
            else
            {
                transform.position = new Vector3(0, -4, 0);
                transform.localScale = new Vector3(20, 1, 1);
            }

            // 添加2D Collider
            BoxCollider2D groundCollider = ground.GetComponent<BoxCollider2D>();
            if (groundCollider == null)
            {
                groundCollider = ground.AddComponent<BoxCollider2D>();
            }

            // 如果是舞台背景，调整碰撞体到底部
            if (groundSprite != null)
            {
                groundCollider.size = new Vector2(1.5f, 0.2f);
                groundCollider.offset = new Vector2(0, -0.4f);
            }

            Debug.Log("✅ 地面创建完成: " + ground.name + (groundSprite != null ? " (使用自定义贴图)" : ""));
            return ground;
        }

        private static GameObject CreatePlayer(GameObject inventorySystem = null)
        {
            GameObject player;

            // 如果有自定义贴图，使用Sprite方式创建
            if (playerSprite != null)
            {
                player = new GameObject("Player");
                SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = playerSprite;
                spriteRenderer.sortingOrder = 1; // 确保在上层
            }
            else
            {
                // 使用默认方块
                player = GameObject.CreatePrimitive(PrimitiveType.Quad);
                player.name = "Player";

                // 设置颜色
                Renderer renderer = player.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Sprites/Default"));
                    mat.color = Color.blue;
                    renderer.material = mat;
                }

                // 移除MeshCollider
                Object.DestroyImmediate(player.GetComponent<MeshCollider>());
            }

            // 确保Player tag存在
            if (!TagExists("Player"))
            {
                AddTag("Player");
            }
            player.tag = "Player";

            Transform transform = player.transform;
            // 如果使用车辆贴图，调整位置和大小
            if (playerSprite != null)
            {
                transform.position = new Vector3(0, -2, 0);
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.position = new Vector3(0, -3, 0);
                transform.localScale = new Vector3(1, 1, 1);
            }

            // 添加Rigidbody2D
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

            // 添加BoxCollider2D（不是Trigger，这样才能检测到ItemPrefab的Trigger）
            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.isTrigger = false; // Player必须是普通碰撞体！

            // 如果是车辆，调整碰撞体（横向）
            if (playerSprite != null)
            {
                collider.size = new Vector2(1.2f, 0.6f);
            }

            // 添加PlayerController脚本
            var controller = player.AddComponent<PickupScene.PlayerController>();

            // 使用SerializedObject来设置字段（更可靠的方法）
            SerializedObject serializedController = new SerializedObject(controller);
            SerializedProperty moveSpeedProp = serializedController.FindProperty("moveSpeed");
            SerializedProperty moveRangeProp = serializedController.FindProperty("moveRangeX");
            SerializedProperty inventoryManagerProp = serializedController.FindProperty("inventoryManager");

            if (moveSpeedProp != null) moveSpeedProp.floatValue = 5f;
            if (moveRangeProp != null) moveRangeProp.floatValue = 8f;

            // 绑定背包管理器
            if (inventorySystem != null && inventoryManagerProp != null)
            {
                var invManager = inventorySystem.GetComponent<PickupScene.InventoryManager>();
                inventoryManagerProp.objectReferenceValue = invManager;
            }

            serializedController.ApplyModifiedProperties();

            Debug.Log("✅ 玩家创建完成并已绑定背包系统: " + player.name + (playerSprite != null ? " (使用自定义贴图)" : ""));
            return player;
        }

        private static bool TagExists(string tag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) return true;
            }
            return false;
        }

        private static void AddTag(string tag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // 添加新tag
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
            newTagProp.stringValue = tag;

            tagManager.ApplyModifiedProperties();
            Debug.Log($"✅ 已添加Tag: {tag}");
        }

        private static GameObject CreateItemPrefab()
        {
            // 创建物资对象 - 使用空对象+SpriteRenderer，而不是3D Quad
            GameObject itemPrefab = new GameObject("ItemPrefab");
            itemPrefab.transform.localScale = new Vector3(1f, 1f, 1); // 正常大小

            // 添加 SpriteRenderer (2D渲染器，有sortingOrder属性)
            SpriteRenderer spriteRenderer = itemPrefab.AddComponent<SpriteRenderer>();

            // 创建一个更大的白色方块sprite (64x64像素)
            int size = 64;
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point; // 像素风格
            texture.Apply();

            // 创建sprite，设置合适的pixels per unit
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64);

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.green;
            spriteRenderer.sortingOrder = 5; // 设置为最前面！

            // 添加2D组件
            Rigidbody2D rb = itemPrefab.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;

            BoxCollider2D collider = itemPrefab.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1f, 1f); // 设置碰撞体大小

            // 创建文字子对象
            GameObject textObj = new GameObject("ItemText");
            textObj.transform.SetParent(itemPrefab.transform);
            textObj.transform.localPosition = Vector3.zero;
            textObj.transform.localScale = Vector3.one;

            TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
            tmp.text = "物资";
            tmp.fontSize = 2;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform rectTransform = textObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(2, 2);
            }

            // 添加PickupItem脚本
            var pickupItem = itemPrefab.AddComponent<PickupScene.PickupItem>();

            // 使用SerializedObject来设置字段
            SerializedObject serializedItem = new SerializedObject(pickupItem);
            SerializedProperty itemTextProp = serializedItem.FindProperty("itemText");
            SerializedProperty destroyDelayProp = serializedItem.FindProperty("destroyDelay");

            if (itemTextProp != null) itemTextProp.objectReferenceValue = tmp;
            if (destroyDelayProp != null) destroyDelayProp.floatValue = 5f;

            serializedItem.ApplyModifiedProperties();

            // 保存为预制体
            string prefabPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            string fullPath = prefabPath + "/ItemPrefab.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(itemPrefab, fullPath);

            // 删除场景中的临时对象
            Object.DestroyImmediate(itemPrefab);

            Debug.Log("✅ 物资预制体创建完成并已绑定: " + fullPath);
            return prefab;
        }

        private static GameObject CreateSpawner(GameObject itemPrefab = null)
        {
            GameObject spawner = new GameObject("ItemSpawner");
            spawner.transform.position = Vector3.zero;

            var itemSpawner = spawner.AddComponent<PickupScene.ItemSpawner>();

            // 使用SerializedObject来设置字段
            SerializedObject serializedSpawner = new SerializedObject(itemSpawner);
            SerializedProperty itemPrefabProp = serializedSpawner.FindProperty("itemPrefab");
            SerializedProperty spawnIntervalProp = serializedSpawner.FindProperty("spawnInterval");
            SerializedProperty spawnHeightProp = serializedSpawner.FindProperty("spawnHeight");
            SerializedProperty spawnRangeProp = serializedSpawner.FindProperty("spawnRangeX");

            if (itemPrefabProp != null && itemPrefab != null) itemPrefabProp.objectReferenceValue = itemPrefab;
            if (spawnIntervalProp != null) spawnIntervalProp.floatValue = 2f;
            if (spawnHeightProp != null) spawnHeightProp.floatValue = 10f;
            if (spawnRangeProp != null) spawnRangeProp.floatValue = 8f;

            serializedSpawner.ApplyModifiedProperties();

            Debug.Log("✅ 物资生成器创建完成并已绑定预制体: " + spawner.name);
            return spawner;
        }

        private static GameObject CreateInventorySystem()
        {
            GameObject inventorySystem = new GameObject("InventorySystem");
            var inventoryManager = inventorySystem.AddComponent<PickupScene.InventoryManager>();

            // 使用SerializedObject设置背包容量
            SerializedObject serializedInventory = new SerializedObject(inventoryManager);
            SerializedProperty maxSlotsProp = serializedInventory.FindProperty("maxSlots");
            if (maxSlotsProp != null) maxSlotsProp.intValue = 6;
            serializedInventory.ApplyModifiedProperties();

            Debug.Log("✅ 背包系统创建完成: " + inventorySystem.name);
            return inventorySystem;
        }

        private static void CreateUISystem(GameObject inventorySystem = null, GameObject player = null)
        {
            // 创建Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // 创建EventSystem
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // 创建背包面板
            GameObject inventoryPanel = new GameObject("InventoryPanel");
            inventoryPanel.transform.SetParent(canvasObj.transform);

            RectTransform panelRect = inventoryPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0);
            panelRect.anchorMax = new Vector2(0.5f, 0);
            panelRect.pivot = new Vector2(0.5f, 0);
            panelRect.anchoredPosition = new Vector2(0, 100);
            panelRect.sizeDelta = new Vector2(600, 100);

            Image panelImage = inventoryPanel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            HorizontalLayoutGroup layoutGroup = inventoryPanel.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 10;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);

            // 创建槽位预制体
            GameObject slotPrefab = CreateSlotPrefab(inventoryPanel.transform);

            // 保存槽位预制体
            string prefabPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            string slotPrefabPath = prefabPath + "/SlotPrefab.prefab";
            GameObject slotPrefabAsset = PrefabUtility.SaveAsPrefabAsset(slotPrefab, slotPrefabPath);
            DestroyImmediate(slotPrefab);

            // 创建UI管理器
            GameObject uiManager = new GameObject("InventoryUIManager");
            uiManager.transform.SetParent(canvasObj.transform);
            var inventoryUI = uiManager.AddComponent<PickupScene.InventoryUI>();

            // 使用SerializedObject设置引用
            SerializedObject serializedUI = new SerializedObject(inventoryUI);
            SerializedProperty panelProp = serializedUI.FindProperty("inventoryPanel");
            SerializedProperty slotPrefabProp = serializedUI.FindProperty("slotPrefab");
            SerializedProperty managerProp = serializedUI.FindProperty("inventoryManager");

            if (panelProp != null) panelProp.objectReferenceValue = inventoryPanel.transform;
            if (slotPrefabProp != null) slotPrefabProp.objectReferenceValue = slotPrefabAsset;
            if (managerProp != null && inventorySystem != null)
            {
                var invManager = inventorySystem.GetComponent<PickupScene.InventoryManager>();
                managerProp.objectReferenceValue = invManager;
            }

            serializedUI.ApplyModifiedProperties();

            Debug.Log("✅ UI系统创建完成并已绑定所有引用!");
        }

        private static GameObject CreateSlotPrefab(Transform parent)
        {
            GameObject slotObj = new GameObject("SlotPrefab");
            slotObj.transform.SetParent(parent, false);

            RectTransform rectTransform = slotObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80, 80);

            Image image = slotObj.AddComponent<Image>();
            image.color = Color.gray;

            Button button = slotObj.AddComponent<Button>();

            // 添加文字
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(slotObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "空";
            text.fontSize = 14;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 80;
            layoutElement.preferredHeight = 80;

            return slotObj;
        }

        /// <summary>
        /// 创建调试器
        /// </summary>
        private static GameObject CreateDebugger()
        {
            GameObject debugger = new GameObject("PickupDebugger");
            debugger.AddComponent<PickupScene.PickupDebugger>();

            Debug.Log("✅ 创建调试器 - 运行时可查看调试信息");
            return debugger;
        }
    }
}
