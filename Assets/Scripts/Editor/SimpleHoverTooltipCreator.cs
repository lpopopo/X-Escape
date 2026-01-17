using UnityEngine;
using UnityEditor;
using XEscape.UI;

namespace XEscape.Editor
{
    /// <summary>
    /// 创建 SimpleHoverTooltip 的编辑器工具
    /// </summary>
    public static class SimpleHoverTooltipCreator
    {
        [MenuItem("Tools/X-Escape/创建悬停提示系统")]
        public static void CreateSimpleHoverTooltip()
        {
            // 检查是否已存在
            SimpleHoverTooltip existingTooltip = Object.FindFirstObjectByType<SimpleHoverTooltip>();
            if (existingTooltip != null)
            {
                EditorUtility.DisplayDialog("已存在", 
                    "场景中已存在 SimpleHoverTooltip 组件！\n\n" +
                    $"位置: {existingTooltip.name}\n\n" +
                    "是否要选中它？", 
                    "是");
                Selection.activeGameObject = existingTooltip.gameObject;
                return;
            }

            // 创建 TooltipManager
            GameObject tooltipManager = new GameObject("TooltipManager");
            SimpleHoverTooltip tooltip = tooltipManager.AddComponent<SimpleHoverTooltip>();

            // 选中新创建的对象
            Selection.activeGameObject = tooltipManager;

            Debug.Log("SimpleHoverTooltip 创建成功！");
            Debug.Log("组件会自动创建 UI 元素，无需手动设置。");
        }
    }
}
