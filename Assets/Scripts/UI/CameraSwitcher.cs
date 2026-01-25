using UnityEngine;
using UnityEngine.UI;

namespace XEscape.UI
{
    /// <summary>
    /// 摄像头切换控制器 - 用于在车内视角和逃亡视角之间切换
    /// </summary>
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("摄像头设置")]
        [SerializeField] private Camera carCamera;          // 车内视角摄像头（Main Camera）
        [SerializeField] private Camera escapeCamera;       // 逃亡视角摄像头（Camera）

        [Header("按钮设置（可选）")]
        [SerializeField] private Button switchButton;       // 切换按钮

        [Header("音频监听器设置")]
        [SerializeField] private bool manageSwitchAudioListener = true;  // 是否自动管理AudioListener

        private bool isInCarView = true;  // 当前是否在车内视角

        private void Start()
        {
            // 初始化：车内视角激活，逃亡视角关闭
            SetCameraState(true);

            // 如果有按钮，注册点击事件
            if (switchButton != null)
            {
                switchButton.onClick.AddListener(SwitchCamera);
            }
        }

        /// <summary>
        /// 切换摄像头
        /// </summary>
        public void SwitchCamera()
        {
            isInCarView = !isInCarView;
            SetCameraState(isInCarView);

            Debug.Log($"切换到: {(isInCarView ? "车内视角" : "逃亡视角")}");
        }

        /// <summary>
        /// 切换到车内视角
        /// </summary>
        public void SwitchToCarView()
        {
            isInCarView = true;
            SetCameraState(true);
            Debug.Log("切换到车内视角");
        }

        /// <summary>
        /// 切换到逃亡视角
        /// </summary>
        public void SwitchToEscapeView()
        {
            isInCarView = false;
            SetCameraState(false);
            Debug.Log("切换到逃亡视角");
        }

        /// <summary>
        /// 设置摄像头状态
        /// </summary>
        private void SetCameraState(bool showCarView)
        {
            if (carCamera == null || escapeCamera == null)
            {
                Debug.LogError("CameraSwitcher: 请在Inspector中分配两个摄像头！");
                return;
            }

            if (showCarView)
            {
                // 显示车内视角
                carCamera.enabled = true;
                escapeCamera.enabled = false;

                // 管理AudioListener（同一时间只能有一个激活）
                if (manageSwitchAudioListener)
                {
                    SetAudioListener(carCamera, true);
                    SetAudioListener(escapeCamera, false);
                }
            }
            else
            {
                // 显示逃亡视角
                carCamera.enabled = false;
                escapeCamera.enabled = true;

                // 管理AudioListener
                if (manageSwitchAudioListener)
                {
                    SetAudioListener(carCamera, false);
                    SetAudioListener(escapeCamera, true);
                }
            }
        }

        /// <summary>
        /// 设置AudioListener状态
        /// </summary>
        private void SetAudioListener(Camera cam, bool enabled)
        {
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null)
            {
                listener.enabled = enabled;
            }
        }

        /// <summary>
        /// 获取当前是否在车内视角
        /// </summary>
        public bool IsInCarView()
        {
            return isInCarView;
        }

        private void OnDestroy()
        {
            // 取消按钮事件订阅
            if (switchButton != null)
            {
                switchButton.onClick.RemoveListener(SwitchCamera);
            }
        }

        // 键盘快捷键测试（可选）
        private void Update()
        {
            // 按空格键切换视角（调试用）
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwitchCamera();
            }
        }
    }
}
