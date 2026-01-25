using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using XEscape.CarScene;

namespace XEscape.Editor
{
    /// <summary>
    /// 编辑器工具：创建视角切换按钮
    /// </summary>
    public class ViewSwitcherCreator : EditorWindow
    {
        [MenuItem("Tools/X-Escape/创建视角切换按钮")]
        public static void ShowWindow()
        {
            GetWindow<ViewSwitcherCreator>("创建视角切换按钮");
        }

        private void OnGUI()
        {
            GUILayout.Label("视角切换按钮创建工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("创建视角切换按钮", GUILayout.Height(30)))
            {
                CreateViewSwitchButton();
            }

            GUILayout.Space(10);
            GUILayout.Label("说明：", EditorStyles.boldLabel);
            GUILayout.Label("• 会在场景右下角创建切换按钮");
            GUILayout.Label("• 需要手动配置 ViewSwitcher 组件的引用");
        }

        private void CreateViewSwitchButton()
        {
            // 查找或创建Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("ViewSwitchCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建按钮
            GameObject buttonObj = new GameObject("ViewSwitchButton");
            buttonObj.transform.SetParent(canvas.transform, false);

            // 设置按钮位置（右下角）
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.pivot = new Vector2(1f, 0f);
            buttonRect.sizeDelta = new Vector2(120, 40);
            buttonRect.anchoredPosition = new Vector2(-20, 20); // 距离右下角20像素

            // 添加Image组件（按钮背景）
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 0.9f);

            // 添加Button组件
            Button button = buttonObj.AddComponent<Button>();

            // 创建按钮文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = "车前窗";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 16;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            // 创建ViewSwitcher控制器
            GameObject controller = new GameObject("ViewSwitcher");
            ViewSwitcher viewSwitcher = controller.AddComponent<ViewSwitcher>();

            // 使用反射设置字段
            var buttonField = typeof(ViewSwitcher).GetField("switchButton",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            buttonField?.SetValue(viewSwitcher, button);

            var textField = typeof(ViewSwitcher).GetField("buttonText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            textField?.SetValue(viewSwitcher, buttonText);

            EditorUtility.DisplayDialog("完成",
                "视角切换按钮已创建！\n\n" +
                "下一步：\n" +
                "1. 选中 ViewSwitcher GameObject\n" +
                "2. 在 Inspector 中配置：\n" +
                "   - Interior View: 拖拽车内视角GameObject\n" +
                "   - Front Window Background: 设置车前窗背景图（可选）\n" +
                "   - Main Camera: 拖拽主相机（可选，会自动查找）",
                "确定");

            Selection.activeGameObject = controller;
        }
    }
}
