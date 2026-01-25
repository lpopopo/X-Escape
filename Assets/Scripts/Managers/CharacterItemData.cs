using System.Collections.Generic;
using UnityEngine;

namespace XEscape.Managers
{
    /// <summary>
    /// 角色物品数据（不依赖 GameObject 状态）
    /// </summary>
    [System.Serializable]
    public class CharacterItemData
    {
        public string characterName;
        public bool hasFood;
        public bool hasDisguise;
        
        public CharacterItemData(string name)
        {
            characterName = name;
            hasFood = false;
            hasDisguise = false;
        }
    }
    
    /// <summary>
    /// 全局物品数据管理器（单例，不依赖 GameObject 激活状态）
    /// </summary>
    public class CharacterItemDataManager : MonoBehaviour
    {
        private static CharacterItemDataManager instance;
        
        // 使用字典存储每个角色的物品数据（key: 角色名称）
        private Dictionary<string, CharacterItemData> characterItems = new Dictionary<string, CharacterItemData>();
        
        public static CharacterItemDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CharacterItemDataManager");
                    instance = go.AddComponent<CharacterItemDataManager>();
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
        /// 获取或创建角色数据
        /// </summary>
        private CharacterItemData GetOrCreateCharacterData(string characterName)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                characterItems[characterName] = new CharacterItemData(characterName);
            }
            return characterItems[characterName];
        }
        
        /// <summary>
        /// 设置角色是否有食物
        /// </summary>
        public void SetHasFood(string characterName, bool hasFood)
        {
            CharacterItemData data = GetOrCreateCharacterData(characterName);
            data.hasFood = hasFood;
            Debug.Log($"CharacterItemDataManager: {characterName} hasFood = {hasFood}");
        }
        
        /// <summary>
        /// 设置角色是否有伪装物品
        /// </summary>
        public void SetHasDisguise(string characterName, bool hasDisguise)
        {
            CharacterItemData data = GetOrCreateCharacterData(characterName);
            data.hasDisguise = hasDisguise;
            Debug.Log($"CharacterItemDataManager: {characterName} hasDisguise = {hasDisguise}");
        }
        
        /// <summary>
        /// 获取角色是否有食物
        /// </summary>
        public bool HasFood(string characterName)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                return false;
            }
            return characterItems[characterName].hasFood;
        }
        
        /// <summary>
        /// 获取角色是否有伪装物品
        /// </summary>
        public bool HasDisguise(string characterName)
        {
            if (!characterItems.ContainsKey(characterName))
            {
                return false;
            }
            return characterItems[characterName].hasDisguise;
        }
        
        /// <summary>
        /// 消耗角色的食物
        /// </summary>
        public void ConsumeFood(string characterName)
        {
            SetHasFood(characterName, false);
        }
        
        /// <summary>
        /// 消耗角色的伪装物品
        /// </summary>
        public void ConsumeDisguise(string characterName)
        {
            SetHasDisguise(characterName, false);
        }
        
        /// <summary>
        /// 清除所有数据（用于重置）
        /// </summary>
        public void ClearAll()
        {
            characterItems.Clear();
        }
    }
}
