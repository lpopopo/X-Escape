using UnityEngine;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 玩家控制器 - 处理左右移动和物资拾取
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveRangeX = 8f; // 移动范围限制

        [Header("背包管理")]
        [SerializeField] private InventoryManager inventoryManager;

        private Rigidbody2D rb;
        private float horizontalInput;
        private bool isPickupEnabled = true; // 是否允许拾取

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // 玩家不受重力影响
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            
            // 确保 Player Tag 被正确设置
            if (!gameObject.CompareTag("Player"))
            {
                // 尝试设置 Player tag
                try
                {
                    gameObject.tag = "Player";
                    Debug.Log($"[PlayerController] 已自动设置 Player Tag");
                }
                catch
                {
                    Debug.LogWarning($"[PlayerController] 无法设置 Player Tag，请确保在 TagManager 中已创建 Player 标签");
                }
            }
        }

        private void Update()
        {
            // 获取输入
            horizontalInput = Input.GetAxisRaw("Horizontal");

            // 也支持A/D键
            if (Input.GetKey(KeyCode.A))
            {
                horizontalInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1f;
            }
        }

        private void FixedUpdate()
        {
            // 移动
            MovePlayer();
        }

        private void MovePlayer()
        {
            // 计算新位置
            Vector2 velocity = new Vector2(horizontalInput * moveSpeed, 0f);
            rb.linearVelocity = velocity;

            // 限制移动范围
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, -moveRangeX / 2, moveRangeX / 2);
            transform.position = position;
        }

        /// <summary>
        /// 设置是否允许拾取
        /// </summary>
        public void SetPickupEnabled(bool enabled)
        {
            isPickupEnabled = enabled;
            Debug.Log($"[PlayerController] 拾取功能已{(enabled ? "启用" : "禁用")}");
        }

        /// <summary>
        /// 拾取物资
        /// </summary>
        /// <returns>是否成功拾取</returns>
        public bool PickupItem(PickupItem item)
        {
            Debug.Log($"[PlayerController] PickupItem 被调用! 物品类型: {item.itemType}");

            // 如果拾取已禁用，直接返回
            if (!isPickupEnabled)
            {
                Debug.Log("[PlayerController] 拾取功能已禁用，无法拾取物品");
                return false;
            }

            if (inventoryManager != null)
            {
                Debug.Log($"[PlayerController] inventoryManager 存在，调用AddItem");
                bool success = inventoryManager.AddItem(item.itemType, item.itemValue);
                if (success)
                {
                    Debug.Log($"✅ 拾取了 {item.itemType} x {item.itemValue}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 拾取失败! 背包可能已满");
                }
                return success;
            }
            else
            {
                Debug.LogError($"❌ [PlayerController] inventoryManager 是 null! 需要在Inspector中绑定!");
                return false;
            }
        }

        private void OnDrawGizmos()
        {
            // 在编辑器中绘制移动范围
            Gizmos.color = Color.cyan;
            Vector3 leftBound = new Vector3(-moveRangeX / 2, transform.position.y, 0);
            Vector3 rightBound = new Vector3(moveRangeX / 2, transform.position.y, 0);
            Gizmos.DrawLine(leftBound, rightBound);
        }
    }
}
