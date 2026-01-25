using UnityEngine;
using System.Collections.Generic;
using System;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 背包管理器 - 管理拾取的物资
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("背包容量")]
        [SerializeField] private int maxSlots = 6;

        // 物品数据结构
        [System.Serializable]
        public class InventorySlot
        {
            public ItemType itemType;
            public float amount;
            public bool isEmpty = true;

            public InventorySlot()
            {
                isEmpty = true;
                amount = 0f;
            }
        }

        private List<InventorySlot> inventory = new List<InventorySlot>();

        // 事件 - 当背包更新时触发
        public event Action<List<InventorySlot>> OnInventoryChanged;

        private void Awake()
        {
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            inventory.Clear();
            for (int i = 0; i < maxSlots; i++)
            {
                inventory.Add(new InventorySlot());
            }
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        public bool AddItem(ItemType itemType, float amount)
        {
            // 尝试堆叠到现有物品
            foreach (var slot in inventory)
            {
                if (!slot.isEmpty && slot.itemType == itemType)
                {
                    slot.amount += amount;
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
            }

            // 找空位放置新物品
            foreach (var slot in inventory)
            {
                if (slot.isEmpty)
                {
                    slot.isEmpty = false;
                    slot.itemType = itemType;
                    slot.amount = amount;
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
            }

            Debug.Log("背包已满!");
            return false;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        public bool UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventory.Count)
                return false;

            var slot = inventory[slotIndex];
            if (slot.isEmpty)
                return false;

            // 应用物品效果
            ApplyItemEffect(slot.itemType, slot.amount);

            // 清空槽位
            slot.isEmpty = true;
            slot.amount = 0f;

            OnInventoryChanged?.Invoke(inventory);
            return true;
        }

        private void ApplyItemEffect(ItemType itemType, float amount)
        {
            var resourceManager = FindObjectOfType<XEscape.Managers.ResourceManager>();
            if (resourceManager == null)
            {
                Debug.LogWarning("未找到ResourceManager!");
                return;
            }

            switch (itemType)
            {
                case ItemType.Food:
                    resourceManager.RestoreStamina(amount);
                    Debug.Log($"使用食物，恢复体力 {amount}");
                    break;
                case ItemType.Fuel:
                    resourceManager.RestoreFuel(amount);
                    Debug.Log($"使用油料，恢复油量 {amount}");
                    break;
                case ItemType.Medicine:
                    resourceManager.RestoreStamina(amount * 2); // 药品效果更好
                    Debug.Log($"使用药品，恢复体力 {amount * 2}");
                    break;
            }
        }

        /// <summary>
        /// 获取背包内容
        /// </summary>
        public List<InventorySlot> GetInventory()
        {
            return inventory;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        public int GetItemCount(ItemType itemType)
        {
            int count = 0;
            foreach (var slot in inventory)
            {
                if (!slot.isEmpty && slot.itemType == itemType)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
