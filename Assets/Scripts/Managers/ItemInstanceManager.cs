using System.Collections.Generic;
using UnityEngine;

namespace XEscape.Managers
{
    /// <summary>
    /// 全局物品实例管理器（单例，不依赖 GameObject 激活状态）
    /// 管理所有角色身上的物品 GameObject 引用
    /// </summary>
    public class ItemInstanceManager : MonoBehaviour
    {
        private static ItemInstanceManager instance;
        
        // 存储每个角色身上的物品 GameObject 引用
        // Key: 角色名称, Value: (食物GameObject, 伪装物品GameObject, 食物取消按钮, 伪装取消按钮)
        private Dictionary<string, (GameObject foodItem, GameObject disguiseItem, GameObject foodCancelButton, GameObject disguiseCancelButton)> characterItems = 
            new Dictionary<string, (GameObject, GameObject, GameObject, GameObject)>();
        
        public static ItemInstanceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("ItemInstanceManager");
                    instance = go.AddComponent<ItemInstanceManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 注册角色的食物物品
        /// </summary>
        public void RegisterFoodItem(string characterName, GameObject foodItem, GameObject cancelButton)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                characterItems[characterName] = (null, null, null, null);
            }
            
            var items = characterItems[characterName];
            characterItems[characterName] = (foodItem, items.disguiseItem, cancelButton, items.disguiseCancelButton);
            Debug.Log($"ItemInstanceManager: {characterName} 注册食物物品: {foodItem?.name}");
        }
        
        /// <summary>
        /// 注册角色的伪装物品
        /// </summary>
        public void RegisterDisguiseItem(string characterName, GameObject disguiseItem, GameObject cancelButton)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                characterItems[characterName] = (null, null, null, null);
            }
            
            var items = characterItems[characterName];
            characterItems[characterName] = (items.foodItem, disguiseItem, items.foodCancelButton, cancelButton);
            Debug.Log($"ItemInstanceManager: {characterName} 注册伪装物品: {disguiseItem?.name}");
        }
        
        /// <summary>
        /// 取消注册角色的食物物品
        /// </summary>
        public void UnregisterFoodItem(string characterName)
        {
            if (characterItems.ContainsKey(characterName))
            {
                var items = characterItems[characterName];
                characterItems[characterName] = (null, items.disguiseItem, null, items.disguiseCancelButton);
            }
        }
        
        /// <summary>
        /// 取消注册角色的伪装物品
        /// </summary>
        public void UnregisterDisguiseItem(string characterName)
        {
            if (characterItems.ContainsKey(characterName))
            {
                var items = characterItems[characterName];
                characterItems[characterName] = (items.foodItem, null, items.foodCancelButton, null);
            }
        }
        
        /// <summary>
        /// 清理角色身上的所有物品 GameObject（结算时调用）
        /// </summary>
        public void ClearCharacterItems(string characterName)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                Debug.Log($"ItemInstanceManager: {characterName} 没有注册的物品");
                return;
            }
            
            var items = characterItems[characterName];
            int destroyedCount = 0;
            
            // 销毁食物物品
            if (items.foodItem != null)
            {
                Debug.Log($"ItemInstanceManager: 销毁 {characterName} 的食物物品: {items.foodItem.name}");
                Destroy(items.foodItem);
                destroyedCount++;
            }
            
            // 销毁伪装物品
            if (items.disguiseItem != null)
            {
                Debug.Log($"ItemInstanceManager: 销毁 {characterName} 的伪装物品: {items.disguiseItem.name}");
                Destroy(items.disguiseItem);
                destroyedCount++;
            }
            
            // 销毁食物取消按钮
            if (items.foodCancelButton != null)
            {
                Debug.Log($"ItemInstanceManager: 销毁 {characterName} 的食物取消按钮: {items.foodCancelButton.name}");
                Destroy(items.foodCancelButton);
                destroyedCount++;
            }
            
            // 销毁伪装取消按钮
            if (items.disguiseCancelButton != null)
            {
                Debug.Log($"ItemInstanceManager: 销毁 {characterName} 的伪装取消按钮: {items.disguiseCancelButton.name}");
                Destroy(items.disguiseCancelButton);
                destroyedCount++;
            }
            
            // 清除注册
            characterItems.Remove(characterName);
            
            Debug.Log($"ItemInstanceManager: {characterName} 清理完成，共销毁 {destroyedCount} 个对象");
        }
        
        /// <summary>
        /// 清理所有角色的物品（用于重置）
        /// </summary>
        public void ClearAllItems()
        {
            List<string> characterNames = new List<string>(characterItems.Keys);
            foreach (var name in characterNames)
            {
                ClearCharacterItems(name);
            }
        }
        
        /// <summary>
        /// 获取角色的食物物品（用于检查）
        /// </summary>
        public GameObject GetFoodItem(string characterName)
        {
            if (characterItems.ContainsKey(characterName))
            {
                return characterItems[characterName].foodItem;
            }
            return null;
        }
        
        /// <summary>
        /// 获取角色的伪装物品（用于检查）
        /// </summary>
        public GameObject GetDisguiseItem(string characterName)
        {
            if (characterItems.ContainsKey(characterName))
            {
                return characterItems[characterName].disguiseItem;
            }
            return null;
        }
    }
}
