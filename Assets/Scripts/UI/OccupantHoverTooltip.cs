using UnityEngine;
using UnityEngine.UI;
using XEscape.CarScene;

namespace XEscape.UI
{
    /// <summary>
    /// 人物悬停提示UI，显示饱腹度和伪装度信息
    /// </summary>
    public class OccupantHoverTooltip : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text nameText;
        [SerializeField] private Text satietyText;
        [SerializeField] private Text disguiseText;
        
        [Header("设置")]
        [SerializeField] private float offsetX = 10f;
        [SerializeField] private float offsetY = 10f;
        [SerializeField] private LayerMask occupantLayer = -1; // 默认所有层
        [Tooltip("需要检测的人物名称关键词（不区分大小写）")]
        [SerializeField] private string[] targetNames = { "father", "mother", "mather", "爸爸", "妈妈" };
        [Header("调试")]
        [SerializeField] private bool enableDebugLog = false; // 启用调试日志
        
        private Camera mainCamera;
        private Canvas canvas;
        private RectTransform tooltipRect;
        private CarOccupant currentOccupant;

        private void Awake()
        {
            if (enableDebugLog)
                Debug.Log("OccupantHoverTooltip: Awake 开始初始化");

            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();

            if (mainCamera == null)
            {
                Debug.LogError("OccupantHoverTooltip: 未找到相机！请确保场景中有Main Camera。");
            }
            else if (enableDebugLog)
            {
                Debug.Log($"OccupantHoverTooltip: 找到相机 {mainCamera.name}");
            }

            // 如果没有指定Canvas，尝试找到或创建
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasObj = new GameObject("TooltipCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                    if (enableDebugLog)
                        Debug.Log("OccupantHoverTooltip: 创建了新Canvas");
                }
            }

            // 如果没有指定tooltipPanel，创建一个默认的
            if (tooltipPanel == null)
            {
                CreateDefaultTooltip();
                if (enableDebugLog)
                    Debug.Log("OccupantHoverTooltip: 创建了默认TooltipPanel");
            }

            if (tooltipPanel != null)
            {
                tooltipRect = tooltipPanel.GetComponent<RectTransform>();
                tooltipPanel.SetActive(false);
                if (enableDebugLog)
                    Debug.Log("OccupantHoverTooltip: TooltipPanel 已创建并隐藏");
            }
            else
            {
                Debug.LogError("OccupantHoverTooltip: TooltipPanel 创建失败！");
            }

            if (enableDebugLog)
                Debug.Log("OccupantHoverTooltip: 初始化完成");
        }

        private void Start()
        {
            if (enableDebugLog)
            {
                Debug.Log("OccupantHoverTooltip: Start 被调用");
                Debug.Log($"OccupantHoverTooltip: 目标名称列表: {string.Join(", ", targetNames)}");
            }
        }

        private void Update()
        {
            // 每60帧输出一次调试信息（避免刷屏）
            if (enableDebugLog && Time.frameCount % 60 == 0)
            {
                Debug.Log($"OccupantHoverTooltip: Update 运行中，鼠标位置: {Input.mousePosition}");
            }

            CheckMouseHover();
            UpdateTooltipPosition();
        }

        /// <summary>
        /// 检查鼠标悬停
        /// </summary>
        private void CheckMouseHover()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                    mainCamera = FindFirstObjectByType<Camera>();
                
                if (mainCamera == null)
                {
                    if (enableDebugLog)
                        Debug.LogWarning("OccupantHoverTooltip: 未找到相机！");
                    return;
                }
            }

            Vector3 mouseWorldPos = GetMouseWorldPosition();
            
            // 每帧都检测所有CarOccupant（用于调试）
            if (enableDebugLog && Time.frameCount % 60 == 0)
            {
                CarOccupant[] allOccupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
                Debug.Log($"场景中找到 {allOccupants.Length} 个CarOccupant组件");
                foreach (var occ in allOccupants)
                {
                    Debug.Log($"  - {occ.name}: Name={occ.GetName()}, HasCollider={occ.GetComponent<Collider2D>() != null}");
                }
            }

            // 使用多种方法检测Collider，提高成功率
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, occupantLayer);
            
            // 如果OverlapPoint没检测到，尝试使用Raycast
            if (hitCollider == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0.1f, occupantLayer);
                if (hit.collider != null)
                {
                    hitCollider = hit.collider;
                }
            }

            if (enableDebugLog && Input.GetMouseButtonDown(0))
            {
                Debug.Log($"点击鼠标 - 鼠标世界坐标: {mouseWorldPos}, 检测到Collider: {(hitCollider != null ? hitCollider.name : "无")}");
            }
            
            // 每60帧输出一次鼠标位置（用于调试）
            if (enableDebugLog && Time.frameCount % 60 == 0)
            {
                Debug.Log($"鼠标世界坐标: {mouseWorldPos}, 检测到Collider: {(hitCollider != null ? hitCollider.name : "无")}");
            }

            if (hitCollider != null)
            {
                CarOccupant occupant = hitCollider.GetComponent<CarOccupant>();
                if (occupant != null)
                {
                    // 检查是否是目标人物（father或mother）
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

                    if (enableDebugLog)
                    {
                        Debug.Log($"检测到人物: {occupant.GetName()}, 是否为目标: {isTarget}");
                    }

                    if (isTarget)
                    {
                        if (currentOccupant != occupant)
                        {
                            ShowTooltip(occupant);
                            if (enableDebugLog)
                                Debug.Log($"显示提示: {occupant.GetName()}");
                        }
                        return;
                    }
                }
                else if (enableDebugLog)
                {
                    Debug.Log($"Collider {hitCollider.name} 没有 CarOccupant 组件");
                }
            }

            // 如果没有悬停在有效目标上，隐藏提示
            if (currentOccupant != null)
            {
                HideTooltip();
            }
        }

        /// <summary>
        /// 获取鼠标世界坐标（2D优化版本）
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                    mainCamera = FindFirstObjectByType<Camera>();
            }

            if (mainCamera == null)
                return Vector3.zero;

            Vector3 mousePos = Input.mousePosition;
            
            // 对于2D正交相机，使用正确的Z距离
            if (mainCamera.orthographic)
            {
                // 2D正交相机：Z距离 = 相机到Z=0平面的距离
                float cameraZ = mainCamera.transform.position.z;
                mousePos.z = Mathf.Abs(cameraZ);
            }
            else
            {
                mousePos.z = mainCamera.nearClipPlane + 1f;
            }
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            
            // 对于2D，强制Z=0
            if (mainCamera.orthographic)
            {
                worldPos.z = 0f;
            }
            
            return worldPos;
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        private void ShowTooltip(CarOccupant occupant)
        {
            currentOccupant = occupant;
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
                UpdateTooltipContent(occupant);
            }
        }

        /// <summary>
        /// 隐藏提示
        /// </summary>
        private void HideTooltip()
        {
            currentOccupant = null;
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 更新提示内容
        /// </summary>
        private void UpdateTooltipContent(CarOccupant occupant)
        {
            if (occupant == null) return;

            // 更新名称
            if (nameText != null)
            {
                nameText.text = occupant.GetName();
            }

            // 更新饱腹度
            if (satietyText != null)
            {
                string satietyStatus = occupant.GetSatietyStatusText();
                float satietyValue = occupant.GetSatiety();
                satietyText.text = $"饱腹度: {satietyStatus} ({satietyValue:F0}%)";
            }

            // 更新伪装度
            if (disguiseText != null)
            {
                string disguiseStatus = occupant.GetDisguiseStatusText();
                float disguiseValue = occupant.GetDisguise();
                disguiseText.text = $"伪装度: {disguiseStatus} ({disguiseValue:F0}%)";
            }
        }

        /// <summary>
        /// 更新提示位置（跟随鼠标）
        /// </summary>
        private void UpdateTooltipPosition()
        {
            if (tooltipPanel == null || tooltipRect == null || !tooltipPanel.activeSelf)
                return;

            Vector2 mousePos = Input.mousePosition;
            Vector2 tooltipSize = tooltipRect.sizeDelta;

            // 计算位置，确保不超出屏幕
            float x = mousePos.x + offsetX;
            float y = mousePos.y + offsetY;

            // 如果超出右边界，显示在鼠标左侧
            if (x + tooltipSize.x > Screen.width)
            {
                x = mousePos.x - tooltipSize.x - offsetX;
            }

            // 如果超出上边界，显示在鼠标下方
            if (y + tooltipSize.y > Screen.height)
            {
                y = mousePos.y - tooltipSize.y - offsetY;
            }

            tooltipRect.position = new Vector3(x, y, 0);
        }

        /// <summary>
        /// 创建默认的提示UI
        /// </summary>
        private void CreateDefaultTooltip()
        {
            if (canvas == null) return;

            // 创建背景面板
            tooltipPanel = new GameObject("TooltipPanel");
            tooltipPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = tooltipPanel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(200, 100);

            Image panelImage = tooltipPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            // 创建名称文本
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(tooltipPanel.transform, false);
            nameText = nameObj.AddComponent<Text>();
            nameText.text = "名称";
            nameText.fontSize = 16;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.UpperLeft;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, -5);

            // 创建饱腹度文本
            GameObject satietyObj = new GameObject("SatietyText");
            satietyObj.transform.SetParent(tooltipPanel.transform, false);
            satietyText = satietyObj.AddComponent<Text>();
            satietyText.text = "饱腹度: 正常";
            satietyText.fontSize = 14;
            satietyText.color = Color.white;
            satietyText.alignment = TextAnchor.UpperLeft;
            RectTransform satietyRect = satietyObj.GetComponent<RectTransform>();
            satietyRect.anchorMin = new Vector2(0, 0);
            satietyRect.anchorMax = new Vector2(1, 0.5f);
            satietyRect.offsetMin = new Vector2(10, 35);
            satietyRect.offsetMax = new Vector2(-10, 5);

            // 创建伪装度文本
            GameObject disguiseObj = new GameObject("DisguiseText");
            disguiseObj.transform.SetParent(tooltipPanel.transform, false);
            disguiseText = disguiseObj.AddComponent<Text>();
            disguiseText.text = "伪装度: 可疑";
            disguiseText.fontSize = 14;
            disguiseText.color = Color.white;
            disguiseText.alignment = TextAnchor.UpperLeft;
            RectTransform disguiseRect = disguiseObj.GetComponent<RectTransform>();
            disguiseRect.anchorMin = new Vector2(0, 0);
            disguiseRect.anchorMax = new Vector2(1, 0.5f);
            disguiseRect.offsetMin = new Vector2(10, 5);
            disguiseRect.offsetMax = new Vector2(-10, -25);
        }
    }
}
