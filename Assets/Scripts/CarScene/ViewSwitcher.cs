using UnityEngine;
using UnityEngine.UI;
using XEscape.UI;

namespace XEscape.CarScene
{
    /// <summary>
    /// 视角切换控制器，用于在车内视角和车前窗视角之间切换
    /// </summary>
    public class ViewSwitcher : MonoBehaviour
    {
        [Header("视角设置")]
        [SerializeField] private ViewType currentView = ViewType.Interior;
        
        [Header("车内视角")]
        [SerializeField] private GameObject interiorView;
        
        [Header("车前窗视角")]
        [SerializeField] private GameObject frontWindowView;

        // 公共方法：获取视角对象
        public GameObject GetInteriorView() => interiorView;
        public GameObject GetFrontWindowView() => frontWindowView;
        
        // 公共方法：隐藏/显示切换按钮
        public void HideSwitchButton()
        {
            if (switchButton != null)
            {
                switchButton.gameObject.SetActive(false);
            }
        }
        
        public void ShowSwitchButton()
        {
            if (switchButton != null)
            {
                switchButton.gameObject.SetActive(true);
            }
        }
        
        [Header("UI按钮")]
        [SerializeField] private Button switchButton;
        [SerializeField] private Text buttonText;
        
        [Header("相机设置")]
        [SerializeField] private Camera mainCamera;

        [Header("出发按钮")]
        [SerializeField] private DepartButton departButton;
        
        private bool isInitialized = false;

        /// <summary>
        /// 视角类型枚举
        /// </summary>
        public enum ViewType
        {
            Interior,      // 车内视角
            FrontWindow    // 车前窗视角
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            // 确保初始状态正确（默认车内视角）
            currentView = ViewType.Interior;
            SwitchView(ViewType.Interior);
            
            // 确保 EventSystem 存在（UI 按钮点击必需）
            UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // 再次确认按钮连接（防止 Inspector 中的设置覆盖代码设置）
            if (switchButton != null)
            {
                switchButton.onClick.RemoveAllListeners();
                switchButton.onClick.AddListener(ToggleView);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            if (isInitialized) return;

            // 获取主相机
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                    mainCamera = FindFirstObjectByType<Camera>();
            }

            // 查找或创建车内视角
            if (interiorView == null)
            {
                // 方法1：查找有 CarInteriorView 组件的对象
                CarInteriorView interiorViewScript = FindFirstObjectByType<CarInteriorView>();
                if (interiorViewScript != null)
                {
                    interiorView = interiorViewScript.gameObject;
                }
                else
                {
                    // 方法2：查找包含 "interior"、"车内"、"car" 的对象
                    GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                    foreach (GameObject obj in allObjects)
                    {
                        string name = obj.name.ToLower();
                        if ((name.Contains("interior") || name.Contains("车内") || name.Contains("carinternal")) 
                            && obj.transform.childCount > 0) // 确保是父对象
                        {
                            interiorView = obj;
                            break;
                        }
                    }
                    
                    if (interiorView == null)
                    {
                        Debug.LogWarning("ViewSwitcher: 未找到车内场景！请手动设置 Interior View 字段。");
                    }
                }
            }

            // 查找车前窗视角（如果未手动设置）
            if (frontWindowView == null)
            {
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (GameObject obj in allObjects)
                {
                    string name = obj.name.ToLower();
                    if (name.Contains("frontwindow") || name.Contains("车前窗") || 
                        name.Contains("carfrontwindow") || name.Contains("frontwindowview"))
                    {
                        frontWindowView = obj;
                        break;
                    }
                }
                
                if (frontWindowView == null)
                {
                    Debug.LogWarning("ViewSwitcher: Front Window View 未设置！请手动设置 Front Window View 字段。");
                }
            }

            // 设置按钮 - 如果未设置，尝试自动查找
            if (switchButton == null)
            {
                // 尝试查找 ViewSwitchButton
                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.Contains("ViewSwitch") || obj.name.Contains("SwitchButton"))
                    {
                        switchButton = obj.GetComponent<Button>();
                        if (switchButton != null)
                        {
                            break;
                        }
                    }
                }
            }
            
            if (switchButton != null)
            {
                // 清除所有监听器（包括 Inspector 中设置的）
                switchButton.onClick.RemoveAllListeners();
                
                // 添加代码中的监听器
                switchButton.onClick.AddListener(ToggleView);
            }
            else
            {
                Debug.LogError("ViewSwitcher: 切换按钮未找到！请在 Inspector 中设置 Switch Button 字段。");
            }
            
            // 设置按钮文本 - 如果未设置，尝试自动查找
            if (buttonText == null && switchButton != null)
            {
                buttonText = switchButton.GetComponentInChildren<Text>();
            }

            isInitialized = true;
        }


        /// <summary>
        /// 切换视角（公共方法，供按钮调用）
        /// </summary>
        public void ToggleView()
        {
            // 确保已初始化
            if (!isInitialized)
            {
                Initialize();
            }
            
            // 检查必要的对象是否存在
            if (interiorView == null)
            {
                Debug.LogError("ViewSwitcher: ToggleView 失败 - Interior View 未设置！");
                return;
            }
            
            if (frontWindowView == null)
            {
                Debug.LogError("ViewSwitcher: ToggleView 失败 - Front Window View 未设置！");
                return;
            }
            
            if (currentView == ViewType.Interior)
            {
                SwitchView(ViewType.FrontWindow);
            }
            else
            {
                SwitchView(ViewType.Interior);
            }
        }
        
        /// <summary>
        /// 测试方法：手动触发切换（用于调试）
        /// </summary>
        [ContextMenu("测试切换视角")]
        public void TestToggleView()
        {
            ToggleView();
        }

        /// <summary>
        /// 切换到指定视角
        /// </summary>
        public void SwitchView(ViewType viewType)
        {
            currentView = viewType;

            switch (viewType)
            {
                case ViewType.Interior:
                    // 显示车内视角，隐藏车前窗视角
                    if (interiorView != null)
                    {
                        interiorView.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("ViewSwitcher: 车内场景未设置！请在 Inspector 中设置 Interior View 字段。");
                    }
                    
                    if (frontWindowView != null)
                    {
                        frontWindowView.SetActive(false);
                    }

                    // 隐藏出发按钮
                    if (departButton != null)
                    {
                        departButton.Hide();
                    }
                    break;

                case ViewType.FrontWindow:
                    // 显示车前窗视角，隐藏车内视角
                    if (interiorView != null)
                    {
                        interiorView.SetActive(false);
                    }
                    
                    if (frontWindowView != null)
                    {
                        frontWindowView.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("ViewSwitcher: 车前窗场景未设置！请在 Inspector 中设置 Front Window View 字段。");
                    }

                    // 显示出发按钮
                    if (departButton != null)
                    {
                        departButton.Show();
                    }
                    break;
            }

            // 更新按钮文本
            UpdateButtonText();
        }


        /// <summary>
        /// 更新按钮文本
        /// </summary>
        private void UpdateButtonText()
        {
            if (buttonText != null)
            {
                buttonText.text = currentView == ViewType.Interior ? "车前窗" : "车内";
            }
        }

        /// <summary>
        /// 获取当前视角
        /// </summary>
        public ViewType GetCurrentView()
        {
            return currentView;
        }
    }
}
