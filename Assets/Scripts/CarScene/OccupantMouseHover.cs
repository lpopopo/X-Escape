using UnityEngine;

namespace XEscape.CarScene
{
    /// <summary>
    /// 人物鼠标悬停检测组件，需要配合Collider2D使用
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class OccupantMouseHover : MonoBehaviour
    {
        [Header("检测设置")]
        [Tooltip("需要检测的人物名称关键词（不区分大小写）")]
        [SerializeField] private string[] targetNames = { "father", "mother", "mather", "爸爸", "妈妈" };
        
        [Header("事件")]
        [SerializeField] private UnityEngine.Events.UnityEvent<CarOccupant> OnHoverEnter;
        [SerializeField] private UnityEngine.Events.UnityEvent<CarOccupant> OnHoverExit;

        private CarOccupant occupant;
        private bool isHovering = false;
        private Camera mainCamera;

        private void Awake()
        {
            occupant = GetComponent<CarOccupant>();
            if (occupant == null)
            {
                Debug.LogWarning($"OccupantMouseHover 需要 CarOccupant 组件！物体: {gameObject.name}");
            }

            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();
        }

        private void Update()
        {
            CheckMouseHover();
        }

        /// <summary>
        /// 检查鼠标悬停
        /// </summary>
        private void CheckMouseHover()
        {
            if (occupant == null || mainCamera == null) return;

            // 检查是否是目标人物
            string occupantName = occupant.GetName().ToLower();
            bool isTarget = false;
            foreach (string targetName in targetNames)
            {
                if (occupantName.Contains(targetName.ToLower()))
                {
                    isTarget = true;
                    break;
                }
            }

            if (!isTarget) return;

            // 检查鼠标是否在Collider2D范围内
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Collider2D collider = GetComponent<Collider2D>();
            
            if (collider != null && collider.OverlapPoint(mouseWorldPos))
            {
                if (!isHovering)
                {
                    isHovering = true;
                    OnHoverEnter?.Invoke(occupant);
                }
            }
            else
            {
                if (isHovering)
                {
                    isHovering = false;
                    OnHoverExit?.Invoke(occupant);
                }
            }
        }

        /// <summary>
        /// 获取鼠标世界坐标
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.nearClipPlane + 1f;
            return mainCamera.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// 检查当前是否正在悬停
        /// </summary>
        public bool IsHovering()
        {
            return isHovering;
        }
    }
}
