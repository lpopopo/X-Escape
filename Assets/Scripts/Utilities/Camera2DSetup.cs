using UnityEngine;

namespace XEscape.Utilities
{
    /// <summary>
    /// 2D相机自动设置脚本，确保相机配置为2D正交模式
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class Camera2DSetup : MonoBehaviour
    {
        [Header("2D相机设置")]
        [SerializeField] private float orthographicSize = 5f;
        [SerializeField] private float cameraZPosition = -10f; // 2D相机通常放在Z=-10位置

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            Setup2DCamera();
        }

        /// <summary>
        /// 设置相机为2D模式
        /// </summary>
        private void Setup2DCamera()
        {
            if (cam == null)
                return;

            // 设置为正交投影（2D模式）
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;

            // 确保相机Z轴位置正确（2D相机通常放在Z=-10）
            Vector3 pos = transform.position;
            if (Mathf.Abs(pos.z - cameraZPosition) > 0.01f)
            {
                transform.position = new Vector3(pos.x, pos.y, cameraZPosition);
            }

            // 清除背景（可选，根据项目需求）
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
        }

        /// <summary>
        /// 在编辑器中也可以调用此方法来设置相机
        /// </summary>
        [ContextMenu("Setup 2D Camera")]
        public void Setup2DCameraManual()
        {
            Setup2DCamera();
        }
    }
}

