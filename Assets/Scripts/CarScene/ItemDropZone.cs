using System.Collections.Generic;
using UnityEngine;
using XEscape.Inventory;
using XEscape.Managers;

namespace XEscape.CarScene
{
    /// <summary>
    /// 角色身上的物品放置区域
    /// </summary>
    public class ItemDropZone : MonoBehaviour
    {
        [Header("关联角色")]
        [SerializeField] private CarOccupant occupant;
        
        [Header("物品显示位置")]
        [SerializeField] private Transform itemDisplayParent;
        
        // 当前持有的物品（使用 SerializeField 确保序列化）
        [SerializeField] private GameObject currentFoodItem;
        [SerializeField] private GameObject currentDisguiseItem;
        
        // 取消按钮
        private GameObject foodCancelButton;
        private GameObject disguiseCancelButton;
        
        private void Awake()
        {
            if (occupant == null)
            {
                occupant = GetComponentInParent<CarOccupant>();
            }
            
            if (itemDisplayParent == null)
            {
                GameObject displayParentObj = new GameObject("ItemDisplay");
                displayParentObj.transform.SetParent(transform);
                displayParentObj.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                itemDisplayParent = displayParentObj.transform;
            }
            
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.size = new Vector2(2f, 2f);
                boxCollider.isTrigger = true;
            }
        }
        
        /// <summary>
        /// 赋予物品给角色
        /// </summary>
        public bool AssignItem(ItemType itemType, GameObject itemObj)
        {
            if (occupant == null || occupant.IsDead())
            {
                Debug.LogWarning($"AssignItem 失败: occupant={occupant?.GetName()}, IsDead={occupant?.IsDead()}");
                return false;
            }
            
            if (itemType == ItemType.Food)
            {
                // 如果已有食物，拒绝赋予（需要先取消）
                if (currentFoodItem != null)
                {
                    Debug.LogWarning($"{occupant.GetName()}: 已有食物，无法赋予新食物");
                    return false;
                }
                
                // 设置新物品（上方）
                currentFoodItem = itemObj;
                Debug.Log($"{occupant.GetName()}: 赋予食物成功，currentFoodItem={currentFoodItem.name}, parent={itemDisplayParent?.name}");
                
                currentFoodItem.transform.SetParent(itemDisplayParent);
                currentFoodItem.transform.localPosition = new Vector3(0f, 0.8f, 0f);
                
                // 恢复渲染顺序
                SpriteRenderer sr = currentFoodItem.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10;
                
                // 禁用碰撞（防止被拖动检测）
                Collider2D[] colliders = currentFoodItem.GetComponentsInChildren<Collider2D>();
                foreach (var col in colliders) col.enabled = false;
                
                // 创建取消按钮
                foodCancelButton = CreateCancelButton(ItemType.Food);
                
                // 更新全局数据管理器
                CharacterItemDataManager.Instance.SetHasFood(occupant.GetName(), true);
                
                // 注册到全局物品实例管理器
                ItemInstanceManager.Instance.RegisterFoodItem(occupant.GetName(), currentFoodItem, foodCancelButton);
                
                Debug.Log($"{occupant.GetName()}: 食物赋予完成，HasFood()={HasFood()}");
                return true;
            }
            else if (itemType == ItemType.Disguise)
            {
                // 如果已有伪装物品，拒绝赋予（需要先取消）
                if (currentDisguiseItem != null)
                {
                    Debug.LogWarning($"{occupant.GetName()}: 已有伪装物品，无法赋予新伪装物品");
                    return false;
                }
                
                // 设置新物品（下方）
                currentDisguiseItem = itemObj;
                Debug.Log($"{occupant.GetName()}: 赋予伪装物品成功，currentDisguiseItem={currentDisguiseItem.name}, parent={itemDisplayParent?.name}");
                
                currentDisguiseItem.transform.SetParent(itemDisplayParent);
                currentDisguiseItem.transform.localPosition = new Vector3(0f, -0.3f, 0f);
                
                // 恢复渲染顺序
                SpriteRenderer sr = currentDisguiseItem.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10;
                
                // 禁用碰撞（防止被拖动检测）
                Collider2D[] colliders = currentDisguiseItem.GetComponentsInChildren<Collider2D>();
                foreach (var col in colliders) col.enabled = false;
                
                // 创建取消按钮
                disguiseCancelButton = CreateCancelButton(ItemType.Disguise);
                
                // 更新全局数据管理器
                CharacterItemDataManager.Instance.SetHasDisguise(occupant.GetName(), true);
                
                // 注册到全局物品实例管理器
                ItemInstanceManager.Instance.RegisterDisguiseItem(occupant.GetName(), currentDisguiseItem, disguiseCancelButton);
                
                Debug.Log($"{occupant.GetName()}: 伪装物品赋予完成，HasDisguise()={HasDisguise()}");
                return true;
            }
            
            return false;
        }
        
        private GameObject CreateCancelButton(ItemType itemType)
        {
            GameObject cancelButton = new GameObject("CancelButton");
            cancelButton.transform.SetParent(itemDisplayParent);
            cancelButton.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // 设置合适大小
            
            if (itemType == ItemType.Food)
            {
                // 食物取消按钮（右上）
                cancelButton.transform.localPosition = new Vector3(0.4f, 1.1f, -0.1f);
            }
            else
            {
                // 伪装物品取消按钮（右下）
                cancelButton.transform.localPosition = new Vector3(0.4f, 0f, -0.1f);
            }
            
            // 添加SpriteRenderer（红色X）
            SpriteRenderer cancelRenderer = cancelButton.AddComponent<SpriteRenderer>();
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            // 绘制X形状
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    // X的两条对角线
                    bool onDiagonal1 = Mathf.Abs(x - y) < 8;
                    bool onDiagonal2 = Mathf.Abs(x - (63 - y)) < 8;
                    if (onDiagonal1 || onDiagonal2)
                    {
                        pixels[y * 64 + x] = Color.red;
                    }
                    else
                    {
                        pixels[y * 64 + x] = Color.clear;
                    }
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            cancelRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
            cancelRenderer.sortingOrder = 200;
            
            // 添加Collider2D
            BoxCollider2D cancelCollider = cancelButton.AddComponent<BoxCollider2D>();
            cancelCollider.size = new Vector2(1f, 1f);
            cancelCollider.isTrigger = true;
            
            // 添加取消按钮脚本
            ItemCancelButton cancelScript = cancelButton.AddComponent<ItemCancelButton>();
            cancelScript.Initialize(this, itemType);
            
            return cancelButton;
        }
        
        /// <summary>
        /// 取消食物（移除并返回库存）
        /// </summary>
        public void CancelFoodItem()
        {
            string charName = occupant?.GetName();
            
            if (currentFoodItem != null)
            {
                Destroy(currentFoodItem);
                currentFoodItem = null;
            }
            
            if (foodCancelButton != null)
            {
                Destroy(foodCancelButton);
                foodCancelButton = null;
            }
            
            // 从全局物品实例管理器取消注册
            if (charName != null)
            {
                ItemInstanceManager.Instance.UnregisterFoodItem(charName);
                CharacterItemDataManager.Instance.SetHasFood(charName, false);
            }
            
            // 增加库存
            ItemManager itemManager = FindFirstObjectByType<ItemManager>();
            if (itemManager != null)
            {
                itemManager.IncreaseFoodStock();
            }
        }
        
        /// <summary>
        /// 取消伪装物品（移除并返回库存）
        /// </summary>
        public void CancelDisguiseItem()
        {
            string charName = occupant?.GetName();
            
            if (currentDisguiseItem != null)
            {
                Destroy(currentDisguiseItem);
                currentDisguiseItem = null;
            }
            
            if (disguiseCancelButton != null)
            {
                Destroy(disguiseCancelButton);
                disguiseCancelButton = null;
            }
            
            // 从全局物品实例管理器取消注册
            if (charName != null)
            {
                ItemInstanceManager.Instance.UnregisterDisguiseItem(charName);
                CharacterItemDataManager.Instance.SetHasDisguise(charName, false);
            }
            
            // 增加库存
            ItemManager itemManager = FindFirstObjectByType<ItemManager>();
            if (itemManager != null)
            {
                itemManager.IncreaseDisguiseStock();
            }
        }
        
        /// <summary>
        /// 刷新物品引用（从子对象中查找）
        /// </summary>
        private void RefreshItemReferences()
        {
            if (itemDisplayParent == null)
            {
                // 尝试查找 ItemDisplay 子对象（即使被禁用也能找到）
                Transform found = transform.Find("ItemDisplay");
                if (found != null)
                {
                    itemDisplayParent = found;
                }
                else
                {
                    Debug.LogWarning($"{occupant?.GetName()}: RefreshItemReferences - 找不到 ItemDisplay");
                    return;
                }
            }
            
            Debug.Log($"{occupant?.GetName()}: RefreshItemReferences - itemDisplayParent={itemDisplayParent.name}, childCount={itemDisplayParent.childCount}");
            
            // 遍历所有子对象，查找物品（即使被禁用也能找到）
            int childCount = itemDisplayParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = itemDisplayParent.GetChild(i);
                
                // 跳过取消按钮
                if (child.name == "CancelButton") continue;
                
                // 通过位置判断物品类型
                float yPos = child.localPosition.y;
                Debug.Log($"{occupant?.GetName()}: RefreshItemReferences - 检查子对象: {child.name}, y={yPos}");
                
                if (Mathf.Approximately(yPos, 0.8f))
                {
                    // 食物在上方
                    currentFoodItem = child.gameObject;
                    Debug.Log($"{occupant?.GetName()}: 通过位置恢复食物引用: {child.name}, y={yPos}");
                }
                else if (Mathf.Approximately(yPos, -0.3f))
                {
                    // 伪装物品在下方
                    currentDisguiseItem = child.gameObject;
                    Debug.Log($"{occupant?.GetName()}: 通过位置恢复伪装物品引用: {child.name}, y={yPos}");
                }
            }
        }
        
        /// <summary>
        /// 是否持有食物（优先使用数据管理器，不依赖 GameObject）
        /// </summary>
        public bool HasFood()
        {
            if (occupant == null) return false;
            
            // 优先使用全局数据管理器（不依赖 GameObject 状态）
            bool hasFood = CharacterItemDataManager.Instance.HasFood(occupant.GetName());
            
            // 同步 GameObject 状态（如果数据管理器说有，但 GameObject 没有，尝试恢复）
            if (hasFood && currentFoodItem == null)
            {
                RefreshItemReferences();
            }
            
            // 如果 GameObject 有但数据管理器没有，同步数据管理器
            if (!hasFood && currentFoodItem != null)
            {
                CharacterItemDataManager.Instance.SetHasFood(occupant.GetName(), true);
                hasFood = true;
            }
            
            Debug.Log($"{occupant.GetName()}: HasFood() - 数据管理器={hasFood}, GameObject={(currentFoodItem != null ? currentFoodItem.name : "null")}");
            return hasFood;
        }
        
        /// <summary>
        /// 是否持有伪装物品（优先使用数据管理器，不依赖 GameObject）
        /// </summary>
        public bool HasDisguise()
        {
            if (occupant == null) return false;
            
            // 优先使用全局数据管理器（不依赖 GameObject 状态）
            bool hasDisguise = CharacterItemDataManager.Instance.HasDisguise(occupant.GetName());
            
            // 同步 GameObject 状态（如果数据管理器说有，但 GameObject 没有，尝试恢复）
            if (hasDisguise && currentDisguiseItem == null)
            {
                RefreshItemReferences();
            }
            
            // 如果 GameObject 有但数据管理器没有，同步数据管理器
            if (!hasDisguise && currentDisguiseItem != null)
            {
                CharacterItemDataManager.Instance.SetHasDisguise(occupant.GetName(), true);
                hasDisguise = true;
            }
            
            Debug.Log($"{occupant.GetName()}: HasDisguise() - 数据管理器={hasDisguise}, GameObject={(currentDisguiseItem != null ? currentDisguiseItem.name : "null")}");
            return hasDisguise;
        }
        
        /// <summary>
        /// 递归查找子对象（即使被禁用也能找到）
        /// </summary>
        private Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent == null) return null;
            
            // 先检查直接子对象
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }
            
            // 递归查找
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Transform found = FindChildRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 清理所有物品 GameObject（结算时调用，确保所有物品都被清除）
        /// 注意：不更新数据管理器，数据管理器应该由 ConsumeFood/ConsumeDisguise 更新
        /// </summary>
        public void ClearAllItems()
        {
            string charName = occupant?.GetName() ?? "Unknown";
            Debug.Log($"{charName}: ClearAllItems 被调用 - 清理所有物品 GameObject");
            
            // 总是重新查找 ItemDisplay（不依赖缓存的引用，即使被禁用也能找到）
            Transform itemDisplay = FindChildRecursive(transform, "ItemDisplay");
            
            if (itemDisplay != null)
            {
                Debug.Log($"{charName}: 找到 ItemDisplay: {itemDisplay.name}, childCount={itemDisplay.childCount}, active={itemDisplay.gameObject.activeSelf}");
                itemDisplayParent = itemDisplay; // 更新引用
            }
            else
            {
                Debug.LogWarning($"{charName}: ClearAllItems - 找不到 ItemDisplay，尝试直接查找所有子对象");
                // 如果找不到 ItemDisplay，直接清理 transform 下的所有子对象（除了 ItemDropZone 本身）
                List<GameObject> toDestroy = new List<GameObject>();
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = transform.GetChild(i);
                    if (child.name != "ItemDropZone") // 避免删除自己
                    {
                        toDestroy.Add(child.gameObject);
                    }
                }
                
                foreach (var obj in toDestroy)
                {
                    if (obj != null)
                    {
                        Debug.Log($"{charName}: 清理子对象: {obj.name}");
                        Destroy(obj);
                    }
                }
                
                // 清理引用后返回
                currentFoodItem = null;
                currentDisguiseItem = null;
                foodCancelButton = null;
                disguiseCancelButton = null;
                return;
            }
            
            if (itemDisplayParent != null)
            {
                Debug.Log($"{charName}: ItemDisplay 存在，childCount={itemDisplayParent.childCount}");
                
                // 遍历所有子对象，销毁所有物品和取消按钮
                List<GameObject> toDestroy = new List<GameObject>();
                
                for (int i = itemDisplayParent.childCount - 1; i >= 0; i--)
                {
                    Transform child = itemDisplayParent.GetChild(i);
                    if (child != null)
                    {
                        Debug.Log($"{charName}: 准备销毁: {child.name}, position={child.localPosition}");
                        toDestroy.Add(child.gameObject);
                    }
                }
                
                Debug.Log($"{charName}: 准备销毁 {toDestroy.Count} 个对象");
                
                foreach (var obj in toDestroy)
                {
                    if (obj != null)
                    {
                        Debug.Log($"{charName}: 销毁物品/按钮: {obj.name}, active={obj.activeSelf}");
                        // 先激活对象（如果被禁用），确保能正确销毁
                        if (!obj.activeSelf)
                        {
                            obj.SetActive(true);
                        }
                        Destroy(obj);
                    }
                }
                
                Debug.Log($"{charName}: 清理完成，ItemDisplay childCount={itemDisplayParent.childCount}");
            }
            else
            {
                Debug.LogWarning($"{charName}: ClearAllItems - itemDisplayParent 为 null");
            }
            
            // 清理引用
            currentFoodItem = null;
            currentDisguiseItem = null;
            foodCancelButton = null;
            disguiseCancelButton = null;
            itemDisplayParent = null; // 也清空引用，下次使用时重新查找
            
            Debug.Log($"{charName}: ClearAllItems 完成 - 所有引用已清理");
        }
        
        /// <summary>
        /// 消耗食物（结算时调用）
        /// </summary>
        public void ConsumeFoodItem()
        {
            string charName = occupant?.GetName() ?? "Unknown";
            Debug.Log($"{charName}: ConsumeFoodItem 被调用");
            
            // 使用全局物品实例管理器清理（不依赖 GameObject 激活状态）
            if (charName != null)
            {
                ItemInstanceManager.Instance.ClearCharacterItems(charName);
            }
            
            // 清理本地引用
            currentFoodItem = null;
            foodCancelButton = null;
            
            // 更新全局数据管理器
            if (occupant != null)
            {
                CharacterItemDataManager.Instance.ConsumeFood(occupant.GetName());
            }
        }
        
        /// <summary>
        /// 消耗伪装物品（结算时调用）
        /// </summary>
        public void ConsumeDisguiseItem()
        {
            string charName = occupant?.GetName() ?? "Unknown";
            Debug.Log($"{charName}: ConsumeDisguiseItem 被调用");
            
            // 使用全局物品实例管理器清理（不依赖 GameObject 激活状态）
            if (charName != null)
            {
                ItemInstanceManager.Instance.ClearCharacterItems(charName);
            }
            
            // 清理本地引用
            currentDisguiseItem = null;
            disguiseCancelButton = null;
            
            // 更新全局数据管理器
            if (occupant != null)
            {
                CharacterItemDataManager.Instance.ConsumeDisguise(occupant.GetName());
            }
        }
        
        public CarOccupant GetOccupant()
        {
            return occupant;
        }
    }
}
