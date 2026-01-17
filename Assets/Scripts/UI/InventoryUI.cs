using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XEscape.Inventory;

namespace XEscape.UI
{
    /// <summary>
    /// 背包UI显示
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemDescriptionText;
        [SerializeField] private Button useButton;
        [SerializeField] private Button closeButton;

        [Header("设置")]
        // 以下字段保留用于未来功能扩展
        // [SerializeField] private int slotsPerRow = 5;
        // [SerializeField] private float slotSpacing = 10f;

        private List<InventorySlot> slots = new List<InventorySlot>();
        private Item selectedItem;

        [Header("调试")]
        [SerializeField] private bool startVisible = false; // 启动时是否可见（用于测试）

        private void Awake()
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(startVisible);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseInventory);

            if (useButton != null)
                useButton.onClick.AddListener(UseSelectedItem);
        }

        private void Start()
        {
            InitializeSlots();
        }

        /// <summary>
        /// 初始化背包槽位
        /// </summary>
        private void InitializeSlots()
        {
            if (slotsParent == null || slotPrefab == null)
            {
                Debug.LogWarning("InventoryUI: slotsParent 或 slotPrefab 未设置！");
                return;
            }

            // 清除现有槽位
            foreach (Transform child in slotsParent)
            {
                Destroy(child.gameObject);
            }
            slots.Clear();

            // 创建槽位
            int maxSlots = InventoryManager.Instance != null ? 20 : 20; // 默认20个槽位
            for (int i = 0; i < maxSlots; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsParent);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                if (slot == null)
                    slot = slotObj.AddComponent<InventorySlot>();

                slot.Initialize(i, this);
                slots.Add(slot);
            }

            Debug.Log($"InventoryUI: 初始化了 {slots.Count} 个背包槽位");
        }

        /// <summary>
        /// 打开背包
        /// </summary>
        public void OpenInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
                RefreshInventory();
            }
        }

        /// <summary>
        /// 关闭背包
        /// </summary>
        public void CloseInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
                selectedItem = null;
            }
        }

        /// <summary>
        /// 刷新背包显示
        /// </summary>
        public void RefreshInventory()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryUI: InventoryManager 未找到！");
                return;
            }

            List<Item> items = InventoryManager.Instance.GetAllItems();

            // 更新所有槽位
            for (int i = 0; i < slots.Count; i++)
            {
                if (i < items.Count)
                {
                    slots[i].SetItem(items[i]);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
        }

        /// <summary>
        /// 选择物品
        /// </summary>
        public void SelectItem(Item item)
        {
            selectedItem = item;

            if (itemNameText != null)
                itemNameText.text = item != null ? item.itemName : "";

            if (itemDescriptionText != null)
                itemDescriptionText.text = item != null ? item.description : "";

            if (useButton != null)
                useButton.interactable = item != null;
        }

        /// <summary>
        /// 使用选中的物品
        /// </summary>
        private void UseSelectedItem()
        {
            if (selectedItem == null || InventoryManager.Instance == null)
                return;

            bool used = InventoryManager.Instance.UseItem(selectedItem.itemId);
            if (used)
            {
                RefreshInventory();
                SelectItem(null);
            }
        }

        /// <summary>
        /// 切换背包显示/隐藏
        /// </summary>
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                if (inventoryPanel.activeSelf)
                    CloseInventory();
                else
                    OpenInventory();
            }
        }

        private void Update()
        {
            // 按 I 键打开/关闭背包
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }
    }
}
