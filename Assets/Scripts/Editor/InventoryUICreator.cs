using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using XEscape.UI;

namespace XEscape.Editor
{
    /// <summary>
    /// 编辑器工具：自动创建背包UI
    /// </summary>
    public class InventoryUICreator : EditorWindow
    {
        [MenuItem("Tools/X-Escape/创建背包UI")]
        public static void ShowWindow()
        {
            GetWindow<InventoryUICreator>("创建背包UI");
        }

        private void OnGUI()
        {
            GUILayout.Label("背包UI创建工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("创建完整背包UI", GUILayout.Height(30)))
            {
                CreateInventoryUI();
            }

            GUILayout.Space(10);
            GUILayout.Label("说明：", EditorStyles.boldLabel);
            GUILayout.Label("• 会自动创建Canvas和所有UI元素");
            GUILayout.Label("• 需要手动配置 InventoryUI 组件的引用");
            GUILayout.Label("• 建议先创建 InventoryManager GameObject");
        }

        private void CreateInventoryUI()
        {
            // 查找或创建Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("InventoryCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建背包面板
            GameObject panel = new GameObject("InventoryPanel");
            panel.transform.SetParent(canvas.transform, false);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(600, 400);
            panelRect.anchoredPosition = Vector2.zero;

            // 创建标题
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "背包";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.UpperCenter;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(0, 0);
            titleRect.offsetMax = new Vector2(0, -10);

            // 创建槽位容器
            GameObject slotsContainer = new GameObject("SlotsContainer");
            slotsContainer.transform.SetParent(panel.transform, false);
            RectTransform containerRect = slotsContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.05f, 0.3f);
            containerRect.anchorMax = new Vector2(0.95f, 0.85f);
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            GridLayoutGroup grid = slotsContainer.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(80, 80);
            grid.spacing = new Vector2(10, 10);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;

            // 创建槽位预制体
            GameObject slotPrefab = CreateSlotPrefab();
            slotPrefab.transform.SetParent(slotsContainer.transform, false);

            // 创建信息面板
            GameObject infoPanel = new GameObject("InfoPanel");
            infoPanel.transform.SetParent(panel.transform, false);
            RectTransform infoRect = infoPanel.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0.05f, 0.05f);
            infoRect.anchorMax = new Vector2(0.95f, 0.25f);
            infoRect.offsetMin = Vector2.zero;
            infoRect.offsetMax = Vector2.zero;

            // 物品名称
            GameObject nameObj = new GameObject("ItemNameText");
            nameObj.transform.SetParent(infoPanel.transform, false);
            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = "选择物品查看详情";
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 18;
            nameText.color = Color.white;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // 物品描述
            GameObject descObj = new GameObject("ItemDescriptionText");
            descObj.transform.SetParent(infoPanel.transform, false);
            Text descText = descObj.AddComponent<Text>();
            descText.text = "";
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 14;
            descText.color = Color.white;
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0);
            descRect.anchorMax = new Vector2(1, 0.5f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;

            // 使用按钮
            GameObject useBtn = new GameObject("UseButton");
            useBtn.transform.SetParent(panel.transform, false);
            Image useBtnImage = useBtn.AddComponent<Image>();
            useBtnImage.color = new Color(0.2f, 0.6f, 0.2f);
            Button useButton = useBtn.AddComponent<Button>();
            Text useBtnText = new GameObject("Text").AddComponent<Text>();
            useBtnText.transform.SetParent(useBtn.transform, false);
            useBtnText.text = "使用";
            useBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            useBtnText.fontSize = 16;
            useBtnText.color = Color.white;
            useBtnText.alignment = TextAnchor.MiddleCenter;
            RectTransform useBtnRect = useBtn.GetComponent<RectTransform>();
            useBtnRect.anchorMin = new Vector2(0.7f, 0.05f);
            useBtnRect.anchorMax = new Vector2(0.95f, 0.2f);
            useBtnRect.offsetMin = Vector2.zero;
            useBtnRect.offsetMax = Vector2.zero;
            RectTransform useBtnTextRect = useBtnText.GetComponent<RectTransform>();
            useBtnTextRect.anchorMin = Vector2.zero;
            useBtnTextRect.anchorMax = Vector2.one;
            useBtnTextRect.offsetMin = Vector2.zero;
            useBtnTextRect.offsetMax = Vector2.zero;

            // 关闭按钮
            GameObject closeBtn = new GameObject("CloseButton");
            closeBtn.transform.SetParent(panel.transform, false);
            Image closeBtnImage = closeBtn.AddComponent<Image>();
            closeBtnImage.color = new Color(0.6f, 0.2f, 0.2f);
            Button closeButton = closeBtn.AddComponent<Button>();
            Text closeBtnText = new GameObject("Text").AddComponent<Text>();
            closeBtnText.transform.SetParent(closeBtn.transform, false);
            closeBtnText.text = "关闭";
            closeBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeBtnText.fontSize = 16;
            closeBtnText.color = Color.white;
            closeBtnText.alignment = TextAnchor.MiddleCenter;
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(0.05f, 0.05f);
            closeBtnRect.anchorMax = new Vector2(0.25f, 0.2f);
            closeBtnRect.offsetMin = Vector2.zero;
            closeBtnRect.offsetMax = Vector2.zero;
            RectTransform closeBtnTextRect = closeBtnText.GetComponent<RectTransform>();
            closeBtnTextRect.anchorMin = Vector2.zero;
            closeBtnTextRect.anchorMax = Vector2.one;
            closeBtnTextRect.offsetMin = Vector2.zero;
            closeBtnTextRect.offsetMax = Vector2.zero;

            // 创建 InventoryUIController
            GameObject controller = new GameObject("InventoryUIController");
            InventoryUI inventoryUI = controller.AddComponent<InventoryUI>();

            // 使用反射设置私有字段（因为它们是 SerializeField）
            var panelField = typeof(InventoryUI).GetField("inventoryPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            panelField?.SetValue(inventoryUI, panel);
            
            // 临时激活面板以便查看（运行时会被Awake关闭）
            panel.SetActive(true);

            var slotPrefabField = typeof(InventoryUI).GetField("slotPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            slotPrefabField?.SetValue(inventoryUI, slotPrefab);

            var slotsParentField = typeof(InventoryUI).GetField("slotsParent", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            slotsParentField?.SetValue(inventoryUI, slotsContainer.transform);

            var nameTextField = typeof(InventoryUI).GetField("itemNameText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nameTextField?.SetValue(inventoryUI, nameText);

            var descTextField = typeof(InventoryUI).GetField("itemDescriptionText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            descTextField?.SetValue(inventoryUI, descText);

            var useBtnField = typeof(InventoryUI).GetField("useButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            useBtnField?.SetValue(inventoryUI, useButton);

            var closeBtnField = typeof(InventoryUI).GetField("closeButton", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            closeBtnField?.SetValue(inventoryUI, closeButton);

            // 将槽位预制体保存为预制体
            string prefabPath = "Assets/Prefabs/InventorySlot.prefab";
            PrefabUtility.SaveAsPrefabAsset(slotPrefab, prefabPath);
            DestroyImmediate(slotPrefab);

            // 测试脚本已移除，不再自动创建

            EditorUtility.DisplayDialog("完成", 
                "背包UI已创建！\n\n" +
                "重要说明：\n" +
                "• InventoryPanel 默认是隐藏的（这是正常的）\n" +
                "• 在编辑器中：选中 InventoryPanel，在 Inspector 中勾选激活查看\n" +
                "• 运行游戏后：按 I 键打开背包\n\n" +
                "下一步：\n" +
                "1. 创建 InventoryManager GameObject\n" +
                "2. 运行游戏，按 I 键打开背包\n" +
                "3. 使用 InventoryTester 添加测试物品", 
                "确定");

            Selection.activeGameObject = panel;
            
            // 在编辑器中激活面板以便查看
            panel.SetActive(true);
        }

        private GameObject CreateSlotPrefab()
        {
            GameObject slot = new GameObject("Slot");
            Image slotBg = slot.AddComponent<Image>();
            slotBg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            // Icon
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(slot.transform, false);
            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = new Color(1, 1, 1, 0);
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            // Quantity
            GameObject quantity = new GameObject("Quantity");
            quantity.transform.SetParent(slot.transform, false);
            Text quantityText = quantity.AddComponent<Text>();
            quantityText.text = "";
            quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            quantityText.fontSize = 14;
            quantityText.color = Color.white;
            quantityText.alignment = TextAnchor.LowerRight;
            RectTransform quantityRect = quantity.GetComponent<RectTransform>();
            quantityRect.anchorMin = new Vector2(0.6f, 0);
            quantityRect.anchorMax = new Vector2(1, 0.4f);
            quantityRect.offsetMin = Vector2.zero;
            quantityRect.offsetMax = Vector2.zero;

            slot.AddComponent<InventorySlot>();

            return slot;
        }
    }
}
