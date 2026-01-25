using UnityEngine;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 拾取系统调试器 - 运行时显示调试信息
    /// </summary>
    public class PickupDebugger : MonoBehaviour
    {
        [Header("调试设置")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool drawColliders = true;

        private PlayerController player;
        private InventoryManager inventoryManager;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            inventoryManager = FindObjectOfType<InventoryManager>();

            // 启动时检查配置
            CheckConfiguration();
        }

        private void CheckConfiguration()
        {
            Debug.Log("========== 拾取系统配置检查 ==========");

            // 检查Player
            if (player != null)
            {
                GameObject playerObj = player.gameObject;
                Debug.Log($"✓ 找到Player: {playerObj.name}");
                Debug.Log($"  - Tag: {playerObj.tag}");
                Debug.Log($"  - Layer: {LayerMask.LayerToName(playerObj.layer)}");

                BoxCollider2D collider = playerObj.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    Debug.Log($"  - BoxCollider2D: Size={collider.size}, IsTrigger={collider.isTrigger}");
                    if (collider.isTrigger)
                    {
                        Debug.LogError("  ❌ Player的碰撞体是Trigger！应该改为false！");
                    }
                }
                else
                {
                    Debug.LogError("  ❌ Player没有BoxCollider2D！");
                }

                Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Debug.Log($"  - Rigidbody2D: BodyType={rb.bodyType}, GravityScale={rb.gravityScale}");
                }
                else
                {
                    Debug.LogError("  ❌ Player没有Rigidbody2D！");
                }
            }
            else
            {
                Debug.LogError("❌ 未找到PlayerController！");
            }

            // 检查InventoryManager
            if (inventoryManager != null)
            {
                Debug.Log($"✓ 找到InventoryManager: {inventoryManager.gameObject.name}");

                // 检查Player是否绑定了InventoryManager
                if (player != null)
                {
                    var field = typeof(PlayerController).GetField("inventoryManager",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        var value = field.GetValue(player);
                        if (value != null)
                        {
                            Debug.Log("  ✓ Player已绑定InventoryManager");
                        }
                        else
                        {
                            Debug.LogError("  ❌ Player的inventoryManager字段为null！需要在Inspector中绑定！");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("❌ 未找到InventoryManager！");
            }

            // 检查Ground
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                Debug.Log($"✓ 找到Ground: Tag={ground.tag}");
                if (!ground.CompareTag("Ground"))
                {
                    Debug.LogWarning("  ⚠️ Ground的标签不是'Ground'！");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到Ground对象");
            }

            Debug.Log("====================================");
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 14;
            style.normal.textColor = Color.white;

            string info = "=== 拾取调试信息 ===\n";

            // Player信息
            if (player != null)
            {
                GameObject playerObj = player.gameObject;
                BoxCollider2D collider = playerObj.GetComponent<BoxCollider2D>();

                info += $"\n[Player]\n";
                info += $"位置: {playerObj.transform.position}\n";
                info += $"标签: {playerObj.tag}\n";

                if (collider != null)
                {
                    info += $"碰撞体: {collider.size}\n";
                    info += $"Is Trigger: {collider.isTrigger}";
                    if (collider.isTrigger)
                    {
                        info += " ❌应该是false!\n";
                    }
                    else
                    {
                        info += " ✓\n";
                    }
                }
            }

            // 物品信息
            PickupItem[] items = FindObjectsOfType<PickupItem>();
            info += $"\n[物品]\n";
            info += $"场景中物品数量: {items.Length}\n";

            if (items.Length > 0)
            {
                info += "\n最近的物品:\n";
                PickupItem nearest = null;
                float minDist = float.MaxValue;

                foreach (var item in items)
                {
                    if (player != null)
                    {
                        float dist = Vector3.Distance(item.transform.position, player.transform.position);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = item;
                        }
                    }
                }

                if (nearest != null)
                {
                    info += $"类型: {nearest.itemType}\n";
                    info += $"位置: {nearest.transform.position}\n";
                    info += $"距离: {minDist:F2}\n";

                    BoxCollider2D itemCollider = nearest.GetComponent<BoxCollider2D>();
                    if (itemCollider != null)
                    {
                        info += $"Is Trigger: {itemCollider.isTrigger}";
                        if (!itemCollider.isTrigger)
                        {
                            info += " ❌应该是true!\n";
                        }
                        else
                        {
                            info += " ✓\n";
                        }
                    }
                }
            }

            // 背包信息
            if (inventoryManager != null)
            {
                var inventory = inventoryManager.GetInventory();
                int usedSlots = 0;
                foreach (var slot in inventory)
                {
                    if (!slot.isEmpty) usedSlots++;
                }
                info += $"\n[背包]\n";
                info += $"已用格子: {usedSlots}/{inventory.Count}\n";
            }

            GUI.Box(new Rect(10, 10, 300, 350), info, style);
        }

        private void OnDrawGizmos()
        {
            if (!drawColliders) return;

            // 绘制Player的碰撞体
            if (player != null)
            {
                BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    Gizmos.color = Color.green;
                    Vector3 center = player.transform.position + (Vector3)collider.offset;
                    Gizmos.DrawWireCube(center, collider.size);
                }
            }

            // 绘制物品的碰撞体
            PickupItem[] items = FindObjectsOfType<PickupItem>();
            foreach (var item in items)
            {
                BoxCollider2D collider = item.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 center = item.transform.position + (Vector3)collider.offset;
                    Gizmos.DrawWireCube(center, collider.size);
                }
            }
        }
    }
}
