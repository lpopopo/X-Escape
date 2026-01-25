using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using XEscape.Managers;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 背包管理器 - 管理拾取的物资
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("背包容量")]
        [SerializeField] private int maxSlots = 6;
        [SerializeField] private int maxItemCount = 6; // 最大物品数量限制

        // 物品数据结构
        [System.Serializable]
        public class InventorySlot
        {
            public ItemType itemType;
            public float amount;
            public bool isEmpty = true;
            public int pickupCount = 0; // 记录拾取次数

            public InventorySlot()
            {
                isEmpty = true;
                amount = 0f;
                pickupCount = 0;
            }
        }

        private List<InventorySlot> inventory = new List<InventorySlot>();

        // 事件 - 当背包更新时触发
        public event Action<List<InventorySlot>> OnInventoryChanged;
        
        // 事件 - 当拾取失败时触发（用于显示用户提示）
        public event Action<string> OnPickupFailed;

        // 标志：是否已禁用拾取（达到限制后）
        private bool isPickupDisabled = false;

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
            Debug.Log($"[InventoryManager] AddItem 被调用: {itemType}, amount={amount}");
            
            // 如果拾取已禁用，直接返回
            if (isPickupDisabled)
            {
                Debug.Log("[InventoryManager] 拾取已禁用，无法添加物品");
                OnPickupFailed?.Invoke("物资选择已结束，无法继续拾取");
                return false;
            }
            
            // 尝试堆叠到现有物品
            foreach (var slot in inventory)
            {
                if (!slot.isEmpty && slot.itemType == itemType)
                {
                    slot.amount += amount;
                    slot.pickupCount++; // 增加拾取次数
                    Debug.Log($"[InventoryManager] 物品堆叠到现有槽位，当前数量: {slot.amount}, 拾取次数: {slot.pickupCount}");
                    OnInventoryChanged?.Invoke(inventory);
                    // 检查背包是否已满
                    CheckInventoryFull();
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
                    slot.pickupCount = 1; // 第一次拾取
                    Debug.Log($"[InventoryManager] 物品添加到新槽位: {itemType}, amount={amount}");
                    OnInventoryChanged?.Invoke(inventory);
                    // 检查背包是否已满
                    CheckInventoryFull();
                    return true;
                }
            }

            Debug.LogWarning("[InventoryManager] 背包已满! 无法添加物品");
            OnPickupFailed?.Invoke("背包已满，无法拾取更多物品");
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
            slot.pickupCount = 0;

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

        /// <summary>
        /// 获取所有物品的总数量（计算实际拾取的物品数量，包括堆叠的物品）
        /// </summary>
        private int GetTotalItemCount()
        {
            int totalCount = 0;
            foreach (var slot in inventory)
            {
                if (!slot.isEmpty)
                {
                    // 使用 pickupCount 来统计实际拾取的物品数量
                    totalCount += slot.pickupCount;
                }
            }
            return totalCount;
        }

        /// <summary>
        /// 检查物品数量是否达到限制
        /// </summary>
        private bool IsItemCountReached()
        {
            int totalCount = GetTotalItemCount();
            return totalCount >= maxItemCount;
        }

        /// <summary>
        /// 检查背包是否已满，如果满了则切换到车内场景
        /// </summary>
        private void CheckInventoryFull()
        {
            int totalCount = GetTotalItemCount();
            bool isReached = IsItemCountReached();
            
            Debug.Log($"[InventoryManager] 检查背包状态: 当前物品数量 = {totalCount}/{maxItemCount}, 是否达到限制 = {isReached}");
            
            if (isReached && !isPickupDisabled)
            {
                Debug.Log($"[InventoryManager] 物品数量已达到限制 ({totalCount}/{maxItemCount})，开始结束物资选择流程");
                
                // 标记拾取已禁用
                isPickupDisabled = true;
                
                // 检查 MonoBehaviour 是否仍然有效
                if (this == null || !gameObject.activeInHierarchy)
                {
                    Debug.LogError("[InventoryManager] MonoBehaviour 无效，无法启动协程，尝试直接切换场景");
                    EndPickupPhaseAndSwitch();
                    return;
                }
                
                // 启动结束物资选择并切换场景的协程
                try
                {
                    StartCoroutine(EndPickupPhaseAndSwitchCoroutine());
                    Debug.Log("[InventoryManager] 已启动结束物资选择协程");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[InventoryManager] 启动协程失败: {e.Message}，尝试直接结束物资选择");
                    EndPickupPhaseAndSwitch();
                }
            }
        }

        /// <summary>
        /// 结束物资选择阶段：停止物品生成，禁用拾取，然后切换场景
        /// </summary>
        private void EndPickupPhaseAndSwitch()
        {
            // 1. 停止物品生成
            ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
                Debug.Log("[InventoryManager] 已停止物品生成");
            }
            else
            {
                Debug.LogWarning("[InventoryManager] 未找到 ItemSpawner");
            }

            // 2. 禁用玩家拾取功能
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPickupEnabled(false);
                Debug.Log("[InventoryManager] 已禁用玩家拾取功能");
            }
            else
            {
                Debug.LogWarning("[InventoryManager] 未找到 PlayerController");
            }

            // 3. 切换场景
            TrySwitchSceneDirectly();
        }

        /// <summary>
        /// 协程：结束物资选择阶段并切换场景
        /// </summary>
        private System.Collections.IEnumerator EndPickupPhaseAndSwitchCoroutine()
        {
            Debug.Log("[InventoryManager] 开始结束物资选择流程");
            
            // 1. 停止物品生成
            ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
                Debug.Log("[InventoryManager] 已停止物品生成");
            }
            else
            {
                Debug.LogWarning("[InventoryManager] 未找到 ItemSpawner");
            }

            // 2. 禁用玩家拾取功能
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPickupEnabled(false);
                Debug.Log("[InventoryManager] 已禁用玩家拾取功能");
            }
            else
            {
                Debug.LogWarning("[InventoryManager] 未找到 PlayerController");
            }

            // 3. 等待一小段时间，让玩家看到效果
            yield return new WaitForSeconds(0.5f);

            // 4. 切换场景
            yield return StartCoroutine(SwitchToCarSceneDelayed());
        }

        /// <summary>
        /// 直接尝试切换场景（不通过协程）
        /// </summary>
        private void TrySwitchSceneDirectly()
        {
            Debug.Log("[InventoryManager] 尝试直接切换场景");
            
            // 方法1: 尝试通过 GameManager 切换场景
            if (GameManager.Instance != null && GameManager.Instance.sceneTransitionManager != null)
            {
                Debug.Log("[InventoryManager] 通过 GameManager 直接切换到车内场景");
                GameManager.Instance.sceneTransitionManager.LoadCarScene();
                return;
            }
            
            // 方法2: 尝试直接查找 SceneTransitionManager
            SceneTransitionManager transitionManager = FindObjectOfType<SceneTransitionManager>();
            if (transitionManager != null)
            {
                Debug.Log("[InventoryManager] 找到 SceneTransitionManager，直接切换到车内场景");
                transitionManager.LoadCarScene();
                return;
            }
            
            // 方法3: 直接使用 SceneManager 加载场景
            Debug.LogWarning("[InventoryManager] GameManager 和 SceneTransitionManager 都不存在，尝试直接加载场景");
            string targetSceneName = "CarScene";
            
            try
            {
                SceneManager.LoadScene(targetSceneName);
                Debug.Log($"[InventoryManager] 已直接调用 LoadScene({targetSceneName})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InventoryManager] 无法切换到车内场景！错误: {e.Message}");
                Debug.LogError("请确保场景名称正确，并且在 Build Settings 中添加了 CarScene 场景！");
            }
        }

        /// <summary>
        /// 延迟切换到车内场景
        /// </summary>
        private System.Collections.IEnumerator SwitchToCarSceneDelayed()
        {
            yield return null; // 等待一帧，确保UI更新完成
            
            Debug.Log("[InventoryManager] 开始执行场景切换协程");
            
            // 方法1: 尝试通过 GameManager 切换场景
            if (GameManager.Instance != null)
            {
                Debug.Log($"[InventoryManager] GameManager.Instance 存在: {GameManager.Instance != null}");
                if (GameManager.Instance.sceneTransitionManager != null)
                {
                    Debug.Log("[InventoryManager] 通过 GameManager 切换到车内场景");
                    GameManager.Instance.sceneTransitionManager.LoadCarScene();
                    yield break;
                }
                else
                {
                    Debug.LogWarning("[InventoryManager] GameManager.Instance.sceneTransitionManager 为 null");
                }
            }
            else
            {
                Debug.LogWarning("[InventoryManager] GameManager.Instance 为 null");
            }
            
            // 方法2: 尝试直接查找 SceneTransitionManager
            SceneTransitionManager transitionManager = FindObjectOfType<SceneTransitionManager>();
            if (transitionManager != null)
            {
                Debug.Log("[InventoryManager] 找到 SceneTransitionManager，切换到车内场景");
                transitionManager.LoadCarScene();
                yield break;
            }
            else
            {
                Debug.LogWarning("[InventoryManager] 未找到 SceneTransitionManager");
            }
            
            // 方法3: 直接使用 SceneManager 加载场景
            Debug.LogWarning("[InventoryManager] GameManager 和 SceneTransitionManager 都不存在，尝试直接加载场景");
            
            // 检查场景是否存在
            string[] sceneNames = new string[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames[i] = sceneName;
                Debug.Log($"[InventoryManager] Build Settings 中的场景 {i}: {sceneName}");
            }
            
            // 尝试加载场景
            string targetSceneName = "CarScene";
            bool sceneExists = System.Array.Exists(sceneNames, name => name == targetSceneName);
            
            if (sceneExists)
            {
                Debug.Log($"[InventoryManager] 场景 {targetSceneName} 存在于 Build Settings，开始加载");
                SceneManager.LoadScene(targetSceneName);
                Debug.Log($"[InventoryManager] 已调用 LoadScene({targetSceneName})");
            }
            else
            {
                Debug.LogError($"[InventoryManager] 场景 {targetSceneName} 不存在于 Build Settings！");
                Debug.LogError("请确保场景名称正确，并且在 Build Settings 中添加了 CarScene 场景！");
                Debug.LogError("当前 Build Settings 中的场景列表：");
                foreach (string name in sceneNames)
                {
                    Debug.LogError($"  - {name}");
                }
            }
        }
    }
}
