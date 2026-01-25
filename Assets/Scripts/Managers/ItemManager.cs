using UnityEngine;
using XEscape.CarScene;
using XEscape.Inventory;

namespace XEscape.Managers
{
    /// <summary>
    /// 物品管理器 - 管理库存和物品显示
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Header("预制体")]
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private GameObject disguisePrefab;
        
        [Header("初始库存")]
        [SerializeField] private int initialFoodCount = 2;
        [SerializeField] private int initialDisguiseCount = 2;
        
        [Header("固定显示位置（场景中的展示物品）")]
        [SerializeField] private Transform foodDisplayPosition;
        [SerializeField] private Transform disguiseDisplayPosition;
        
        // 当前库存数量
        private int foodStock;
        private int disguiseStock;
        
        // 固定位置的显示物品
        private GameObject foodDisplayObj;
        private GameObject disguiseDisplayObj;
        
        private void Start()
        {
            // 初始化库存
            foodStock = initialFoodCount;
            disguiseStock = initialDisguiseCount;
            
            // 查找或创建显示位置
            SetupDisplayPositions();
            
            // 创建固定位置的显示物品
            CreateDisplayItems();
            
            // 更新显示状态
            UpdateFoodDisplay();
            UpdateDisguiseDisplay();
        }
        
        private void SetupDisplayPositions()
        {
            ViewSwitcher viewSwitcher = FindFirstObjectByType<ViewSwitcher>();
            if (viewSwitcher == null) return;
            
            GameObject interiorView = viewSwitcher.GetInteriorView();
            if (interiorView == null) return;
            
            interiorView.SetActive(true);
            
            // 查找或创建 ItemContainer
            Transform container = interiorView.transform.Find("ItemContainer");
            if (container == null)
            {
                GameObject containerObj = new GameObject("ItemContainer");
                containerObj.transform.SetParent(interiorView.transform);
                containerObj.transform.localPosition = Vector3.zero;
                container = containerObj.transform;
            }
            
            // 食物显示位置
            if (foodDisplayPosition == null)
            {
                Transform existing = container.Find("FoodDisplay");
                if (existing != null)
                {
                    foodDisplayPosition = existing;
                }
                else
                {
                    GameObject foodPos = new GameObject("FoodDisplay");
                    foodPos.transform.SetParent(container);
                    foodPos.transform.localPosition = new Vector3(-1f, -2f, 0f);
                    foodDisplayPosition = foodPos.transform;
                }
            }
            
            // 伪装物品显示位置
            if (disguiseDisplayPosition == null)
            {
                Transform existing = container.Find("DisguiseDisplay");
                if (existing != null)
                {
                    disguiseDisplayPosition = existing;
                }
                else
                {
                    GameObject disguisePos = new GameObject("DisguiseDisplay");
                    disguisePos.transform.SetParent(container);
                    disguisePos.transform.localPosition = new Vector3(1f, -2f, 0f);
                    disguiseDisplayPosition = disguisePos.transform;
                }
            }
        }
        
        private void CreateDisplayItems()
        {
            // 创建食物显示物品
            if (foodPrefab != null && foodDisplayPosition != null)
            {
                foodDisplayObj = Instantiate(foodPrefab, foodDisplayPosition);
                foodDisplayObj.name = "FoodStock";
                foodDisplayObj.transform.localPosition = Vector3.zero;
                
                // 添加库存物品组件
                StockItem stockItem = foodDisplayObj.AddComponent<StockItem>();
                stockItem.Initialize(this, ItemType.Food);
                
                // 移除多余的组件
                var existingScript = foodDisplayObj.GetComponent<StockItem>();
                if (existingScript != null && existingScript != stockItem) Destroy(existingScript);
            }
            
            // 创建伪装物品显示物品
            if (disguisePrefab != null && disguiseDisplayPosition != null)
            {
                disguiseDisplayObj = Instantiate(disguisePrefab, disguiseDisplayPosition);
                disguiseDisplayObj.name = "DisguiseStock";
                disguiseDisplayObj.transform.localPosition = Vector3.zero;
                
                // 添加库存物品组件
                StockItem stockItem = disguiseDisplayObj.AddComponent<StockItem>();
                stockItem.Initialize(this, ItemType.Disguise);
                
                // 移除多余的组件
                var existingScript = disguiseDisplayObj.GetComponent<StockItem>();
                if (existingScript != null && existingScript != stockItem) Destroy(existingScript);
            }
        }
        
        /// <summary>
        /// 更新食物显示状态
        /// </summary>
        public void UpdateFoodDisplay()
        {
            if (foodDisplayObj != null)
            {
                foodDisplayObj.SetActive(foodStock > 0);
            }
        }
        
        /// <summary>
        /// 更新伪装物品显示状态
        /// </summary>
        public void UpdateDisguiseDisplay()
        {
            if (disguiseDisplayObj != null)
            {
                disguiseDisplayObj.SetActive(disguiseStock > 0);
            }
        }
        
        /// <summary>
        /// 减少食物库存（赋予角色时调用）
        /// </summary>
        public void DecreaseFoodStock()
        {
            if (foodStock > 0)
            {
                foodStock--;
                UpdateFoodDisplay();
            }
        }
        
        /// <summary>
        /// 增加食物库存（取消赋予时调用）
        /// </summary>
        public void IncreaseFoodStock()
        {
            foodStock++;
            UpdateFoodDisplay();
        }
        
        /// <summary>
        /// 减少伪装物品库存（赋予角色时调用）
        /// </summary>
        public void DecreaseDisguiseStock()
        {
            if (disguiseStock > 0)
            {
                disguiseStock--;
                UpdateDisguiseDisplay();
            }
        }
        
        /// <summary>
        /// 增加伪装物品库存（取消赋予时调用）
        /// </summary>
        public void IncreaseDisguiseStock()
        {
            disguiseStock++;
            UpdateDisguiseDisplay();
        }
        
        /// <summary>
        /// 获取食物库存数量
        /// </summary>
        public int GetFoodStock()
        {
            return foodStock;
        }
        
        /// <summary>
        /// 获取伪装物品库存数量
        /// </summary>
        public int GetDisguiseStock()
        {
            return disguiseStock;
        }
        
        /// <summary>
        /// 获取食物预制体
        /// </summary>
        public GameObject GetFoodPrefab()
        {
            return foodPrefab;
        }
        
        /// <summary>
        /// 获取伪装物品预制体
        /// </summary>
        public GameObject GetDisguisePrefab()
        {
            return disguisePrefab;
        }
    }
}
