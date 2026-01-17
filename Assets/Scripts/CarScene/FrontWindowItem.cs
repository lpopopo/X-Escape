using UnityEngine;
using UnityEngine.Events;

namespace XEscape.CarScene
{
    /// <summary>
    /// 车前窗物品，可以点击互动
    /// </summary>
    public class FrontWindowItem : MonoBehaviour
    {
        [Header("物品信息")]
        [SerializeField] private string itemName;
        [SerializeField] private string itemDescription;
        [SerializeField] private Sprite itemIcon;

        [Header("交互设置")]
        [SerializeField] private bool canInteract = true;
        // interactionRange 保留用于未来功能扩展
        // [SerializeField] private float interactionRange = 2f; // 交互范围（如果需要）

        [Header("事件")]
        [SerializeField] private UnityEvent onItemClicked; // 点击事件
        [SerializeField] private UnityEvent onItemHoverEnter; // 鼠标悬停进入
        [SerializeField] private UnityEvent onItemHoverExit; // 鼠标悬停离开

        [Header("视觉反馈")]
        [SerializeField] private Color hoverColor = Color.yellow;
        [SerializeField] private float hoverScale = 1.1f;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Vector3 originalScale;
        private bool isHovering = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            originalScale = transform.localScale;

            // 确保有Collider2D用于检测点击
            if (GetComponent<Collider2D>() == null)
            {
                BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
            }
        }

        private void OnMouseEnter()
        {
            if (!canInteract) return;

            isHovering = true;
            OnHoverEnter();
            onItemHoverEnter?.Invoke();
        }

        private void OnMouseExit()
        {
            if (!isHovering) return;

            isHovering = false;
            OnHoverExit();
            onItemHoverExit?.Invoke();
        }

        private void OnMouseDown()
        {
            if (!canInteract) return;

            OnItemClicked();
            onItemClicked?.Invoke();
        }

        /// <summary>
        /// 鼠标悬停进入时的视觉反馈
        /// </summary>
        private void OnHoverEnter()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = hoverColor;
            }
            transform.localScale = originalScale * hoverScale;
        }

        /// <summary>
        /// 鼠标悬停离开时的视觉反馈
        /// </summary>
        private void OnHoverExit()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            transform.localScale = originalScale;
        }

        /// <summary>
        /// 物品被点击时的处理
        /// </summary>
        private void OnItemClicked()
        {
            Debug.Log($"车前窗物品被点击: {itemName}");
            // 这里可以添加具体的交互逻辑
            // 例如：拾取物品、打开详情面板等
        }

        /// <summary>
        /// 设置物品信息
        /// </summary>
        public void SetItemInfo(string name, string description, Sprite icon = null)
        {
            itemName = name;
            itemDescription = description;
            if (icon != null)
            {
                itemIcon = icon;
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = icon;
                }
            }
        }

        /// <summary>
        /// 设置是否可以交互
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }

        /// <summary>
        /// 获取物品名称
        /// </summary>
        public string GetItemName()
        {
            return itemName;
        }

        /// <summary>
        /// 获取物品描述
        /// </summary>
        public string GetItemDescription()
        {
            return itemDescription;
        }
    }
}
