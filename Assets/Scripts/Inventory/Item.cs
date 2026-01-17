using UnityEngine;

namespace XEscape.Inventory
{
    /// <summary>
    /// 物品基类
    /// </summary>
    [System.Serializable]
    public class Item
    {
        [Header("基本信息")]
        public string itemId;              // 物品唯一ID
        public string itemName;            // 物品名称
        public string description;         // 物品描述
        public ItemType itemType;          // 物品类型
        public Sprite icon;                // 物品图标
        public int maxStack = 1;           // 最大堆叠数量

        [Header("物品属性")]
        public int quantity = 1;           // 当前数量
        public int value = 0;              // 物品价值（可选）

        /// <summary>
        /// 构造函数
        /// </summary>
        public Item()
        {
            itemId = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 创建物品副本
        /// </summary>
        public virtual Item Clone()
        {
            Item clone = new Item
            {
                itemId = System.Guid.NewGuid().ToString(),
                itemName = this.itemName,
                description = this.description,
                itemType = this.itemType,
                icon = this.icon,
                maxStack = this.maxStack,
                quantity = this.quantity,
                value = this.value
            };
            return clone;
        }

        /// <summary>
        /// 是否可以堆叠
        /// </summary>
        public bool CanStack()
        {
            return maxStack > 1;
        }

        /// <summary>
        /// 是否可以堆叠指定数量的物品
        /// </summary>
        public bool CanStackWith(int amount)
        {
            return CanStack() && (quantity + amount) <= maxStack;
        }
    }
}
