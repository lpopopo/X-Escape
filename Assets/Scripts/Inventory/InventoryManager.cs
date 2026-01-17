using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using XEscape.Managers;
using XEscape.CarScene;

namespace XEscape.Inventory
{
    /// <summary>
    /// 背包管理器
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("背包设置")]
        [SerializeField] private int maxSlots = 20; // 最大背包槽位

        [Header("调试")]
        [SerializeField] private bool enableDebugLog = false;

        private List<Item> items = new List<Item>(); // 物品列表

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (enableDebugLog)
            {
                Debug.Log($"InventoryManager: 初始化完成，最大槽位: {maxSlots}");
            }
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        public bool AddItem(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("InventoryManager: 尝试添加空物品");
                return false;
            }

            // 检查是否有空位
            if (items.Count >= maxSlots && !CanStackItem(item))
            {
                Debug.LogWarning($"InventoryManager: 背包已满！当前物品数: {items.Count}/{maxSlots}");
                return false;
            }

            // 尝试堆叠
            if (item.CanStack())
            {
                Item existingItem = items.FirstOrDefault(i => 
                    i.itemName == item.itemName && 
                    i.itemType == item.itemType &&
                    i.CanStackWith(item.quantity));

                if (existingItem != null)
                {
                    // 可以堆叠
                    int spaceLeft = existingItem.maxStack - existingItem.quantity;
                    if (spaceLeft >= item.quantity)
                    {
                        existingItem.quantity += item.quantity;
                        if (enableDebugLog)
                            Debug.Log($"InventoryManager: 堆叠物品 {item.itemName}，数量: {existingItem.quantity}");
                        return true;
                    }
                    else
                    {
                        // 部分堆叠
                        existingItem.quantity = existingItem.maxStack;
                        item.quantity -= spaceLeft;
                        if (enableDebugLog)
                            Debug.Log($"InventoryManager: 部分堆叠 {item.itemName}，剩余: {item.quantity}");
                    }
                }
            }

            // 添加新物品
            if (items.Count < maxSlots)
            {
                items.Add(item.Clone());
                if (enableDebugLog)
                    Debug.Log($"InventoryManager: 添加物品 {item.itemName}，数量: {item.quantity}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            Item item = items.FirstOrDefault(i => i.itemId == itemId);
            if (item == null)
                return false;

            if (item.quantity <= quantity)
            {
                items.Remove(item);
                if (enableDebugLog)
                    Debug.Log($"InventoryManager: 移除物品 {item.itemName}");
            }
            else
            {
                item.quantity -= quantity;
                if (enableDebugLog)
                    Debug.Log($"InventoryManager: 减少物品 {item.itemName}，剩余: {item.quantity}");
            }

            return true;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        public bool UseItem(string itemId)
        {
            Item item = items.FirstOrDefault(i => i.itemId == itemId);
            if (item == null)
            {
                Debug.LogWarning($"InventoryManager: 物品不存在: {itemId}");
                return false;
            }

            bool used = false;

            // 根据物品类型执行不同的使用逻辑
            switch (item.itemType)
            {
                case ItemType.Food:
                    used = UseFoodItem(item as FoodItem);
                    break;
                case ItemType.Disguise:
                    used = UseDisguiseItem(item as DisguiseItem);
                    break;
                default:
                    Debug.LogWarning($"InventoryManager: 物品类型 {item.itemType} 暂不支持使用");
                    break;
            }

            if (used)
            {
                // 使用后减少数量
                RemoveItem(itemId, 1);
            }

            return used;
        }

        /// <summary>
        /// 使用食物
        /// </summary>
        private bool UseFoodItem(FoodItem food)
        {
            if (food == null) return false;

            // 恢复饱腹度
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            foreach (CarOccupant occupant in occupants)
            {
                float currentSatiety = occupant.GetSatiety();
                float newSatiety = Mathf.Min(100f, currentSatiety + food.satietyRestore);
                occupant.SetSatiety(newSatiety);

                if (enableDebugLog)
                    Debug.Log($"InventoryManager: {occupant.GetName()} 使用 {food.itemName}，饱腹度: {currentSatiety} -> {newSatiety}");
            }

            // 恢复体力（如果有ResourceManager）
            if (GameManager.Instance != null && GameManager.Instance.resourceManager != null)
            {
                if (food.staminaRestore > 0)
                {
                    GameManager.Instance.resourceManager.RestoreStamina(food.staminaRestore);
                }
            }

            return true;
        }

        /// <summary>
        /// 使用伪装物品
        /// </summary>
        private bool UseDisguiseItem(DisguiseItem disguise)
        {
            if (disguise == null) return false;

            // 增加伪装度
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            foreach (CarOccupant occupant in occupants)
            {
                float currentDisguise = occupant.GetDisguise();
                float newDisguise = Mathf.Min(100f, currentDisguise + disguise.disguiseBonus);
                occupant.SetDisguise(newDisguise);

                if (enableDebugLog)
                    Debug.Log($"InventoryManager: {occupant.GetName()} 使用 {disguise.itemName}，伪装度: {currentDisguise} -> {newDisguise}");
            }

            return true;
        }

        /// <summary>
        /// 检查是否可以堆叠物品
        /// </summary>
        private bool CanStackItem(Item item)
        {
            if (!item.CanStack()) return false;
            return items.Any(i => i.itemName == item.itemName && i.itemType == item.itemType && i.CanStackWith(item.quantity));
        }

        /// <summary>
        /// 获取所有物品
        /// </summary>
        public List<Item> GetAllItems()
        {
            return new List<Item>(items);
        }

        /// <summary>
        /// 获取指定类型的物品
        /// </summary>
        public List<Item> GetItemsByType(ItemType type)
        {
            return items.Where(i => i.itemType == type).ToList();
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        public int GetItemCount()
        {
            return items.Count;
        }

        /// <summary>
        /// 获取物品总数量（包括堆叠）
        /// </summary>
        public int GetTotalItemCount()
        {
            return items.Sum(i => i.quantity);
        }

        /// <summary>
        /// 检查是否有空位
        /// </summary>
        public bool HasEmptySlot()
        {
            return items.Count < maxSlots;
        }

        /// <summary>
        /// 清空背包（用于测试）
        /// </summary>
        [ContextMenu("清空背包")]
        public void ClearInventory()
        {
            items.Clear();
            Debug.Log("InventoryManager: 背包已清空");
        }
    }
}
