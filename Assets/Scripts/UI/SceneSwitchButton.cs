using UnityEngine;
using UnityEngine.UI;
using XEscape.Managers;

namespace XEscape.UI
{
    /// <summary>
    /// 场景切换按钮控制器
    /// </summary>
    public class SceneSwitchButton : MonoBehaviour
    {
        [Header("按钮设置")]
        [SerializeField] private Button button;

        [Header("切换目标")]
        [SerializeField] private SceneType targetScene = SceneType.EscapeScene;

        private void Start()
        {
            // 如果没有指定按钮，尝试从当前对象获取
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            // 注册按钮点击事件
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
            else
            {
                Debug.LogError("SceneSwitchButton: 未找到Button组件！");
            }
        }

        private void OnButtonClick()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager实例不存在！");
                return;
            }

            if (GameManager.Instance.sceneTransitionManager == null)
            {
                Debug.LogError("SceneTransitionManager不存在！");
                return;
            }

            // 根据目标场景类型进行切换
            switch (targetScene)
            {
                case SceneType.CarScene:
                    Debug.Log("切换到车内场景");
                    GameManager.Instance.sceneTransitionManager.LoadCarScene();
                    break;
                case SceneType.EscapeScene:
                    Debug.Log("开始逃亡！切换到逃亡场景");
                    GameManager.Instance.sceneTransitionManager.LoadEscapeScene();
                    break;
            }
        }

        private void OnDestroy()
        {
            // 取消事件订阅
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }
    }

    /// <summary>
    /// 场景类型枚举
    /// </summary>
    public enum SceneType
    {
        CarScene,       // 车内场景
        EscapeScene     // 逃亡场景
    }
}
