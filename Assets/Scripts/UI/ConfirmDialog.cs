using UnityEngine;
using UnityEngine.UI;
using System;

namespace XEscape.UI
{
    /// <summary>
    /// 确认对话框
    /// </summary>
    public class ConfirmDialog : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private Action onConfirm;

        private void Awake()
        {
            CreateDialog();
        }

        private void CreateDialog()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasObj = new GameObject("DialogCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 250;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }

            if (dialogPanel == null)
            {
                dialogPanel = new GameObject("DialogPanel");
                dialogPanel.transform.SetParent(canvas.transform, false);

                RectTransform panelRect = dialogPanel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(400, 200);
                panelRect.anchoredPosition = Vector2.zero;
                
                // 确保面板有 RectMask2D 来裁剪子对象
                RectMask2D mask = dialogPanel.AddComponent<RectMask2D>();

                Image panelImage = dialogPanel.AddComponent<Image>();
                panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

                // 消息文字
                GameObject msgObj = new GameObject("MessageText");
                msgObj.transform.SetParent(dialogPanel.transform, false);
                messageText = msgObj.AddComponent<Text>();
                messageText.text = "是否出发？";
                messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                messageText.fontSize = 28;
                messageText.color = Color.white;
                messageText.alignment = TextAnchor.MiddleCenter;

                RectTransform msgRect = msgObj.GetComponent<RectTransform>();
                msgRect.anchorMin = new Vector2(0, 0.6f);
                msgRect.anchorMax = new Vector2(1, 0.95f);
                msgRect.sizeDelta = Vector2.zero;
                msgRect.offsetMin = new Vector2(20, 0);
                msgRect.offsetMax = new Vector2(-20, 0);

                // 确认按钮
                GameObject confirmObj = new GameObject("ConfirmButton");
                confirmObj.transform.SetParent(dialogPanel.transform, false);
                confirmButton = confirmObj.AddComponent<Button>();
                Image confirmImage = confirmObj.AddComponent<Image>();
                confirmImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

                RectTransform confirmRect = confirmObj.GetComponent<RectTransform>();
                confirmRect.anchorMin = new Vector2(0, 0);
                confirmRect.anchorMax = new Vector2(0.5f, 0.4f);
                confirmRect.sizeDelta = Vector2.zero;
                confirmRect.offsetMin = new Vector2(20, 20);
                confirmRect.offsetMax = new Vector2(-10, -20);

                GameObject confirmTextObj = new GameObject("Text");
                confirmTextObj.transform.SetParent(confirmObj.transform, false);
                Text confirmText = confirmTextObj.AddComponent<Text>();
                confirmText.text = "确认";
                confirmText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                confirmText.fontSize = 24;
                confirmText.color = Color.white;
                confirmText.alignment = TextAnchor.MiddleCenter;

                RectTransform confirmTextRect = confirmTextObj.GetComponent<RectTransform>();
                confirmTextRect.anchorMin = Vector2.zero;
                confirmTextRect.anchorMax = Vector2.one;
                confirmTextRect.sizeDelta = Vector2.zero;

                // 取消按钮
                GameObject cancelObj = new GameObject("CancelButton");
                cancelObj.transform.SetParent(dialogPanel.transform, false);
                cancelButton = cancelObj.AddComponent<Button>();
                Image cancelImage = cancelObj.AddComponent<Image>();
                cancelImage.color = new Color(0.6f, 0.2f, 0.2f, 1f);

                RectTransform cancelRect = cancelObj.GetComponent<RectTransform>();
                cancelRect.anchorMin = new Vector2(0.5f, 0);
                cancelRect.anchorMax = new Vector2(1, 0.4f);
                cancelRect.sizeDelta = Vector2.zero;
                cancelRect.offsetMin = new Vector2(10, 20);
                cancelRect.offsetMax = new Vector2(-20, -20);

                GameObject cancelTextObj = new GameObject("Text");
                cancelTextObj.transform.SetParent(cancelObj.transform, false);
                Text cancelText = cancelTextObj.AddComponent<Text>();
                cancelText.text = "取消";
                cancelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                cancelText.fontSize = 24;
                cancelText.color = Color.white;
                cancelText.alignment = TextAnchor.MiddleCenter;

                RectTransform cancelTextRect = cancelTextObj.GetComponent<RectTransform>();
                cancelTextRect.anchorMin = Vector2.zero;
                cancelTextRect.anchorMax = Vector2.one;
                cancelTextRect.sizeDelta = Vector2.zero;

                confirmButton.onClick.AddListener(OnConfirm);
                cancelButton.onClick.AddListener(OnCancel);
            }

            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
        }

        public void Show(string message, Action onConfirmCallback)
        {
            onConfirm = onConfirmCallback;
            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
            }
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
            }
        }

        private void OnConfirm()
        {
            onConfirm?.Invoke();
            Hide();
        }

        private void OnCancel()
        {
            Hide();
        }

        private void Hide()
        {
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
        }
    }
}
