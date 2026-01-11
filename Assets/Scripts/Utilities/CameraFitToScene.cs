using UnityEngine;

namespace XEscape.Utilities
{
    /// <summary>
    /// 2D相机自动适配场景脚本，自动调整相机大小和位置以显示场景中的所有对象
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFitToScene : MonoBehaviour
    {
        [Header("自动适配设置")]
        [SerializeField] private bool autoFitOnStart = true; // 启动时自动适配
        [SerializeField] private bool autoFitOnAwake = false; // Awake时自动适配
        [SerializeField] private float padding = 1f; // 边距，确保对象不会紧贴屏幕边缘
        
        [Header("手动设置")]
        [SerializeField] private float minOrthographicSize = 2f; // 最小相机大小
        [SerializeField] private float maxOrthographicSize = 20f; // 最大相机大小
        
        [Header("适配目标")]
        [SerializeField] private bool fitToAllRenderers = true; // 适配所有SpriteRenderer
        [SerializeField] private bool fitToAllColliders = false; // 适配所有Collider2D
        [SerializeField] private bool fitToSpecificObjects = false; // 适配指定对象
        [SerializeField] private Transform[] specificObjects; // 要适配的特定对象
        
        private Camera cam;
        private Bounds sceneBounds;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
                cam = GetComponent<Camera>();
            
            // 确保是正交相机
            if (cam != null && !cam.orthographic)
            {
                cam.orthographic = true;
            }
            
            if (autoFitOnAwake)
            {
                FitToScene();
            }
        }

        private void Start()
        {
            if (autoFitOnStart)
            {
                FitToScene();
            }
        }

        /// <summary>
        /// 适配场景中的所有对象
        /// </summary>
        [ContextMenu("适配到场景")]
        public void FitToScene()
        {
            if (cam == null)
            {
                cam = GetComponent<Camera>();
                if (cam == null)
                {
                    Debug.LogWarning("CameraFitToScene: 未找到Camera组件");
                    return;
                }
            }

            // 计算场景边界
            CalculateSceneBounds();

            // 如果边界有效，调整相机
            if (sceneBounds.size.magnitude > 0.01f)
            {
                FitCameraToBounds(sceneBounds);
            }
            else
            {
                Debug.LogWarning("CameraFitToScene: 未找到任何对象来适配，请检查场景中是否有SpriteRenderer或Collider2D");
            }
        }

        /// <summary>
        /// 计算场景边界
        /// </summary>
        private void CalculateSceneBounds()
        {
            bool boundsInitialized = false;
            sceneBounds = new Bounds();

            // 适配所有SpriteRenderer
            if (fitToAllRenderers)
            {
                SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
                foreach (SpriteRenderer renderer in renderers)
                {
                    if (renderer.enabled && renderer.sprite != null)
                    {
                        Bounds rendererBounds = renderer.bounds;
                        if (!boundsInitialized)
                        {
                            sceneBounds = rendererBounds;
                            boundsInitialized = true;
                        }
                        else
                        {
                            sceneBounds.Encapsulate(rendererBounds);
                        }
                    }
                }
            }

            // 适配所有Collider2D
            if (fitToAllColliders)
            {
                Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.enabled)
                    {
                        Bounds colliderBounds = collider.bounds;
                        if (!boundsInitialized)
                        {
                            sceneBounds = colliderBounds;
                            boundsInitialized = true;
                        }
                        else
                        {
                            sceneBounds.Encapsulate(colliderBounds);
                        }
                    }
                }
            }

            // 适配指定对象
            if (fitToSpecificObjects && specificObjects != null)
            {
                foreach (Transform obj in specificObjects)
                {
                    if (obj != null)
                    {
                        // 尝试获取SpriteRenderer
                        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                        if (sr != null && sr.enabled && sr.sprite != null)
                        {
                            Bounds objBounds = sr.bounds;
                            if (!boundsInitialized)
                            {
                                sceneBounds = objBounds;
                                boundsInitialized = true;
                            }
                            else
                            {
                                sceneBounds.Encapsulate(objBounds);
                            }
                        }
                        // 尝试获取Collider2D
                        else
                        {
                            Collider2D col = obj.GetComponent<Collider2D>();
                            if (col != null && col.enabled)
                            {
                                Bounds objBounds = col.bounds;
                                if (!boundsInitialized)
                                {
                                    sceneBounds = objBounds;
                                    boundsInitialized = true;
                                }
                                else
                                {
                                    sceneBounds.Encapsulate(objBounds);
                                }
                            }
                            // 如果没有Renderer或Collider，使用位置
                            else
                            {
                                if (!boundsInitialized)
                                {
                                    sceneBounds = new Bounds(obj.position, Vector3.zero);
                                    boundsInitialized = true;
                                }
                                else
                                {
                                    sceneBounds.Encapsulate(obj.position);
                                }
                            }
                        }
                    }
                }
            }

            // 如果什么都没找到，使用默认边界
            if (!boundsInitialized)
            {
                sceneBounds = new Bounds(Vector3.zero, new Vector3(10f, 10f, 0f));
            }
        }

        /// <summary>
        /// 调整相机以适配边界
        /// </summary>
        private void FitCameraToBounds(Bounds bounds)
        {
            // 扩展边界以包含边距
            bounds.Expand(padding * 2f);

            // 计算所需的orthographicSize
            float width = bounds.size.x;
            float height = bounds.size.y;

            // 根据屏幕宽高比计算
            float screenAspect = (float)Screen.width / Screen.height;
            float targetAspect = width / height;

            float orthographicSize;
            if (targetAspect > screenAspect)
            {
                // 宽度是限制因素
                orthographicSize = width / (2f * screenAspect);
            }
            else
            {
                // 高度是限制因素
                orthographicSize = height / 2f;
            }

            // 限制在最小和最大值之间
            orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSize, maxOrthographicSize);

            // 设置相机大小
            cam.orthographicSize = orthographicSize;

            // 移动相机到边界中心
            Vector3 targetPosition = bounds.center;
            targetPosition.z = cam.transform.position.z; // 保持Z轴位置不变
            cam.transform.position = targetPosition;

            Debug.Log($"CameraFitToScene: 相机已适配到场景。OrthographicSize = {orthographicSize:F2}, 边界大小 = {bounds.size}");
        }

        /// <summary>
        /// 手动设置相机大小（用于测试）
        /// </summary>
        public void SetOrthographicSize(float size)
        {
            if (cam == null)
                cam = GetComponent<Camera>();
            
            if (cam != null)
            {
                cam.orthographicSize = Mathf.Clamp(size, minOrthographicSize, maxOrthographicSize);
            }
        }

        /// <summary>
        /// 获取当前场景边界（用于调试）
        /// </summary>
        public Bounds GetSceneBounds()
        {
            CalculateSceneBounds();
            return sceneBounds;
        }

        // 在编辑器中绘制边界（仅用于调试）
        private void OnDrawGizmos()
        {
            if (cam == null)
                return;

            CalculateSceneBounds();
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(sceneBounds.center, sceneBounds.size);
        }
    }
}

