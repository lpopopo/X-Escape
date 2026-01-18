using UnityEngine;
using UnityEngine.Events;
using XEscape.CarScene;

namespace XEscape.Managers
{
    /// <summary>
    /// 天数管理器，控制游戏天数流程
    /// </summary>
    public class DayManager : MonoBehaviour
    {
        [Header("天数设置")]
        [SerializeField] private int currentDay = 1;
        [SerializeField] private int maxDay = 10;

        [Header("事件")]
        public UnityEvent<int> OnDayChanged;
        public UnityEvent OnGameEnd; // 所有角色死亡
        public UnityEvent OnGameWin; // 通关（第10天还有角色幸存）

        private static DayManager instance;
        public static DayManager Instance => instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 第一天初始化：设置所有角色为满档
            InitializeOccupants();
        }

        /// <summary>
        /// 初始化所有角色状态（第一天）
        /// </summary>
        private void InitializeOccupants()
        {
            // 方法1：查找所有 CarOccupant 组件
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            // 方法2：如果没找到，尝试通过名称查找
            if (occupants.Length == 0)
            {
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                System.Collections.Generic.List<CarOccupant> foundOccupants = new System.Collections.Generic.List<CarOccupant>();
                
                foreach (GameObject obj in allObjects)
                {
                    string name = obj.name.ToLower();
                    if (name.Contains("father") || name.Contains("mather") || name.Contains("mother"))
                    {
                        CarOccupant occupant = obj.GetComponent<CarOccupant>();
                        if (occupant != null)
                        {
                            foundOccupants.Add(occupant);
                        }
                    }
                }
                occupants = foundOccupants.ToArray();
            }
            
            Debug.Log($"DayManager: 初始化时找到 {occupants.Length} 个角色");
            
            foreach (var occupant in occupants)
            {
                if (occupant != null && !occupant.IsDead())
                {
                    occupant.SetSatiety(100f); // 5档满
                    occupant.SetDisguise(100f); // 5档满
                    Debug.Log($"{occupant.GetName()}: 初始化饱腹度和伪装度为 100");
                }
            }
        }

        public int GetCurrentDay() => currentDay;
        public bool IsLastDay() => currentDay >= maxDay;

        public void NextDay()
        {
            if (currentDay < maxDay)
            {
                currentDay++;
                
                // 降低所有角色的饱腹度和伪装度
                DecreaseOccupantStats();
                
                // 检查是否所有角色都死亡
                if (AreAllOccupantsDead())
                {
                    Debug.Log("DayManager: 所有角色都死亡，游戏结束");
                    OnGameEnd?.Invoke();
                    return;
                }
                
                OnDayChanged?.Invoke(currentDay);
            }
            else
            {
                // 第10天结束，检查是否有幸存角色
                if (AreAllOccupantsDead())
                {
                    Debug.Log("DayManager: 第10天结束，所有角色都死亡，游戏结束");
                    OnGameEnd?.Invoke();
                }
                else
                {
                    Debug.Log("DayManager: 第10天结束，有角色幸存，通关！");
                    OnGameWin?.Invoke();
                }
            }
        }

        /// <summary>
        /// 检查是否所有角色都死亡
        /// </summary>
        private bool AreAllOccupantsDead()
        {
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            if (occupants.Length == 0)
            {
                // 如果没有找到任何角色，认为都死亡了
                return true;
            }
            
            foreach (var occupant in occupants)
            {
                if (occupant != null && !occupant.IsDead())
                {
                    return false; // 至少有一个角色存活
                }
            }
            
            return true; // 所有角色都死亡
        }

        /// <summary>
        /// 降低所有角色的状态（每天调用）
        /// </summary>
        private void DecreaseOccupantStats()
        {
            // 方法1：查找所有 CarOccupant 组件（包括被禁用的）
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            // 方法2：如果没找到，尝试通过名称查找
            if (occupants.Length == 0)
            {
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                System.Collections.Generic.List<CarOccupant> foundOccupants = new System.Collections.Generic.List<CarOccupant>();
                
                foreach (GameObject obj in allObjects)
                {
                    string name = obj.name.ToLower();
                    if (name.Contains("father") || name.Contains("mather") || name.Contains("mother"))
                    {
                        CarOccupant occupant = obj.GetComponent<CarOccupant>();
                        if (occupant != null)
                        {
                            foundOccupants.Add(occupant);
                        }
                    }
                }
                occupants = foundOccupants.ToArray();
            }
            
            Debug.Log($"DayManager: 找到 {occupants.Length} 个角色，准备结算物品和降低状态");
            
            // 先处理物品结算（消耗物品并恢复状态），返回消耗前的物品状态
            var hadItems = ProcessItemConsumption(occupants);
            
            // 然后降低状态（没有物品的角色）
            foreach (var occupant in occupants)
            {
                if (occupant == null)
                {
                    Debug.LogWarning("DayManager: 发现 null 角色对象");
                    continue;
                }
                
                Debug.Log($"DayManager: 检查角色 {occupant.GetName()}, Active={occupant.gameObject.activeInHierarchy}, IsDead={occupant.IsDead()}");
                
                if (!occupant.IsDead())
                {
                    string charName = occupant.GetName();
                    
                    // 使用消耗前的物品状态来判断是否应该降低状态
                    bool hadFood = hadItems.ContainsKey(charName) && hadItems[charName].hadFood;
                    bool hadDisguise = hadItems.ContainsKey(charName) && hadItems[charName].hadDisguise;
                    
                    float oldSatiety = occupant.GetSatiety();
                    float oldDisguise = occupant.GetDisguise();
                    
                    // 根据消耗前的物品情况降低状态
                    // 如果消耗前有食物，不降低饱腹度（已恢复为100）
                    if (!hadFood)
                    {
                        occupant.DecreaseSatietyLevel();
                    }
                    
                    // 如果消耗前有伪装物品，不降低伪装度（已恢复为100）
                    if (!hadDisguise)
                    {
                        occupant.DecreaseDisguiseLevel();
                    }
                    
                    Debug.Log($"{charName}: 降低状态 - 饱腹度 {oldSatiety} -> {occupant.GetSatiety()} (消耗前有食物={hadFood}), 伪装度 {oldDisguise} -> {occupant.GetDisguise()} (消耗前有伪装={hadDisguise})");
                }
                else
                {
                    Debug.Log($"{occupant.GetName()}: 已死亡，跳过");
                }
            }
        }
        
        /// <summary>
        /// 处理物品消耗（每天结算时调用）
        /// </summary>
        /// <returns>返回一个字典，记录哪些角色在消耗前有食物/伪装物品</returns>
        private System.Collections.Generic.Dictionary<string, (bool hadFood, bool hadDisguise)> ProcessItemConsumption(CarOccupant[] occupants)
        {
            Debug.Log($"=== 开始物品结算 ===");
            
            CharacterItemDataManager dataManager = CharacterItemDataManager.Instance;
            // 记录消耗前的物品状态
            System.Collections.Generic.Dictionary<string, (bool hadFood, bool hadDisguise)> hadItems = 
                new System.Collections.Generic.Dictionary<string, (bool, bool)>();
            
            foreach (var occupant in occupants)
            {
                if (occupant == null || occupant.IsDead()) 
                {
                    Debug.Log($"跳过角色: {occupant?.GetName()} (null或已死亡)");
                    continue;
                }
                
                string charName = occupant.GetName();
                
                // 使用全局数据管理器检查物品（不依赖 GameObject 状态）
                bool hasFood = dataManager.HasFood(charName);
                bool hasDisguise = dataManager.HasDisguise(charName);
                
                // 记录消耗前的状态
                hadItems[charName] = (hasFood, hasDisguise);
                
                Debug.Log($"{charName}: 数据管理器 - HasFood={hasFood}, HasDisguise={hasDisguise}");
                
                // 处理食物
                if (hasFood)
                {
                    float oldSatiety = occupant.GetSatiety();
                    occupant.SetSatiety(100f);
                    dataManager.ConsumeFood(charName);
                    Debug.Log($"{charName}: 消耗食物，饱腹度 {oldSatiety} -> {occupant.GetSatiety()}");
                }
                
                // 处理伪装物品
                if (hasDisguise)
                {
                    float oldDisguise = occupant.GetDisguise();
                    occupant.SetDisguise(100f);
                    dataManager.ConsumeDisguise(charName);
                    Debug.Log($"{charName}: 消耗伪装物品，伪装度 {oldDisguise} -> {occupant.GetDisguise()}");
                }
                
                // 使用全局物品实例管理器清理所有物品 GameObject（不依赖 ItemDropZone 或 GameObject 激活状态）
                if (hasFood || hasDisguise)
                {
                    ItemInstanceManager.Instance.ClearCharacterItems(charName);
                }
            }
            
            Debug.Log($"=== 物品结算完成 ===");
            return hadItems;
        }
    }
}
