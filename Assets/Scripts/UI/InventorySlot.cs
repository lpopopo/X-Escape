using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XEscape.Inventory;

namespace XEscape.UI
{
    /// <summary>
    /// 背包槽位UI
    /// </summary>
    public class InventorySlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI组件")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Text quantityText;
        [SerializeField] private Image backgroundImage;

        [Header("设置")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private int slotIndex;
        private InventoryUI inventoryUI;
        private Item currentItem;
        private bool isSelected = false;

        /// <summary>
        /// 初始化槽位
        /// </summary>
        public void Initialize(int index, InventoryUI ui)
        {
            slotIndex = index;
            inventoryUI = ui;

            // 如果没有指定UI组件，尝试自动查找
            if (iconImage == null)
                iconImage = transform.Find("Icon")?.GetComponent<Image>();
            if (quantityText == null)
                quantityText = transform.Find("Quantity")?.GetComponent<Text>();
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();

            ClearSlot();
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        public void SetItem(Item item)
        {
            currentItem = item;

            if (iconImage != null)
            {
                if (item != null && item.icon != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.color = Color.white;
                    iconImage.enabled = true;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }

            if (quantityText != null)
            {
                if (item != null && item.quantity > 1)
                {
                    quantityText.text = item.quantity.ToString();
                    quantityText.enabled = true;
                }
                else
                {
                    quantityText.enabled = false;
                }
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = item != null ? normalColor : emptyColor;
            }
        }

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void ClearSlot()
        {
            currentItem = null;
            SetItem(null);
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            if (backgroundImage != null && currentItem != null)
            {
                backgroundImage.color = selected ? selectedColor : normalColor;
            }
        }

        /// <summary>
        /// 点击槽位
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentItem != null && inventoryUI != null)
            {
                inventoryUI.SelectItem(currentItem);
            }
        }

        /// <summary>
        /// 获取当前物品
        /// </summary>
        public Item GetItem()
        {
            return currentItem;
        }
    }
}
