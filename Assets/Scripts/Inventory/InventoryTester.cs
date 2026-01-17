using UnityEngine;
using XEscape.Inventory;

namespace XEscape.Inventory
{
    /// <summary>
    /// 背包系统测试脚本，用于快速测试背包功能
    /// </summary>
    public class InventoryTester : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool addTestItemsOnStart = true;

        private void Start()
        {
            if (addTestItemsOnStart)
            {
                AddTestItems();
            }
        }

        /// <summary>
        /// 添加测试物品
        /// </summary>
        [ContextMenu("添加测试物品")]
        public void AddTestItems()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogError("InventoryTester: InventoryManager 未找到！请确保场景中有 InventoryManager GameObject。");
                return;
            }

            // 添加食物
            FoodItem bread = new FoodItem
            {
                itemName = "面包",
                description = "恢复20点饱腹度",
                foodType = FoodType.Bread,
                satietyRestore = 20f,
                quantity = 5,
                maxStack = 10
            };

            FoodItem water = new FoodItem
            {
                itemName = "水",
                description = "恢复10点饱腹度",
                foodType = FoodType.Water,
                satietyRestore = 10f,
                quantity = 3,
                maxStack = 10
            };

            FoodItem cannedFood = new FoodItem
            {
                itemName = "罐头",
                description = "恢复30点饱腹度",
                foodType = FoodType.CannedFood,
                satietyRestore = 30f,
                quantity = 2,
                maxStack = 10
            };

            // 添加伪装物品
            DisguiseItem hat = new DisguiseItem
            {
                itemName = "帽子",
                description = "增加15点伪装度",
                disguiseType = DisguiseType.Hat,
                disguiseBonus = 15f,
                quantity = 1
            };

            DisguiseItem glasses = new DisguiseItem
            {
                itemName = "眼镜",
                description = "增加10点伪装度",
                disguiseType = DisguiseType.Glasses,
                disguiseBonus = 10f,
                quantity = 1
            };

            DisguiseItem mask = new DisguiseItem
            {
                itemName = "面具",
                description = "增加25点伪装度",
                disguiseType = DisguiseType.Mask,
                disguiseBonus = 25f,
                quantity = 1
            };

            // 添加到背包
            InventoryManager.Instance.AddItem(bread);
            InventoryManager.Instance.AddItem(water);
            InventoryManager.Instance.AddItem(cannedFood);
            InventoryManager.Instance.AddItem(hat);
            InventoryManager.Instance.AddItem(glasses);
            InventoryManager.Instance.AddItem(mask);

            Debug.Log($"InventoryTester: 已添加 {6} 个测试物品到背包");
        }

        /// <summary>
        /// 清空背包
        /// </summary>
        [ContextMenu("清空背包")]
        public void ClearInventory()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ClearInventory();
                Debug.Log("InventoryTester: 背包已清空");
            }
        }
    }
}
