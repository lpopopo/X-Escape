using UnityEngine;
using UnityEditor;
using XEscape.Managers;
using XEscape.CarScene;

namespace XEscape.Editor
{
    /// <summary>
    /// 物品系统创建工具
    /// </summary>
    public class ItemSystemCreator
    {
        [MenuItem("Tools/X-Escape/创建物品系统")]
        public static void CreateItemSystem()
        {
            // 查找或创建ItemManager
            ItemManager itemManager = Object.FindFirstObjectByType<ItemManager>();
            if (itemManager == null)
            {
                GameObject managerObj = new GameObject("ItemManager");
                itemManager = managerObj.AddComponent<ItemManager>();
                Debug.Log("已创建 ItemManager");
            }
            
            // 查找ViewSwitcher以获取车内视角
            ViewSwitcher viewSwitcher = Object.FindFirstObjectByType<ViewSwitcher>();
            if (viewSwitcher != null)
            {
                GameObject interiorView = viewSwitcher.GetInteriorView();
                if (interiorView != null)
                {
                    // 创建物品容器
                    Transform itemContainer = interiorView.transform.Find("ItemContainer");
                    if (itemContainer == null)
                    {
                        GameObject containerObj = new GameObject("ItemContainer");
                        containerObj.transform.SetParent(interiorView.transform);
                        containerObj.transform.localPosition = Vector3.zero;
                        
                        // 使用反射设置itemSpawnParent
                        var field = typeof(ItemManager).GetField("itemSpawnParent", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(itemManager, containerObj.transform);
                        }
                        
                        Debug.Log("已在车内视角创建 ItemContainer");
                    }
                }
            }
            
            // 为所有角色添加ItemDropZone
            CarOccupant[] occupants = Object.FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            foreach (CarOccupant occupant in occupants)
            {
                if (occupant != null)
                {
                    ItemDropZone dropZone = occupant.GetComponentInChildren<ItemDropZone>();
                    if (dropZone == null)
                    {
                        GameObject dropZoneObj = new GameObject("ItemDropZone");
                        dropZoneObj.transform.SetParent(occupant.transform);
                        dropZoneObj.transform.localPosition = Vector3.zero;
                        dropZone = dropZoneObj.AddComponent<ItemDropZone>();
                        
                        Debug.Log($"已为 {occupant.GetName()} 添加 ItemDropZone");
                    }
                }
            }
            
            Debug.Log("物品系统创建完成！");
            Debug.Log("请手动设置 ItemManager 的 Food Sprite 和 Disguise Sprite");
        }
    }
}
