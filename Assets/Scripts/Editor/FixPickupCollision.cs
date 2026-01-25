using UnityEngine;
using UnityEditor;

namespace XEscape.Editor
{
    /// <summary>
    /// 修复物品拾取碰撞问题
    /// </summary>
    public class FixPickupCollision : EditorWindow
    {
        [MenuItem("Tools/🔧 修复物品拾取问题")]
        public static void FixCollision()
        {
            if (EditorUtility.DisplayDialog("修复物品拾取",
                "这个工具会检查并修复以下问题：\n\n" +
                "1. Player的碰撞体必须是普通碰撞体（不勾选Is Trigger）\n" +
                "2. ItemPrefab的碰撞体必须是Trigger（勾选Is Trigger）\n" +
                "3. Player必须有Player标签\n" +
                "4. Ground必须有Ground标签\n\n" +
                "确定继续？",
                "修复", "取消"))
            {
                ExecuteFix();
            }
        }

        private static void ExecuteFix()
        {
            bool hasFixed = false;

            // 1. 检查并修复Player
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                // 确保Player有正确的标签
                if (!player.CompareTag("Player"))
                {
                    player.tag = "Player";
                    Debug.Log("✅ 已设置Player标签");
                    hasFixed = true;
                }

                // 检查Player的碰撞体
                BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
                if (playerCollider != null)
                {
                    if (playerCollider.isTrigger)
                    {
                        playerCollider.isTrigger = false;
                        Debug.Log("✅ 已修复Player碰撞体（设为普通碰撞体）");
                        hasFixed = true;
                    }
                    else
                    {
                        Debug.Log("✓ Player碰撞体配置正确（普通碰撞体）");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ Player没有BoxCollider2D组件！");
                }

                // 检查Player是否有Rigidbody2D
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb == null)
                {
                    playerRb = player.AddComponent<Rigidbody2D>();
                    playerRb.gravityScale = 0f;
                    playerRb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
                    Debug.Log("✅ 已添加Rigidbody2D到Player");
                    hasFixed = true;
                }
            }
            else
            {
                Debug.LogError("❌ 场景中没有Player对象！");
            }

            // 2. 检查并修复Ground
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                if (!ground.CompareTag("Ground"))
                {
                    ground.tag = "Ground";
                    Debug.Log("✅ 已设置Ground标签");
                    hasFixed = true;
                }
                else
                {
                    Debug.Log("✓ Ground标签正确");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ 场景中没有Ground对象");
            }

            // 3. 检查ItemPrefab
            string prefabPath = "Assets/Prefabs/ItemPrefab.prefab";
            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (itemPrefab != null)
            {
                BoxCollider2D itemCollider = itemPrefab.GetComponent<BoxCollider2D>();
                if (itemCollider != null)
                {
                    if (!itemCollider.isTrigger)
                    {
                        // 需要修改预制体
                        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
                        BoxCollider2D prefabCollider = prefabInstance.GetComponent<BoxCollider2D>();
                        if (prefabCollider != null)
                        {
                            prefabCollider.isTrigger = true;
                            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                            Debug.Log("✅ 已修复ItemPrefab碰撞体（设为Trigger）");
                            hasFixed = true;
                        }
                        PrefabUtility.UnloadPrefabContents(prefabInstance);
                    }
                    else
                    {
                        Debug.Log("✓ ItemPrefab碰撞体配置正确（Trigger）");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ ItemPrefab没有BoxCollider2D组件！");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到ItemPrefab预制体");
            }

            // 4. 检查场景中的物品实例
            XEscape.PickupScene.PickupItem[] items = Object.FindObjectsOfType<XEscape.PickupScene.PickupItem>();
            if (items.Length > 0)
            {
                foreach (var item in items)
                {
                    BoxCollider2D itemCollider = item.GetComponent<BoxCollider2D>();
                    if (itemCollider != null && !itemCollider.isTrigger)
                    {
                        itemCollider.isTrigger = true;
                        Debug.Log($"✅ 已修复场景中的物品 {item.name}（设为Trigger）");
                        hasFixed = true;
                    }
                }
            }

            // 显示结果
            if (hasFixed)
            {
                EditorUtility.DisplayDialog("修复完成",
                    "✅ 碰撞配置已修复！\n\n" +
                    "配置说明:\n" +
                    "• Player: 普通碰撞体（Is Trigger = false）\n" +
                    "• ItemPrefab: Trigger碰撞体（Is Trigger = true）\n" +
                    "• Player标签: Player\n" +
                    "• Ground标签: Ground\n\n" +
                    "现在可以测试拾取功能了！",
                    "开始测试");
            }
            else
            {
                EditorUtility.DisplayDialog("检查完成",
                    "所有配置都是正确的！\n\n" +
                    "如果拾取还不work，请检查:\n" +
                    "1. Player是否绑定了InventoryManager\n" +
                    "2. 运行时Console中是否有报错\n" +
                    "3. Player和Item是否真的碰撞了（检查碰撞体大小）",
                    "确定");
            }
        }
    }
}
