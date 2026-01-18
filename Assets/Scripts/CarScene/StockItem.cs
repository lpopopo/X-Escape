using UnityEngine;
using XEscape.Inventory;
using XEscape.Managers;

namespace XEscape.CarScene
{
    /// <summary>
    /// 固定位置的库存物品，点击后拖出一个副本
    /// </summary>
    public class StockItem : MonoBehaviour
    {
        private ItemManager itemManager;
        private ItemType itemType;
        private Camera mainCamera;
        private Collider2D itemCollider;
        
        private bool isDragging = false;
        private GameObject draggedItem; // 拖动中的副本
        
        public void Initialize(ItemManager manager, ItemType type)
        {
            itemManager = manager;
            itemType = type;
            mainCamera = Camera.main ?? FindFirstObjectByType<Camera>();
            itemCollider = GetComponent<Collider2D>();
            
            if (itemCollider == null)
            {
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    boxCollider.size = spriteRenderer.sprite.bounds.size;
                }
                else
                {
                    boxCollider.size = new Vector2(1f, 1f);
                }
                itemCollider = boxCollider;
            }
        }
        
        private void Update()
        {
            if (itemManager == null || mainCamera == null) return;
            
            // 检查库存是否大于0
            int stock = (itemType == ItemType.Food) ? itemManager.GetFoodStock() : itemManager.GetDisguiseStock();
            if (stock <= 0) return;
            
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            // 开始拖动 - 创建副本
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                if (itemCollider != null && itemCollider.OverlapPoint(mouseWorldPos))
                {
                    StartDrag(mouseWorldPos);
                }
            }
            
            // 拖动中
            if (isDragging && draggedItem != null)
            {
                draggedItem.transform.position = mouseWorldPos;
                
                // 结束拖动
                if (Input.GetMouseButtonUp(0))
                {
                    EndDrag(mouseWorldPos);
                }
            }
        }
        
        private void StartDrag(Vector3 mousePos)
        {
            isDragging = true;
            
            // 创建拖动的副本
            GameObject prefab = (itemType == ItemType.Food) ? itemManager.GetFoodPrefab() : itemManager.GetDisguisePrefab();
            if (prefab == null) return;
            
            draggedItem = Instantiate(prefab);
            draggedItem.name = "DraggedItem";
            draggedItem.transform.position = mousePos;
            
            // 提高渲染顺序
            SpriteRenderer spriteRenderer = draggedItem.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 100;
            }
            
            // 移除StockItem组件（副本不需要）
            StockItem stockScript = draggedItem.GetComponent<StockItem>();
            if (stockScript != null) Destroy(stockScript);
        }
        
        private void EndDrag(Vector3 mousePos)
        {
            isDragging = false;
            
            if (draggedItem == null) return;
            
            // 检测是否拖到角色上
            ItemDropZone dropZone = null;
            ItemDropZone[] allDropZones = FindObjectsByType<ItemDropZone>(FindObjectsSortMode.None);
            float minDistance = float.MaxValue;
            float detectionRange = 2.0f;
            
            foreach (ItemDropZone zone in allDropZones)
            {
                if (zone == null || !zone.gameObject.activeInHierarchy) continue;
                
                CarOccupant occupant = zone.GetOccupant();
                if (occupant != null && occupant.IsDead()) continue;
                
                float distance = Vector3.Distance(mousePos, zone.transform.position);
                if (distance < detectionRange && distance < minDistance)
                {
                    dropZone = zone;
                    minDistance = distance;
                }
            }
            
            if (dropZone != null)
            {
                // 成功放置到角色上
                bool success = dropZone.AssignItem(itemType, draggedItem);
                if (success)
                {
                    // 减少库存
                    if (itemType == ItemType.Food)
                    {
                        itemManager.DecreaseFoodStock();
                    }
                    else
                    {
                        itemManager.DecreaseDisguiseStock();
                    }
                    draggedItem = null; // 不销毁，已被角色持有
                    return;
                }
            }
            
            // 没有放置成功，销毁副本
            Destroy(draggedItem);
            draggedItem = null;
        }
        
        /// <summary>
        /// 获取物品类型
        /// </summary>
        public ItemType GetItemType()
        {
            return itemType;
        }
    }
}
