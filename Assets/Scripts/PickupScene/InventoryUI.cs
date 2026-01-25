using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace XEscape.PickupScene
{
    /// <summary>
    /// 背包UI显示
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Transform inventoryPanel;
        [SerializeField] private GameObject slotPrefab;

        [Header("背包管理器")]
        [SerializeField] private InventoryManager inventoryManager;

        [Header("提示消息")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float messageDisplayDuration = 2f; // 提示显示时长

        private List<GameObject> slotObjects = new List<GameObject>();
        private Coroutine messageCoroutine;

        private void Start()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged += UpdateUI;
                inventoryManager.OnPickupFailed += ShowMessage;
                InitializeUI();
                InitializeMessageText();
            }
        }

        private void OnDestroy()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged -= UpdateUI;
                inventoryManager.OnPickupFailed -= ShowMessage;
            }
            
            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
            }
        }

        /// <summary>
        /// 初始化提示文本（如果未分配则自动创建）
        /// </summary>
        private void InitializeMessageText()
        {
            if (messageText == null)
            {
                // 创建一个专门的Canvas用于提示消息，确保显示在最上层
                GameObject canvasObj = new GameObject("MessageCanvas");
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true; // 启用覆盖排序
                canvas.sortingOrder = 999; // 设置非常高的排序顺序，确保在最上层
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // 确保Canvas在场景中不会被销毁
                DontDestroyOnLoad(canvasObj);

                // 创建提示文本对象
                GameObject messageObj = new GameObject("PickupMessageText");
                messageObj.transform.SetParent(canvas.transform, false);
                
                messageText = messageObj.AddComponent<TextMeshProUGUI>();
                
                // 尝试加载默认字体资源
                TMPro.TMP_FontAsset defaultFont = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                if (defaultFont == null)
                {
                    // 如果找不到，尝试使用TMP设置中的默认字体
                    defaultFont = TMPro.TMP_Settings.defaultFontAsset;
                }
                if (defaultFont != null)
                {
                    messageText.font = defaultFont;
                }
                
                messageText.text = "";
                messageText.fontSize = 48; // 增大字体，更容易看到
                messageText.color = new Color(1f, 0.2f, 0.2f, 1f); // 更鲜艳的红色
                messageText.alignment = TextAlignmentOptions.Center;
                messageText.fontStyle = FontStyles.Bold; // 加粗
                
                // 添加轮廓效果，使文字更清晰可见
                messageText.outlineWidth = 0.2f;
                messageText.outlineColor = Color.black;
                
                RectTransform rectTransform = messageObj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 150); // 屏幕中央偏上
                rectTransform.sizeDelta = new Vector2(800, 80); // 增大尺寸
                
                // 确保在Hierarchy中显示在最上层
                messageObj.transform.SetAsLastSibling();
                
                messageText.gameObject.SetActive(false);
                
                Debug.Log("[InventoryUI] 提示消息文本已创建，Canvas sortingOrder = 999");
            }
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        private void ShowMessage(string message)
        {
            if (messageText == null)
            {
                InitializeMessageText();
            }

            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
                
                // 确保文本在Canvas的最上层
                messageText.transform.SetAsLastSibling();
                
                // 确保Canvas在最上层
                Canvas canvas = messageText.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvas.sortingOrder = 999;
                    canvas.overrideSorting = true;
                }

                Debug.Log($"[InventoryUI] 显示提示消息: {message}");

                // 停止之前的协程
                if (messageCoroutine != null)
                {
                    StopCoroutine(messageCoroutine);
                }

                // 启动新的协程来隐藏消息
                messageCoroutine = StartCoroutine(HideMessageAfterDelay());
            }
            else
            {
                Debug.LogError("[InventoryUI] messageText 为 null，无法显示提示消息");
            }
        }

        /// <summary>
        /// 延迟隐藏消息
        /// </summary>
        private IEnumerator HideMessageAfterDelay()
        {
            yield return new WaitForSeconds(messageDisplayDuration);
            
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
                messageText.text = "";
            }
            
            messageCoroutine = null;
        }

        private void InitializeUI()
        {
            // 设置背包面板大小和位置
            if (inventoryPanel != null)
            {
                RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
                if (panelRect != null)
                {
                    // 设置大小
                    panelRect.sizeDelta = new Vector2(225, 40);
                    
                    // 设置锚点到右侧
                    panelRect.anchorMin = new Vector2(1, 0);
                    panelRect.anchorMax = new Vector2(1, 0);
                    panelRect.pivot = new Vector2(1, 0);
                    
                    // 设置位置（贴到右边缘，底部偏移50）
                    panelRect.anchoredPosition = new Vector2(0, 50);
                }

                // 设置布局组的内边距和间距
                HorizontalLayoutGroup layoutGroup = inventoryPanel.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup != null)
                {
                    layoutGroup.padding = new RectOffset(4, 4, 4, 4);
                    layoutGroup.spacing = 4;
                }
            }

            // 创建UI槽位
            var inventory = inventoryManager.GetInventory();
            for (int i = 0; i < inventory.Count; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, inventoryPanel);
                slotObjects.Add(slotObj);

                // 添加点击事件
                int index = i;
                Button button = slotObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnSlotClicked(index));
                }
            }

            UpdateUI(inventory);
        }

        private void UpdateUI(List<InventoryManager.InventorySlot> inventory)
        {
            // 确保背包面板大小和位置正确
            if (inventoryPanel != null)
            {
                RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
                if (panelRect != null)
                {
                    panelRect.sizeDelta = new Vector2(225, 40);
                    panelRect.anchorMin = new Vector2(1, 0);
                    panelRect.anchorMax = new Vector2(1, 0);
                    panelRect.pivot = new Vector2(1, 0);
                    panelRect.anchoredPosition = new Vector2(0, 50);
                }
            }

            for (int i = 0; i < slotObjects.Count && i < inventory.Count; i++)
            {
                UpdateSlot(slotObjects[i], inventory[i]);
            }
        }

        private void UpdateSlot(GameObject slotObj, InventoryManager.InventorySlot slot)
        {
            // 更新槽位显示
            Image bgImage = slotObj.GetComponent<Image>();
            TextMeshProUGUI text = slotObj.GetComponentInChildren<TextMeshProUGUI>();

            if (slot.isEmpty)
            {
                if (bgImage != null) bgImage.color = Color.gray;
                if (text != null) text.text = "空";
            }
            else
            {
                // 根据物品类型设置颜色
                if (bgImage != null)
                {
                    switch (slot.itemType)
                    {
                        case ItemType.Food:
                            bgImage.color = Color.green;
                            break;
                        case ItemType.Fuel:
                            bgImage.color = Color.yellow;
                            break;
                        case ItemType.Medicine:
                            bgImage.color = Color.red;
                            break;
                    }
                }

                if (text != null)
                {
                    string itemName = GetItemName(slot.itemType);
                    text.text = $"{itemName}\n{slot.amount:F0}";
                }
            }
        }

        private string GetItemName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Food:
                    return "食物";
                case ItemType.Fuel:
                    return "油料";
                case ItemType.Medicine:
                    return "药品";
                default:
                    return "未知";
            }
        }

        private void OnSlotClicked(int slotIndex)
        {
            if (inventoryManager != null)
            {
                inventoryManager.UseItem(slotIndex);
            }
        }
    }
}
