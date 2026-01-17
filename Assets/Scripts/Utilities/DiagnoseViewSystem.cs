using UnityEngine;
using UnityEngine.UI;
using XEscape.CarScene;
using XEscape.UI;

namespace XEscape.Utilities
{
    /// <summary>
    /// 诊断视角切换和悬停提示系统的问题
    /// </summary>
    public class DiagnoseViewSystem : MonoBehaviour
    {
        [ContextMenu("诊断所有问题")]
        public void DiagnoseAll()
        {
            Debug.Log("========== 系统诊断开始 ==========");
            DiagnoseViewSwitcher();
            DiagnoseHoverTooltip();
            Debug.Log("========== 系统诊断结束 ==========\n");
        }

        [ContextMenu("诊断 ViewSwitcher")]
        public void DiagnoseViewSwitcher()
        {
            Debug.Log("\n--- ViewSwitcher 诊断 ---");
            
            ViewSwitcher switcher = FindFirstObjectByType<ViewSwitcher>();
            if (switcher == null)
            {
                Debug.LogError("❌ 未找到 ViewSwitcher 组件！");
                return;
            }
            
            Debug.Log($"✅ 找到 ViewSwitcher: {switcher.name}");
            
            // 使用反射获取字段
            var interiorViewField = typeof(ViewSwitcher).GetField("interiorView", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var frontWindowViewField = typeof(ViewSwitcher).GetField("frontWindowView", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var switchButtonField = typeof(ViewSwitcher).GetField("switchButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            GameObject interiorView = interiorViewField?.GetValue(switcher) as GameObject;
            GameObject frontWindowView = frontWindowViewField?.GetValue(switcher) as GameObject;
            Button switchButton = switchButtonField?.GetValue(switcher) as Button;
            
            // 检查 Interior View
            if (interiorView == null)
            {
                Debug.LogError("❌ Interior View 未设置！");
                Debug.Log("   解决方法：在 ViewSwitcher 的 Inspector 中，将车内场景对象拖拽到 Interior View 字段");
            }
            else
            {
                Debug.Log($"✅ Interior View 已设置: {interiorView.name}");
                Debug.Log($"   - 激活状态: {interiorView.activeInHierarchy}");
            }
            
            // 检查 Front Window View
            if (frontWindowView == null)
            {
                Debug.LogWarning("⚠️ Front Window View 未设置（会在运行时自动创建）");
            }
            else
            {
                Debug.Log($"✅ Front Window View 已设置: {frontWindowView.name}");
            }
            
            // 检查按钮
            if (switchButton == null)
            {
                Debug.LogError("❌ Switch Button 未设置！");
                Debug.Log("   解决方法：在 ViewSwitcher 的 Inspector 中，将 ViewSwitchButton 拖拽到 Switch Button 字段");
            }
            else
            {
                Debug.Log($"✅ Switch Button 已设置: {switchButton.name}");
                Debug.Log($"   - 激活状态: {switchButton.gameObject.activeInHierarchy}");
                Debug.Log($"   - 可交互: {switchButton.interactable}");
                
                // 检查OnClick事件
                int listenerCount = switchButton.onClick.GetPersistentEventCount();
                Debug.Log($"   - Inspector中的OnClick事件数: {listenerCount}");
                
                // 测试按钮点击
                Debug.Log("   测试：尝试手动调用 ToggleView...");
                try
                {
                    var toggleMethod = typeof(ViewSwitcher).GetMethod("ToggleView", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (toggleMethod != null)
                    {
                        toggleMethod.Invoke(switcher, null);
                        Debug.Log("   ✅ ToggleView 方法可以正常调用");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"   ❌ ToggleView 调用失败: {e.Message}");
                }
            }
        }

        [ContextMenu("诊断悬停提示系统")]
        public void DiagnoseHoverTooltip()
        {
            Debug.Log("\n--- 悬停提示系统诊断 ---");
            
            SimpleHoverTooltip tooltip = FindFirstObjectByType<SimpleHoverTooltip>();
            if (tooltip == null)
            {
                Debug.LogError("❌ 未找到 SimpleHoverTooltip 组件！");
                return;
            }
            
            Debug.Log($"✅ 找到 SimpleHoverTooltip: {tooltip.name}");
            
            // 使用反射获取字段
            var tooltipPanelField = typeof(SimpleHoverTooltip).GetField("tooltipPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var nameTextField = typeof(SimpleHoverTooltip).GetField("nameText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var satietyTextField = typeof(SimpleHoverTooltip).GetField("satietyText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var disguiseTextField = typeof(SimpleHoverTooltip).GetField("disguiseText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            GameObject tooltipPanel = tooltipPanelField?.GetValue(tooltip) as GameObject;
            Text nameText = nameTextField?.GetValue(tooltip) as Text;
            Text satietyText = satietyTextField?.GetValue(tooltip) as Text;
            Text disguiseText = disguiseTextField?.GetValue(tooltip) as Text;
            
            // 检查UI组件
            if (tooltipPanel == null)
            {
                Debug.LogWarning("⚠️ TooltipPanel 未设置（会在运行时自动创建）");
            }
            else
            {
                Debug.Log($"✅ TooltipPanel 已设置: {tooltipPanel.name}");
                Debug.Log($"   - 激活状态: {tooltipPanel.activeInHierarchy}");
            }
            
            if (nameText == null)
            {
                Debug.LogWarning("⚠️ NameText 未设置（会在运行时自动创建）");
            }
            else
            {
                Debug.Log($"✅ NameText 已设置: {nameText.name}");
            }
            
            if (satietyText == null)
            {
                Debug.LogWarning("⚠️ SatietyText 未设置（会在运行时自动创建）");
            }
            else
            {
                Debug.Log($"✅ SatietyText 已设置: {satietyText.name}");
            }
            
            if (disguiseText == null)
            {
                Debug.LogWarning("⚠️ DisguiseText 未设置（会在运行时自动创建）");
            }
            else
            {
                Debug.Log($"✅ DisguiseText 已设置: {disguiseText.name}");
            }
            
            // 检查人物对象
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            Debug.Log($"\n找到 {occupants.Length} 个人物对象:");
            foreach (CarOccupant occ in occupants)
            {
                Debug.Log($"  - {occ.gameObject.name}:");
                Debug.Log($"    名称: {occ.GetName()}");
                Debug.Log($"    激活: {occ.gameObject.activeInHierarchy}");
                
                Collider2D col = occ.GetComponent<Collider2D>();
                if (col == null)
                {
                    Debug.LogError($"    ❌ 没有 Collider2D 组件！无法检测鼠标悬停");
                }
                else
                {
                    Debug.Log($"    ✅ 有 Collider2D: {col.GetType().Name}");
                    Debug.Log($"       - 启用: {col.enabled}");
                    Debug.Log($"       - IsTrigger: {col.isTrigger}");
                }
                
                SpriteRenderer renderer = occ.GetComponent<SpriteRenderer>();
                if (renderer == null)
                {
                    Debug.LogWarning($"    ⚠️ 没有 SpriteRenderer 组件");
                }
                else
                {
                    Debug.Log($"    ✅ 有 SpriteRenderer");
                }
            }
            
            // 检查相机
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogError("❌ 未找到主相机！");
            }
            else
            {
                Debug.Log($"\n✅ 主相机: {mainCam.name}");
                Debug.Log($"   - 正交模式: {mainCam.orthographic}");
            }
        }
    }
}
