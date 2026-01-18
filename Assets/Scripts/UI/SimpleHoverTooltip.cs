using UnityEngine;
using UnityEngine.UI;
using XEscape.CarScene;
using XEscape.Inventory;

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
        [SerializeField] private bool enableDebugLog = false; // 调试日志开关
        
        private Camera mainCamera;
        private Canvas canvas;
        private RectTransform tooltipRect;
        private CarOccupant currentOccupant;
        private StockItem currentItem;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();

            if (mainCamera == null)
            {
                Debug.LogError("SimpleHoverTooltip: 未找到相机！");
            }

            // 创建或找到Canvas
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                // 尝试查找场景中已有的 Canvas
                canvas = FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasObj = new GameObject("TooltipCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 100; // 确保在最上层
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
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
            }
            else
            {
                Debug.LogError("SimpleHoverTooltip: TooltipPanel 创建失败！");
            }
        }

        private void Start()
        {
            // 初始化完成，无需日志
        }

        private void Update()
        {
            // 检查是否有天数标题正在显示，如果有则禁用交互
            DayTitleDisplay titleDisplay = FindFirstObjectByType<DayTitleDisplay>();
            if (titleDisplay != null && titleDisplay.IsShowing())
            {
                if (tooltipPanel != null && tooltipPanel.activeSelf)
                {
                    tooltipPanel.SetActive(false);
                }
                return;
            }

            CheckMouseHover();
            UpdateTooltipPosition();
        }

        private void CheckMouseHover()
        {
            if (mainCamera == null)
            {
                if (enableDebugLog)
                    Debug.LogWarning("SimpleHoverTooltip: mainCamera 为 null");
                return;
            }

            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            
            // 优先检测物品（食物/伪装物品）
            StockItem hoveredItem = null;
            StockItem[] allItems = FindObjectsByType<StockItem>(FindObjectsSortMode.None);
            
            foreach (StockItem item in allItems)
            {
                if (item == null || !item.gameObject.activeInHierarchy) continue;
                
                Collider2D col = item.GetComponent<Collider2D>();
                if (col != null && col.enabled && col.OverlapPoint(mouseWorldPos))
                {
                    hoveredItem = item;
                    break;
                }
            }
            
            // 如果悬停在物品上，显示物品库存信息
            if (hoveredItem != null)
            {
                if (currentItem != hoveredItem)
                {
                    ShowItemTooltip(hoveredItem);
                }
                return;
            }
            
            // 否则检测角色
            CarOccupant[] occupants = FindObjectsByType<CarOccupant>(FindObjectsSortMode.None);
            CarOccupant hoveredOccupant = null;

            foreach (CarOccupant occupant in occupants)
            {
                if (occupant == null || !occupant.gameObject.activeInHierarchy) continue;
                if (occupant.IsDead()) continue;
                
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

                Collider2D col = occupant.GetComponent<Collider2D>();
                if (col == null || !col.enabled) continue;
                
                if (col.OverlapPoint(mouseWorldPos))
                {
                    hoveredOccupant = occupant;
                    break;
                }
            }

            // 显示或隐藏提示
            if (hoveredOccupant != null)
            {
                if (currentOccupant != hoveredOccupant)
                {
                    ShowTooltip(hoveredOccupant);
                }
            }
            else
            {
                if (currentOccupant != null || currentItem != null)
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
            currentItem = null;
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
                UpdateTooltipContent(occupant);
            }
        }
        
        private void ShowItemTooltip(StockItem item)
        {
            currentItem = item;
            currentOccupant = null;
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
                UpdateItemTooltipContent(item);
            }
        }

        private void HideTooltip()
        {
            currentOccupant = null;
            currentItem = null;
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        private void UpdateTooltipContent(CarOccupant occupant)
        {
            if (occupant == null) return;

            if (nameText != null)
            {
                nameText.text = occupant.GetName();
            }

            if (satietyText != null)
            {
                string status = occupant.GetSatietyStatusText();
                float value = occupant.GetSatiety();
                satietyText.text = $"饱腹度: {status} ({value:F0}%)";
            }

            if (disguiseText != null)
            {
                string status = occupant.GetDisguiseStatusText();
                float value = occupant.GetDisguise();
                disguiseText.text = $"伪装度: {status} ({value:F0}%)";
            }
        }
        
        private void UpdateItemTooltipContent(StockItem item)
        {
            if (item == null) return;
            
            Managers.ItemManager itemManager = FindFirstObjectByType<Managers.ItemManager>();
            int foodStock = itemManager != null ? itemManager.GetFoodStock() : 0;
            int disguiseStock = itemManager != null ? itemManager.GetDisguiseStock() : 0;
            
            ItemType itemType = item.GetItemType();
            
            if (nameText != null)
            {
                nameText.text = itemType == ItemType.Food ? "食物" : "伪装物品";
            }
            
            if (satietyText != null)
            {
                if (itemType == ItemType.Food)
                {
                    satietyText.text = $"食物库存: {foodStock}";
                }
                else
                {
                    satietyText.text = $"伪装物品库存: {disguiseStock}";
                }
            }
            
            if (disguiseText != null)
            {
                disguiseText.text = "";
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
