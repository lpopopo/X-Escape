using UnityEngine;
using XEscape.Inventory;

namespace XEscape.CarScene
{
    /// <summary>
    /// 物品取消按钮，点击后移除角色身上的物品并返回库存
    /// </summary>
    public class ItemCancelButton : MonoBehaviour
    {
        private ItemDropZone dropZone;
        private ItemType itemType;
        private Camera mainCamera;
        
        public void Initialize(ItemDropZone zone, ItemType type)
        {
            dropZone = zone;
            itemType = type;
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        private void Update()
        {
            if (dropZone == null || mainCamera == null) return;
            
            // 检测鼠标点击
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                
                Collider2D collider = GetComponent<Collider2D>();
                if (collider != null && collider.OverlapPoint(mouseWorldPos))
                {
                    Debug.Log($"ItemCancelButton: 点击了取消按钮，物品类型={itemType}");
                    // 点击了取消按钮
                    if (itemType == ItemType.Food)
                    {
                        dropZone.CancelFoodItem();
                    }
                    else if (itemType == ItemType.Disguise)
                    {
                        dropZone.CancelDisguiseItem();
                    }
                }
            }
        }
        
        private void OnMouseEnter()
        {
            // 鼠标悬停时改变颜色
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = new Color(1f, 0.5f, 0.5f); // 浅红色
            }
        }
        
        private void OnMouseExit()
        {
            // 鼠标离开时恢复颜色
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.red;
            }
        }
    }
}
