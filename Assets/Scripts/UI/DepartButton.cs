using UnityEngine;
using UnityEngine.UI;
using XEscape.Managers;

namespace XEscape.UI
{
    /// <summary>
    /// 出发按钮，显示在车前窗场景
    /// </summary>
    public class DepartButton : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Button departButton;
        [SerializeField] private Text buttonText;

        [Header("引用")]
        [SerializeField] private ConfirmDialog confirmDialog;

        private DayManager dayManager;

        private void Awake()
        {
            dayManager = FindFirstObjectByType<DayManager>();
            CreateButton();
            
            // 自动查找确认对话框
            if (confirmDialog == null)
            {
                confirmDialog = FindFirstObjectByType<ConfirmDialog>();
            }
        }

        private void CreateButton()
        {
            // 查找或创建Canvas
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    GameObject canvasObj = new GameObject("DepartButtonCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 150;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }

            // 创建按钮
            if (departButton == null)
            {
                GameObject buttonObj = new GameObject("DepartButton");
                buttonObj.transform.SetParent(canvas.transform, false);
                
                RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
                buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
                buttonRect.sizeDelta = new Vector2(200, 60);
                buttonRect.anchoredPosition = Vector2.zero;

                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

                departButton = buttonObj.AddComponent<Button>();

                // 创建按钮文字
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                buttonText = textObj.AddComponent<Text>();
                buttonText.text = "出发";
                buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                buttonText.fontSize = 32;
                buttonText.color = Color.white;
                buttonText.alignment = TextAnchor.MiddleCenter;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
            }

            departButton.onClick.AddListener(OnDepartClick);
            
            // 初始状态隐藏
            if (departButton != null)
            {
                departButton.gameObject.SetActive(false);
            }
        }

        private void OnDepartClick()
        {
            if (confirmDialog != null)
            {
                confirmDialog.Show("是否进入下一天？", OnConfirm);
            }
            else
            {
                OnConfirm();
            }
        }

        private void OnConfirm()
        {
            if (dayManager != null)
            {
                // 延迟一下再进入下一天，让确认对话框先关闭
                // NextDay() 内部会判断是游戏结束还是通关
                StartCoroutine(DelayedNextDay());
            }
        }

        private System.Collections.IEnumerator DelayedNextDay()
        {
            yield return new WaitForSeconds(0.2f); // 等待对话框关闭动画
            if (dayManager != null)
            {
                dayManager.NextDay();
            }
        }

        public void Show()
        {
            if (departButton != null)
            {
                departButton.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (departButton != null)
            {
                departButton.gameObject.SetActive(false);
            }
        }
    }
}
