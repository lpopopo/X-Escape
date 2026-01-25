using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 背包UI显示
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Transform inventoryPanel;
        [SerializeField] private GameObject slotPrefab;

        [Header("背包管理器")]
        [SerializeField] private InventoryManager inventoryManager;

        private List<GameObject> slotObjects = new List<GameObject>();

        private void Start()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged += UpdateUI;
                InitializeUI();
            }
        }

        private void OnDestroy()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged -= UpdateUI;
            }
        }

        private void InitializeUI()
        {
            // 创建UI槽位
            var inventory = inventoryManager.GetInventory();
            for (int i = 0; i < inventory.Count; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, inventoryPanel);
                slotObjects.Add(slotObj);

                // 添加点击事件
                int index = i;
                Button button = slotObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnSlotClicked(index));
                }
            }

            UpdateUI(inventory);
        }

        private void UpdateUI(List<InventoryManager.InventorySlot> inventory)
        {
            for (int i = 0; i < slotObjects.Count && i < inventory.Count; i++)
            {
                UpdateSlot(slotObjects[i], inventory[i]);
            }
        }

        private void UpdateSlot(GameObject slotObj, InventoryManager.InventorySlot slot)
        {
            // 更新槽位显示
            Image bgImage = slotObj.GetComponent<Image>();
            TextMeshProUGUI text = slotObj.GetComponentInChildren<TextMeshProUGUI>();

            if (slot.isEmpty)
            {
                if (bgImage != null) bgImage.color = Color.gray;
                if (text != null) text.text = "空";
            }
            else
            {
                // 根据物品类型设置颜色
                if (bgImage != null)
                {
                    switch (slot.itemType)
                    {
                        case ItemType.Food:
                            bgImage.color = Color.green;
                            break;
                        case ItemType.Fuel:
                            bgImage.color = Color.yellow;
                            break;
                        case ItemType.Medicine:
                            bgImage.color = Color.red;
                            break;
                    }
                }

                if (text != null)
                {
                    string itemName = GetItemName(slot.itemType);
                    text.text = $"{itemName}\n{slot.amount:F0}";
                }
            }
        }

        private string GetItemName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Food:
                    return "食物";
                case ItemType.Fuel:
                    return "油料";
                case ItemType.Medicine:
                    return "药品";
                default:
                    return "未知";
            }
        }

        private void OnSlotClicked(int slotIndex)
        {
            if (inventoryManager != null)
            {
                inventoryManager.UseItem(slotIndex);
            }
        }
    }
}
