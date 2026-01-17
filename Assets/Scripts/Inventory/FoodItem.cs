using UnityEngine;

namespace XEscape.Inventory
{
    /// <summary>
    /// 食物物品
    /// </summary>
    [System.Serializable]
    public class FoodItem : Item
    {
        [Header("食物属性")]
        public FoodType foodType;          // 食物类型
        public float satietyRestore;       // 恢复的饱腹度
        public float staminaRestore;       // 恢复的体力（可选）

        public FoodItem()
        {
            itemType = ItemType.Food;
            maxStack = 10; // 食物通常可以堆叠
        }

        /// <summary>
        /// 使用食物
        /// </summary>
        public void Use()
        {
            // 使用逻辑在InventoryManager中处理
            Debug.Log($"使用食物: {itemName}, 恢复饱腹度: {satietyRestore}");
        }

        public override Item Clone()
        {
            FoodItem clone = new FoodItem
            {
                itemId = System.Guid.NewGuid().ToString(),
                itemName = this.itemName,
                description = this.description,
                itemType = this.itemType,
                icon = this.icon,
                maxStack = this.maxStack,
                quantity = this.quantity,
                value = this.value,
                foodType = this.foodType,
                satietyRestore = this.satietyRestore,
                staminaRestore = this.staminaRestore
            };
            return clone;
        }
    }
}
