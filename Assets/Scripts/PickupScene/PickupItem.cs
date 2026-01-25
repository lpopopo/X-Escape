using UnityEngine;
using TMPro;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 可拾取物资
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PickupItem : MonoBehaviour
    {
        [Header("物资属性")]
        public ItemType itemType;
        public float itemValue = 20f;

        [Header("UI显示")]
        [SerializeField] private TextMeshProUGUI itemText;

        private Rigidbody2D rb;
        private bool isPickedUp = false;

        [Header("销毁设置")]
        [SerializeField] private float destroyDelay = 5f; // 落地后5秒自动销毁

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;

            // 设置 Sorting Order，确保物资在 Ground 前面
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 5; // 在Player(1)之后，确保可见
            }
        }

        public void Initialize(ItemType type)
        {
            itemType = type;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            // 根据物资类型设置颜色和文字
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                switch (itemType)
                {
                    case ItemType.Food:
                        spriteRenderer.color = Color.green;
                        // 使用英文避免中文字体问题，或者可以移除文本显示
                        if (itemText != null) itemText.text = "Food";
                        break;
                    case ItemType.Fuel:
                        spriteRenderer.color = Color.yellow;
                        if (itemText != null) itemText.text = "Fuel";
                        break;
                    case ItemType.Medicine:
                        spriteRenderer.color = Color.red;
                        if (itemText != null) itemText.text = "Medicine";
                        break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 检测是否落地
            if (collision.gameObject.CompareTag("Ground"))
            {
                // 落地后开始计时销毁
                Invoke(nameof(DestroyItem), destroyDelay);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isPickedUp) return;

            // 检测是否被玩家拾取 - 优先检查 PlayerController 组件，这样即使 Tag 未设置也能工作
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"[PickupItem] 检测到PlayerController组件，开始拾取 {itemType}");
                bool success = player.PickupItem(this);
                if (success)
                {
                    isPickedUp = true;
                    Destroy(gameObject);
                    Debug.Log($"[PickupItem] 物品已销毁");
                }
                else
                {
                    Debug.LogWarning($"[PickupItem] 拾取失败，物品保留（可能背包已满）");
                    // 提示消息已通过InventoryManager的事件系统显示，这里不需要额外处理
                }
            }
            // 如果 Tag 是 Player 但没有 PlayerController，给出警告
            else if (other.CompareTag("Player"))
            {
                Debug.LogWarning($"[PickupItem] 碰撞对象有Player标签但没有PlayerController组件! 对象: {other.name}");
            }
            // 其他情况不输出日志，避免日志过多
        }

        private void DestroyItem()
        {
            if (!isPickedUp)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 物资类型枚举
    /// </summary>
    public enum ItemType
    {
        Food,       // 食物 - 恢复体力
        Fuel,       // 油料 - 恢复油量
        Medicine    // 药品 - 大幅恢复体力
    }
}
