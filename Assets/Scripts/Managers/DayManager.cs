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
            
            Debug.Log($"DayManager: 找到 {occupants.Length} 个角色，准备降低状态");
            
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
                    float oldSatiety = occupant.GetSatiety();
                    float oldDisguise = occupant.GetDisguise();
                    
                    occupant.DecreaseSatietyLevel();
                    occupant.DecreaseDisguiseLevel();
                    
                    Debug.Log($"{occupant.GetName()}: 饱腹度 {oldSatiety} -> {occupant.GetSatiety()}, 伪装度 {oldDisguise} -> {occupant.GetDisguise()}");
                }
                else
                {
                    Debug.Log($"{occupant.GetName()}: 已死亡，跳过");
                }
            }
        }
    }
}
