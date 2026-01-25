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
                        if (itemText != null) itemText.text = "食物";
                        break;
                    case ItemType.Fuel:
                        spriteRenderer.color = Color.yellow;
                        if (itemText != null) itemText.text = "油料";
                        break;
                    case ItemType.Medicine:
                        spriteRenderer.color = Color.red;
                        if (itemText != null) itemText.text = "药品";
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
            Debug.Log($"[PickupItem] OnTriggerEnter2D 被触发! 碰撞对象: {other.name}, Tag: {other.tag}");

            if (isPickedUp) return;

            // 检测是否被玩家拾取
            if (other.CompareTag("Player"))
            {
                Debug.Log($"[PickupItem] 检测到Player标签!");
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    Debug.Log($"[PickupItem] 找到PlayerController，开始拾取 {itemType}");
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
                    }
                }
                else
                {
                    Debug.LogError($"[PickupItem] Player对象上没有PlayerController组件!");
                }
            }
            else
            {
                Debug.Log($"[PickupItem] 碰撞对象不是Player，Tag是: {other.tag}");
            }
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
