using UnityEngine;
using UnityEngine.UI;
using XEscape.CarScene;

namespace XEscape.UI
{
    /// <summary>
    /// 简化版鼠标悬停提示，更可靠的实现
    /// </summary>
    public class SimpleHoverTooltip : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text nameText;
        [SerializeField] private Text satietyText;
        [SerializeField] private Text disguiseText;
        
        [Header("设置")]
        [SerializeField] private float offsetX = 10f;
        [SerializeField] private float offsetY = 10f;
        [Tooltip("需要检测的人物名称关键词（不区分大小写）")]
        [SerializeField] private string[] targetNames = { "father", "mother", "mather", "爸爸", "妈妈" };
        
        [Header("调试")]
        [SerializeField] private bool enableDebugLog = true;
        
        private Camera mainCamera;
        private Canvas canvas;
        private RectTransform tooltipRect;
        private CarOccupant currentOccupant;

        private void Awake()
        {
            Debug.Log("SimpleHoverTooltip: Awake 开始");
            
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();

            if (mainCamera == null)
            {
                Debug.LogError("SimpleHoverTooltip: 未找到相机！");
            }
            else
            {
                Debug.Log($"SimpleHoverTooltip: 找到相机 {mainCamera.name}, 正交模式: {mainCamera.orthographic}");
            }

            // 创建或找到Canvas
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("TooltipCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                Debug.Log("SimpleHoverTooltip: 创建了新Canvas");
            }

            // 创建TooltipPanel
            if (tooltipPanel == null)
            {
                CreateDefaultTooltip();
            }

            if (tooltipPanel != null)
            {
                tooltipRect = tooltipPanel.GetComponent<RectTransform>();
                tooltipPanel.SetActive(false);
                Debug.Log("SimpleHoverTooltip: TooltipPanel 已创建");
            }
        }

        private void Start()
        {
            Debug.Log("SimpleHoverTooltip: Start 被调用");
            
            // 检查所有CarOccupant
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            Debug.Log($"SimpleHoverTooltip: 找到 {occupants.Length} 个CarOccupant");
            
            foreach (var occ in occupants)
            {
                string name = occ.GetName();
                Collider2D col = occ.GetComponent<Collider2D>();
                Debug.Log($"  - {occ.gameObject.name}: Name={name}, HasCollider={col != null}, ColliderType={col?.GetType().Name}");
            }
        }

        private void Update()
        {
            CheckMouseHover();
            UpdateTooltipPosition();
        }

        private void CheckMouseHover()
        {
            if (mainCamera == null) return;

            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            
            // 检测所有CarOccupant，检查鼠标是否在其Collider内
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            CarOccupant hoveredOccupant = null;

            foreach (CarOccupant occupant in occupants)
            {
                // 检查名称是否匹配
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

                if (!isTarget) continue;

                // 检查Collider
                Collider2D col = occupant.GetComponent<Collider2D>();
                if (col != null)
                {
                    // 使用OverlapPoint检测
                    if (col.OverlapPoint(mouseWorldPos))
                    {
                        hoveredOccupant = occupant;
                        break;
                    }
                }
            }

            // 显示或隐藏提示
            if (hoveredOccupant != null)
            {
                if (currentOccupant != hoveredOccupant)
                {
                    ShowTooltip(hoveredOccupant);
                    if (enableDebugLog)
                        Debug.Log($"显示提示: {hoveredOccupant.GetName()}");
                }
            }
            else
            {
                if (currentOccupant != null)
                {
                    HideTooltip();
                }
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            if (mainCamera == null) return Vector3.zero;

            Vector3 mousePos = Input.mousePosition;
            
            if (mainCamera.orthographic)
            {
                float cameraZ = mainCamera.transform.position.z;
                mousePos.z = Mathf.Abs(cameraZ);
            }
            else
            {
                mousePos.z = mainCamera.nearClipPlane + 1f;
            }
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            
            // 2D游戏强制Z=0
            if (mainCamera.orthographic)
            {
                worldPos.z = 0f;
            }
            
            return worldPos;
        }

        private void ShowTooltip(CarOccupant occupant)
        {
            currentOccupant = occupant;
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
                UpdateTooltipContent(occupant);
            }
        }

        private void HideTooltip()
        {
            currentOccupant = null;
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        private void UpdateTooltipContent(CarOccupant occupant)
        {
            if (occupant == null)
            {
                Debug.LogWarning("SimpleHoverTooltip: occupant 为 null");
                return;
            }

            Debug.Log($"SimpleHoverTooltip: 更新提示内容 - {occupant.GetName()}");

            if (nameText != null)
            {
                nameText.text = occupant.GetName();
                Debug.Log($"SimpleHoverTooltip: 名称文本已设置: {nameText.text}");
            }
            else
            {
                Debug.LogWarning("SimpleHoverTooltip: nameText 为 null");
            }

            if (satietyText != null)
            {
                string status = occupant.GetSatietyStatusText();
                float value = occupant.GetSatiety();
                satietyText.text = $"饱腹度: {status} ({value:F0}%)";
                Debug.Log($"SimpleHoverTooltip: 饱腹度文本已设置: {satietyText.text}");
            }
            else
            {
                Debug.LogWarning("SimpleHoverTooltip: satietyText 为 null");
            }

            if (disguiseText != null)
            {
                string status = occupant.GetDisguiseStatusText();
                float value = occupant.GetDisguise();
                disguiseText.text = $"伪装度: {status} ({value:F0}%)";
                Debug.Log($"SimpleHoverTooltip: 伪装度文本已设置: {disguiseText.text}");
            }
            else
            {
                Debug.LogWarning("SimpleHoverTooltip: disguiseText 为 null");
            }
        }

        private void UpdateTooltipPosition()
        {
            if (tooltipPanel == null || tooltipRect == null || !tooltipPanel.activeSelf)
                return;

            Vector2 mousePos = Input.mousePosition;
            Vector2 tooltipSize = tooltipRect.sizeDelta;

            float x = mousePos.x + offsetX;
            float y = mousePos.y + offsetY;

            if (x + tooltipSize.x > Screen.width)
                x = mousePos.x - tooltipSize.x - offsetX;

            if (y + tooltipSize.y > Screen.height)
                y = mousePos.y - tooltipSize.y - offsetY;

            tooltipRect.position = new Vector3(x, y, 0);
        }

        private void CreateDefaultTooltip()
        {
            if (canvas == null) return;

            tooltipPanel = new GameObject("TooltipPanel");
            tooltipPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = tooltipPanel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(250, 120);

            Image panelImage = tooltipPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.85f);

            // 名称文本
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(tooltipPanel.transform, false);
            nameText = nameObj.AddComponent<Text>();
            nameText.text = "名称";
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 18;
            nameText.fontStyle = FontStyle.Bold;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.UpperLeft;
            nameText.horizontalOverflow = HorizontalWrapMode.Overflow;
            nameText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.66f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, -5);

            // 饱腹度文本
            GameObject satietyObj = new GameObject("SatietyText");
            satietyObj.transform.SetParent(tooltipPanel.transform, false);
            satietyText = satietyObj.AddComponent<Text>();
            satietyText.text = "饱腹度: 正常";
            satietyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            satietyText.fontSize = 14;
            satietyText.color = Color.white;
            satietyText.alignment = TextAnchor.UpperLeft;
            satietyText.horizontalOverflow = HorizontalWrapMode.Overflow;
            satietyText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform satietyRect = satietyObj.GetComponent<RectTransform>();
            satietyRect.anchorMin = new Vector2(0, 0.33f);
            satietyRect.anchorMax = new Vector2(1, 0.66f);
            satietyRect.offsetMin = new Vector2(10, 5);
            satietyRect.offsetMax = new Vector2(-10, -5);

            // 伪装度文本
            GameObject disguiseObj = new GameObject("DisguiseText");
            disguiseObj.transform.SetParent(tooltipPanel.transform, false);
            disguiseText = disguiseObj.AddComponent<Text>();
            disguiseText.text = "伪装度: 可疑";
            disguiseText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            disguiseText.fontSize = 14;
            disguiseText.color = Color.white;
            disguiseText.alignment = TextAnchor.UpperLeft;
            disguiseText.horizontalOverflow = HorizontalWrapMode.Overflow;
            disguiseText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform disguiseRect = disguiseObj.GetComponent<RectTransform>();
            disguiseRect.anchorMin = new Vector2(0, 0);
            disguiseRect.anchorMax = new Vector2(1, 0.33f);
            disguiseRect.offsetMin = new Vector2(10, 5);
            disguiseRect.offsetMax = new Vector2(-10, -5);
        }
    }
}
