using UnityEngine;
using XEscape.CarScene;

namespace XEscape.UI
{
    /// <summary>
    /// 简单的工具提示测试脚本，用于验证系统是否工作
    /// </summary>
    public class TooltipTester : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("=== TooltipTester: 开始测试 ===");
            
            // 检查OccupantHoverTooltip是否存在
            OccupantHoverTooltip tooltip = FindFirstObjectByType<OccupantHoverTooltip>();
            if (tooltip == null)
            {
                Debug.LogError("❌ 未找到 OccupantHoverTooltip 组件！请确保场景中有包含此组件的物体。");
            }
            else
            {
                Debug.Log($"✓ 找到 OccupantHoverTooltip 组件，位于: {tooltip.gameObject.name}");
            }
            
            // 检查所有CarOccupant
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            Debug.Log($"找到 {occupants.Length} 个 CarOccupant 组件:");
            
            foreach (CarOccupant occ in occupants)
            {
                string name = occ.GetName();
                Collider2D col = occ.GetComponent<Collider2D>();
                bool hasCollider = col != null;
                
                Debug.Log($"  - {occ.gameObject.name}:");
                Debug.Log($"    名称: {name}");
                Debug.Log($"    有Collider2D: {hasCollider}");
                if (hasCollider)
                {
                    Debug.Log($"    Collider类型: {col.GetType().Name}");
                    Debug.Log($"    IsTrigger: {col.isTrigger}");
                    Debug.Log($"    大小: {col.bounds.size}");
                }
            }
            
            // 检查相机
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogWarning("⚠️ 未找到 Main Camera！");
            }
            else
            {
                Debug.Log($"✓ 找到 Main Camera: {mainCam.name}");
                Debug.Log($"  正交模式: {mainCam.orthographic}");
                Debug.Log($"  位置: {mainCam.transform.position}");
            }
            
            Debug.Log("=== TooltipTester: 测试完成 ===");
        }
        
        private void Update()
        {
            // 每60帧输出一次鼠标位置
            if (Time.frameCount % 60 == 0)
            {
                Vector3 mousePos = Input.mousePosition;
                Camera mainCam = Camera.main;
                
                if (mainCam != null)
                {
                    Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(mainCam.transform.position.z)));
                    Collider2D hit = Physics2D.OverlapPoint(worldPos);
                    
                    Debug.Log($"鼠标屏幕位置: {mousePos}, 世界位置: {worldPos}, 检测到Collider: {(hit != null ? hit.name : "无")}");
                }
            }
        }
    }
}
