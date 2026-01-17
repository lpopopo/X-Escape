using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XEscape.Managers;
using XEscape.CarScene;

namespace XEscape.UI
{
    /// <summary>
    /// 天数标题显示，显示"第X天"文字
    /// </summary>
    public class DayTitleDisplay : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private Text titleText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("设置")]
        [SerializeField] private float displayDuration = 2f; // 显示时长（秒）
        [SerializeField] private float fadeInDuration = 0.5f; // 淡入时长
        [SerializeField] private float fadeOutDuration = 0.5f; // 淡出时长

        private DayManager dayManager;
        private Coroutine fadeCoroutine;
        private SimpleHoverTooltip hoverTooltip;
        private bool isShowing = false;

        private void Awake()
        {
            dayManager = FindFirstObjectByType<DayManager>();
            hoverTooltip = FindFirstObjectByType<SimpleHoverTooltip>();
            CreateUI();
        }

        private void Start()
        {
            dayManager = FindFirstObjectByType<DayManager>();
            if (dayManager != null)
            {
                // 第一天直接显示，不渐显
                ShowDayTitleImmediate(dayManager.GetCurrentDay());
            }
        }

        private void OnEnable()
        {
            if (dayManager != null)
            {
                dayManager.OnDayChanged.AddListener(ShowDayTitle);
                dayManager.OnGameEnd.AddListener(ShowGameOver);
                dayManager.OnGameWin.AddListener(ShowGameWin);
            }
        }

        private void OnDisable()
        {
            if (dayManager != null)
            {
                dayManager.OnDayChanged.RemoveListener(ShowDayTitle);
                dayManager.OnGameEnd.RemoveListener(ShowGameOver);
                dayManager.OnGameWin.RemoveListener(ShowGameWin);
            }
        }

        private void CreateUI()
        {
            // 创建Canvas（如果不存在）
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("DayTitleCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 200; // 确保在最上层
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建标题面板
            if (titlePanel == null)
            {
                titlePanel = new GameObject("TitlePanel");
                titlePanel.transform.SetParent(canvas.transform, false);
                
                RectTransform panelRect = titlePanel.AddComponent<RectTransform>();
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.sizeDelta = Vector2.zero;

                Image panelImage = titlePanel.AddComponent<Image>();
                panelImage.color = Color.black;

                // 创建文字
                GameObject textObj = new GameObject("TitleText");
                textObj.transform.SetParent(titlePanel.transform, false);
                titleText = textObj.AddComponent<Text>();
                titleText.text = "第1天";
                titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                titleText.fontSize = 72;
                titleText.color = Color.white;
                titleText.alignment = TextAnchor.MiddleCenter;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                textRect.sizeDelta = new Vector2(800, 200); // 增大文字区域以容纳"结局：无人幸存"
                textRect.anchoredPosition = Vector2.zero;
            }

            // 添加 CanvasGroup 用于控制透明度
            if (canvasGroup == null && titlePanel != null)
            {
                canvasGroup = titlePanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = titlePanel.AddComponent<CanvasGroup>();
                }
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            titlePanel.SetActive(false);
        }

        public void ShowDayTitle(int day)
        {
            if (titleText != null)
            {
                titleText.text = $"第{day}天";
            }

            if (titlePanel != null)
            {
                titlePanel.SetActive(true);
                isShowing = true;
                
                // 禁用交互
                DisableInteractions();
                
                // 停止之前的协程
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                // 设置初始透明度为0（渐显）
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                }
                
                // 开始淡入淡出动画
                fadeCoroutine = StartCoroutine(FadeInOutSequence());
            }
        }

        /// <summary>
        /// 立即显示天数标题（第一天使用，不渐显）
        /// </summary>
        private void ShowDayTitleImmediate(int day)
        {
            if (titleText != null)
            {
                titleText.text = $"第{day}天";
            }

            if (titlePanel != null)
            {
                titlePanel.SetActive(true);
                isShowing = true;
                
                // 禁用交互
                DisableInteractions();
                
                // 停止之前的协程
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                // 立即设置为完全不透明
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
                
                // 只执行保持和淡出
                fadeCoroutine = StartCoroutine(FadeOutSequence());
            }
        }

        private IEnumerator FadeOutSequence()
        {
            if (canvasGroup == null) yield break;

            // 保持显示
            yield return new WaitForSeconds(displayDuration);

            // 淡出
            yield return StartCoroutine(FadeTo(1f, 0f, fadeOutDuration));

            // 隐藏面板
            if (titlePanel != null)
            {
                titlePanel.SetActive(false);
            }

            // 重新启用交互
            isShowing = false;
            EnableInteractions();
        }

        private IEnumerator FadeInOutSequence()
        {
            if (canvasGroup == null) yield break;

            // 淡入
            yield return StartCoroutine(FadeTo(0f, 1f, fadeInDuration));

            // 保持显示
            yield return new WaitForSeconds(displayDuration);

            // 淡出
            yield return StartCoroutine(FadeTo(1f, 0f, fadeOutDuration));

            // 隐藏面板
            if (titlePanel != null)
            {
                titlePanel.SetActive(false);
            }

            // 重新启用交互
            isShowing = false;
            EnableInteractions();
        }

        private IEnumerator FadeTo(float startAlpha, float endAlpha, float duration)
        {
            if (canvasGroup == null) yield break;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }

        private void HideTitle()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (titlePanel != null)
            {
                StartCoroutine(FadeOutAndHide());
            }
        }

        private IEnumerator FadeOutAndHide()
        {
            if (canvasGroup == null)
            {
                if (titlePanel != null)
                {
                    titlePanel.SetActive(false);
                }
                yield break;
            }

            // 淡出
            yield return StartCoroutine(FadeTo(canvasGroup.alpha, 0f, fadeOutDuration));

            // 隐藏面板
            if (titlePanel != null)
            {
                titlePanel.SetActive(false);
            }
        }

        private void DisableInteractions()
        {
            // 禁用鼠标悬停提示
            if (hoverTooltip != null)
            {
                hoverTooltip.enabled = false;
            }
        }

        private void EnableInteractions()
        {
            // 启用鼠标悬停提示
            if (hoverTooltip != null)
            {
                hoverTooltip.enabled = true;
            }
        }

        public bool IsShowing() => isShowing;

        public void ShowGameEnd()
        {
            ShowGameOver();
        }

        /// <summary>
        /// 显示游戏结束（所有角色死亡）
        /// </summary>
        private void ShowGameOver()
        {
            if (titlePanel != null)
            {
                titlePanel.SetActive(true);
                isShowing = true;
                
                // 禁用交互
                DisableInteractions();
                
                // 停止之前的协程
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                // 开始显示序列：先显示"结束"，然后渐隐渐显显示"结局：无人幸存"
                fadeCoroutine = StartCoroutine(ShowGameOverSequence());
            }
        }

        private IEnumerator ShowGameOverSequence()
        {
            if (canvasGroup == null || titleText == null) yield break;

            // 在显示"结束"之前就隐藏车内场景和车前窗场景，避免背景显示
            ViewSwitcher viewSwitcher = FindFirstObjectByType<ViewSwitcher>();
            if (viewSwitcher != null)
            {
                GameObject interiorView = viewSwitcher.GetInteriorView();
                GameObject frontWindowView = viewSwitcher.GetFrontWindowView();
                
                if (interiorView != null)
                {
                    interiorView.SetActive(false);
                }
                if (frontWindowView != null)
                {
                    frontWindowView.SetActive(false);
                }
                
                // 隐藏切换视角按钮
                viewSwitcher.HideSwitchButton();
            }

            // 隐藏出发按钮
            DepartButton departButton = FindFirstObjectByType<DepartButton>();
            if (departButton != null)
            {
                departButton.Hide();
            }

            // 隐藏所有其他UI元素（查找所有Canvas下的Button）
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                // 排除标题面板内的按钮（如果有）
                if (btn.transform.parent != null && btn.transform.parent.gameObject != titlePanel)
                {
                    btn.gameObject.SetActive(false);
                }
            }

            // 第一步：显示"结束"
            titleText.text = "结束";
            canvasGroup.alpha = 0f;
            yield return StartCoroutine(FadeTo(0f, 1f, fadeInDuration));

            // 保持显示"结束"
            yield return new WaitForSeconds(1f);

            // 第二步：淡出"结束"
            yield return StartCoroutine(FadeTo(1f, 0f, fadeOutDuration));

            // 第三步：淡入显示"结局：无人幸存"
            titleText.text = "结局：无人幸存";
            yield return StartCoroutine(FadeTo(0f, 1f, fadeInDuration));

            // 保持显示，不自动隐藏
        }

        /// <summary>
        /// 显示通关（第10天还有角色幸存）
        /// </summary>
        private void ShowGameWin()
        {
            if (titleText != null)
            {
                titleText.text = "通关";
            }

            if (titlePanel != null)
            {
                titlePanel.SetActive(true);
                isShowing = true;
                
                // 禁用交互
                DisableInteractions();
                
                // 停止之前的协程
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                // 淡入显示（不自动隐藏）
                if (canvasGroup != null)
                {
                    fadeCoroutine = StartCoroutine(FadeTo(0f, 1f, fadeInDuration));
                }
                else
                {
                    titlePanel.SetActive(true);
                }
            }
        }
    }
}
