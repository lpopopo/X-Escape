using UnityEngine;

namespace XEscape.Inventory
{
    /// <summary>
    /// 伪装物品
    /// </summary>
    [System.Serializable]
    public class DisguiseItem : Item
    {
        [Header("伪装属性")]
        public DisguiseType disguiseType;  // 伪装类型
        public float disguiseBonus;         // 伪装度加成（0-100）

        public DisguiseItem()
        {
            itemType = ItemType.Disguise;
            maxStack = 1; // 伪装物品通常不能堆叠
        }

        /// <summary>
        /// 使用伪装物品
        /// </summary>
        public void Use()
        {
            Debug.Log($"使用伪装物品: {itemName}, 伪装加成: {disguiseBonus}");
        }

        public override Item Clone()
        {
            DisguiseItem clone = new DisguiseItem
            {
                itemId = System.Guid.NewGuid().ToString(),
                itemName = this.itemName,
                description = this.description,
                itemType = this.itemType,
                icon = this.icon,
                maxStack = this.maxStack,
                quantity = this.quantity,
                value = this.value,
                disguiseType = this.disguiseType,
                disguiseBonus = this.disguiseBonus
            };
            return clone;
        }
    }
}
