using UnityEngine;
using XEscape.CarScene;

namespace XEscape.Utilities
{
    /// <summary>
    /// 验证 ViewSwitcher 设置是否正确
    /// </summary>
    public class VerifyViewSwitcherSetup : MonoBehaviour
    {
        [ContextMenu("验证 ViewSwitcher 设置")]
        public void VerifySetup()
        {
            Debug.Log("========== ViewSwitcher 设置验证 ==========");
            
            ViewSwitcher switcher = FindFirstObjectByType<ViewSwitcher>();
            if (switcher == null)
            {
                Debug.LogError("❌ 未找到 ViewSwitcher 组件！");
                Debug.Log("==========================================\n");
                return;
            }
            
            Debug.Log($"✅ 找到 ViewSwitcher: {switcher.name}");
            
            // 使用反射获取私有字段
            var interiorViewField = typeof(ViewSwitcher).GetField("interiorView", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var frontWindowViewField = typeof(ViewSwitcher).GetField("frontWindowView", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var switchButtonField = typeof(ViewSwitcher).GetField("switchButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buttonTextField = typeof(ViewSwitcher).GetField("buttonText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            GameObject interiorView = interiorViewField?.GetValue(switcher) as GameObject;
            GameObject frontWindowView = frontWindowViewField?.GetValue(switcher) as GameObject;
            UnityEngine.UI.Button switchButton = switchButtonField?.GetValue(switcher) as UnityEngine.UI.Button;
            UnityEngine.UI.Text buttonText = buttonTextField?.GetValue(switcher) as UnityEngine.UI.Text;
            
            // 检查 Interior View
            Debug.Log("\n1. Interior View (车内场景):");
            if (interiorView != null)
            {
                Debug.Log($"   ✅ 已设置: {interiorView.name}");
                Debug.Log($"      - 激活状态: {interiorView.activeInHierarchy}");
                
                // 检查是否有 CarInteriorView 组件
                CarInteriorView carInteriorView = interiorView.GetComponent<CarInteriorView>();
                if (carInteriorView != null)
                {
                    Debug.Log($"      - ✅ 有 CarInteriorView 组件");
                }
                else
                {
                    Debug.LogWarning($"      - ⚠️ 没有 CarInteriorView 组件（可选，但推荐添加）");
                }
                
                // 检查子对象
                int childCount = interiorView.transform.childCount;
                Debug.Log($"      - 子对象数量: {childCount}");
                if (childCount > 0)
                {
                    Debug.Log("      - 子对象列表:");
                    for (int i = 0; i < childCount; i++)
                    {
                        Transform child = interiorView.transform.GetChild(i);
                        Debug.Log($"        [{i + 1}] {child.name} (激活: {child.gameObject.activeInHierarchy})");
                        
                        // 检查人物是否有 CarOccupant 组件
                        CarOccupant occupant = child.GetComponent<CarOccupant>();
                        if (occupant != null)
                        {
                            Debug.Log($"            ✅ 有 CarOccupant 组件");
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("   ❌ 未设置！请在 Inspector 中设置 Interior View 字段");
            }
            
            // 检查 Front Window View
            Debug.Log("\n2. Front Window View (车前窗场景):");
            if (frontWindowView != null)
            {
                Debug.Log($"   ✅ 已设置: {frontWindowView.name}");
                Debug.Log($"      - 激活状态: {frontWindowView.activeInHierarchy}");
                
                SpriteRenderer renderer = frontWindowView.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Debug.Log($"      - ✅ 有 SpriteRenderer 组件");
                    if (renderer.sprite != null)
                    {
                        Debug.Log($"      - ✅ Sprite 已设置: {renderer.sprite.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"      - ⚠️ Sprite 未设置");
                    }
                }
                else
                {
                    Debug.LogWarning($"      - ⚠️ 没有 SpriteRenderer 组件");
                }
            }
            else
            {
                Debug.LogWarning("   ⚠️ 未设置！ViewSwitcher 会在运行时自动创建");
            }
            
            // 检查 Switch Button
            Debug.Log("\n3. Switch Button (切换按钮):");
            if (switchButton != null)
            {
                Debug.Log($"   ✅ 已设置: {switchButton.name}");
                Debug.Log($"      - 激活状态: {switchButton.gameObject.activeInHierarchy}");
                Debug.Log($"      - 可交互: {switchButton.interactable}");
                
                // 检查 OnClick 事件
                int listenerCount = switchButton.onClick.GetPersistentEventCount();
                Debug.Log($"      - OnClick 监听器数量: {listenerCount}");
                if (listenerCount > 0)
                {
                    Debug.Log($"      - ✅ 按钮已连接");
                }
                else
                {
                    Debug.LogWarning($"      - ⚠️ OnClick 事件为空（但代码中会自动连接）");
                }
            }
            else
            {
                Debug.LogError("   ❌ 未设置！请在 Inspector 中设置 Switch Button 字段");
            }
            
            // 检查 Button Text
            Debug.Log("\n4. Button Text (按钮文本):");
            if (buttonText != null)
            {
                Debug.Log($"   ✅ 已设置: {buttonText.name}");
                Debug.Log($"      - 当前文本: \"{buttonText.text}\"");
            }
            else
            {
                Debug.LogWarning("   ⚠️ 未设置（可选，但推荐设置）");
            }
            
            // 总结
            Debug.Log("\n========== 总结 ==========");
            bool allSet = interiorView != null && switchButton != null;
            if (allSet)
            {
                Debug.Log("✅ 基本设置已完成！可以运行游戏测试。");
                if (frontWindowView == null)
                {
                    Debug.Log("ℹ️ Front Window View 会在运行时自动创建");
                }
            }
            else
            {
                Debug.LogError("❌ 还有未完成的设置，请检查上述信息");
            }
            Debug.Log("==========================\n");
        }
    }
}
