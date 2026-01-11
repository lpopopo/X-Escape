using UnityEngine;

namespace XEscape.Utilities
{
    /// <summary>
    /// 渲染顺序调试工具，用于检查场景中所有SpriteRenderer的渲染设置
    /// </summary>
    public class RenderOrderDebugger : MonoBehaviour
    {
        [ContextMenu("检查所有渲染顺序")]
        public void CheckAllRenderOrders()
        {
            Debug.Log("=== 渲染顺序检查 ===");
            
            SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            
            if (renderers.Length == 0)
            {
                Debug.LogWarning("场景中没有找到任何SpriteRenderer组件！");
                return;
            }
            
            Debug.Log($"找到 {renderers.Length} 个SpriteRenderer组件：");
            
            foreach (SpriteRenderer sr in renderers)
            {
                if (sr == null || sr.gameObject == null) continue;
                
                string info = $"物体: {sr.gameObject.name}\n" +
                             $"  - Sorting Layer ID: {sr.sortingLayerID}\n" +
                             $"  - Sorting Layer Name: {sr.sortingLayerName}\n" +
                             $"  - Sorting Order: {sr.sortingOrder}\n" +
                             $"  - 世界位置Z: {sr.transform.position.z}\n" +
                             $"  - 本地位置Z: {sr.transform.localPosition.z}\n" +
                             $"  - 父物体: {(sr.transform.parent != null ? sr.transform.parent.name : "无")}\n" +
                             $"  - Sprite: {(sr.sprite != null ? sr.sprite.name : "无")}\n" +
                             $"  - 启用状态: {sr.enabled}";
                
                Debug.Log(info);
            }
            
            Debug.Log("=== 检查完成 ===");
        }
        
        private void Start()
        {
            // 自动检查一次
            CheckAllRenderOrders();
        }
    }
}
