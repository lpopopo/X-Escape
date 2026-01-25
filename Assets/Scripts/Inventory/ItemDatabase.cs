using UnityEngine;
using System.Collections.Generic;

namespace XEscape.Inventory
{
    /// <summary>
    /// 物品数据库，用于创建预设物品
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "X-Escape/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [Header("食物物品")]
        public List<FoodItem> foodItems = new List<FoodItem>();

        [Header("伪装物品")]
        public List<DisguiseItem> disguiseItems = new List<DisguiseItem>();

        /// <summary>
        /// 根据ID获取食物
        /// </summary>
        public FoodItem GetFoodById(string id)
        {
            return foodItems.Find(f => f.itemId == id);
        }

        /// <summary>
        /// 根据名称获取食物
        /// </summary>
        public FoodItem GetFoodByName(string name)
        {
            return foodItems.Find(f => f.itemName == name);
        }

        /// <summary>
        /// 根据ID获取伪装物品
        /// </summary>
        public DisguiseItem GetDisguiseById(string id)
        {
            return disguiseItems.Find(d => d.itemId == id);
        }

        /// <summary>
        /// 根据名称获取伪装物品
        /// </summary>
        public DisguiseItem GetDisguiseByName(string name)
        {
            return disguiseItems.Find(d => d.itemName == name);
        }
    }
}
