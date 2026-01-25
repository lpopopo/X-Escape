using UnityEngine;
using UnityEditor;

namespace XEscape.Editor
{
    /// <summary>
    /// 诊断和修复物资拾取场景问题
    /// </summary>
    public class DiagnosePickupScene : EditorWindow
    {
        [MenuItem("Tools/🔍 诊断物资生成问题")]
        public static void ShowWindow()
        {
            var window = GetWindow<DiagnosePickupScene>("诊断工具");
            window.minSize = new Vector2(400, 500);
        }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.HelpBox("检查物资生成器问题", MessageType.Info);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("🔍 开始诊断", GUILayout.Height(40)))
            {
                Diagnose();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("🔧 自动修复所有问题", GUILayout.Height(40)))
            {
                AutoFix();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("🔄 重新绑定 ItemPrefab", GUILayout.Height(30)))
            {
                RebindItemPrefab();
            }

            EditorGUILayout.EndScrollView();
        }

        private void Diagnose()
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("🔍 开始诊断物资生成问题...");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            bool hasIssues = false;

            // 1. 检查场景中是否有 ItemSpawner
            GameObject spawnerObj = GameObject.Find("ItemSpawner");
            if (spawnerObj == null)
            {
                Debug.LogError("❌ 场景中未找到 ItemSpawner 对象!");
                Debug.LogWarning("   解决方案: Tools → Setup Pickup Scene 创建场景");
                hasIssues = true;
            }
            else
            {
                Debug.Log("✅ 找到 ItemSpawner 对象");

                // 检查 ItemSpawner 组件
                var spawner = spawnerObj.GetComponent<PickupScene.ItemSpawner>();
                if (spawner == null)
                {
                    Debug.LogError("❌ ItemSpawner 对象上没有 ItemSpawner 脚本!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log("✅ ItemSpawner 脚本正常");

                    // 检查 itemPrefab 引用
                    SerializedObject serializedSpawner = new SerializedObject(spawner);
                    SerializedProperty prefabProp = serializedSpawner.FindProperty("itemPrefab");

                    if (prefabProp.objectReferenceValue == null)
                    {
                        Debug.LogError("❌ ItemSpawner 的 itemPrefab 引用为空!");
                        Debug.LogWarning("   这是物资不生成的主要原因!");
                        hasIssues = true;
                    }
                    else
                    {
                        Debug.Log($"✅ itemPrefab 已绑定: {prefabProp.objectReferenceValue.name}");
                    }

                    // 检查其他设置
                    SerializedProperty intervalProp = serializedSpawner.FindProperty("spawnInterval");
                    SerializedProperty heightProp = serializedSpawner.FindProperty("spawnHeight");
                    SerializedProperty rangeProp = serializedSpawner.FindProperty("spawnRangeX");

                    Debug.Log($"   生成间隔: {intervalProp.floatValue} 秒");
                    Debug.Log($"   生成高度: {heightProp.floatValue}");
                    Debug.Log($"   生成范围: {rangeProp.floatValue}");
                }
            }

            // 2. 检查 ItemPrefab 预制体是否存在
            Debug.Log("\n━━━ 检查预制体 ━━━");
            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemPrefab.prefab");
            if (itemPrefab == null)
            {
                Debug.LogError("❌ ItemPrefab.prefab 不存在!");
                Debug.LogWarning("   解决方案: Tools → Setup Pickup Scene → 3. 创建物资预制体");
                hasIssues = true;
            }
            else
            {
                Debug.Log("✅ ItemPrefab.prefab 存在");

                // 检查预制体组件
                var pickupItem = itemPrefab.GetComponent<PickupScene.PickupItem>();
                if (pickupItem == null)
                {
                    Debug.LogError("❌ ItemPrefab 上没有 PickupItem 脚本!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log("✅ PickupItem 脚本正常");
                }

                // 检查 Rigidbody2D
                var rb = itemPrefab.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    Debug.LogError("❌ ItemPrefab 缺少 Rigidbody2D!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log($"✅ Rigidbody2D 存在 (Gravity: {rb.gravityScale})");
                }

                // 检查 Collider
                var collider = itemPrefab.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    Debug.LogError("❌ ItemPrefab 缺少 BoxCollider2D!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log($"✅ BoxCollider2D 存在 (Is Trigger: {collider.isTrigger})");
                }

                // 检查 SpriteRenderer 和 Sorting Order
                var sr = itemPrefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Debug.Log($"✅ SpriteRenderer 存在 (Sorting Order: {sr.sortingOrder})");
                    if (sr.sortingOrder < 1)
                    {
                        Debug.LogWarning($"⚠️ Sorting Order ({sr.sortingOrder}) 可能太低，物资可能被遮挡!");
                        Debug.LogWarning("   建议设置为 5");
                        hasIssues = true;
                    }
                }
            }

            // 3. 检查 Player
            Debug.Log("\n━━━ 检查 Player ━━━");
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError("❌ 场景中未找到 Player 对象!");
                hasIssues = true;
            }
            else
            {
                Debug.Log("✅ Player 存在");

                var controller = player.GetComponent<PickupScene.PlayerController>();
                if (controller == null)
                {
                    Debug.LogError("❌ Player 缺少 PlayerController 脚本!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log("✅ PlayerController 存在");
                }
            }

            // 4. 检查 Ground
            Debug.Log("\n━━━ 检查 Ground ━━━");
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                Debug.LogError("❌ 场景中未找到 Ground 对象!");
                hasIssues = true;
            }
            else
            {
                Debug.Log("✅ Ground 存在");
                if (ground.tag != "Ground")
                {
                    Debug.LogError("❌ Ground 对象的 Tag 不是 'Ground'!");
                    hasIssues = true;
                }
                else
                {
                    Debug.Log("✅ Ground Tag 正确");
                }
            }

            // 总结
            Debug.Log("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            if (hasIssues)
            {
                Debug.LogWarning("⚠️ 发现问题! 点击 '🔧 自动修复所有问题' 按钮");
                EditorUtility.DisplayDialog("诊断完成",
                    "发现问题！\n\n请查看 Console 窗口了解详情。\n\n" +
                    "点击 '🔧 自动修复所有问题' 按钮尝试修复。",
                    "确定");
            }
            else
            {
                Debug.Log("✅ 所有检查通过！物资生成器应该正常工作。");
                EditorUtility.DisplayDialog("诊断完成",
                    "所有检查通过！\n\n如果物资还是不生成，请尝试：\n" +
                    "1. 重新进入播放模式\n" +
                    "2. 检查 Console 是否有错误信息\n" +
                    "3. 确认场景中有 ItemSpawner 对象",
                    "确定");
            }
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        private void AutoFix()
        {
            Debug.Log("🔧 开始自动修复...");

            bool hasFixed = false;

            // 修复 ItemSpawner 的 itemPrefab 引用
            GameObject spawnerObj = GameObject.Find("ItemSpawner");
            if (spawnerObj != null)
            {
                var spawner = spawnerObj.GetComponent<PickupScene.ItemSpawner>();
                if (spawner != null)
                {
                    SerializedObject serializedSpawner = new SerializedObject(spawner);
                    SerializedProperty prefabProp = serializedSpawner.FindProperty("itemPrefab");

                    if (prefabProp.objectReferenceValue == null)
                    {
                        // 尝试加载 ItemPrefab
                        GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemPrefab.prefab");
                        if (itemPrefab != null)
                        {
                            prefabProp.objectReferenceValue = itemPrefab;
                            serializedSpawner.ApplyModifiedProperties();
                            Debug.Log("✅ 已重新绑定 ItemPrefab 到 ItemSpawner");
                            hasFixed = true;
                        }
                        else
                        {
                            Debug.LogError("❌ 无法找到 ItemPrefab.prefab，请先创建预制体");
                        }
                    }
                }
            }

            // 修复 ItemPrefab 的 Sorting Order
            GameObject itemPrefab2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemPrefab.prefab");
            if (itemPrefab2 != null)
            {
                var sr = itemPrefab2.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sortingOrder < 1)
                {
                    sr.sortingOrder = 5;
                    PrefabUtility.SavePrefabAsset(itemPrefab2);
                    Debug.Log("✅ 已修复 ItemPrefab 的 Sorting Order = 5");
                    hasFixed = true;
                }
            }

            if (hasFixed)
            {
                EditorUtility.DisplayDialog("修复完成",
                    "已自动修复部分问题！\n\n" +
                    "请查看 Console 了解修复内容。\n\n" +
                    "现在可以尝试播放游戏。",
                    "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("无需修复",
                    "未发现可以自动修复的问题。\n\n" +
                    "如果物资仍不生成，请运行诊断查看详细信息。",
                    "确定");
            }
        }

        private void RebindItemPrefab()
        {
            GameObject spawnerObj = GameObject.Find("ItemSpawner");
            if (spawnerObj == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "场景中未找到 ItemSpawner 对象！\n\n" +
                    "请先创建场景。",
                    "确定");
                return;
            }

            var spawner = spawnerObj.GetComponent<PickupScene.ItemSpawner>();
            if (spawner == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "ItemSpawner 对象上没有脚本！",
                    "确定");
                return;
            }

            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemPrefab.prefab");
            if (itemPrefab == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "找不到 ItemPrefab.prefab！\n\n" +
                    "请先创建预制体:\n" +
                    "Tools → Setup Pickup Scene → 3. 创建物资预制体",
                    "确定");
                return;
            }

            SerializedObject serializedSpawner = new SerializedObject(spawner);
            SerializedProperty prefabProp = serializedSpawner.FindProperty("itemPrefab");
            prefabProp.objectReferenceValue = itemPrefab;
            serializedSpawner.ApplyModifiedProperties();

            Debug.Log("✅ 已重新绑定 ItemPrefab!");
            EditorUtility.DisplayDialog("完成",
                "已重新绑定 ItemPrefab 到 ItemSpawner！\n\n" +
                "现在可以播放测试了。",
                "确定");
        }
    }
}
