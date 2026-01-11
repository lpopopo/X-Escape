using UnityEngine;

namespace XEscape.CarScene
{
    /// <summary>
    /// 后视镜控制器，点击后视镜可以切换查看车内所有人
    /// </summary>
    public class MirrorController : MonoBehaviour
    {
        [Header("后视镜设置")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera mirrorCamera;
        
        [Header("车内人物")]
        [SerializeField] private Transform[] carOccupants; // 车内所有人的位置
        
        [Header("相机设置")]
        [SerializeField] private float cameraTransitionSpeed = 2f;
        
        private int currentViewIndex = 0;
        private bool isViewingMirror = false;
        private Vector3 originalCameraPosition;
        private Quaternion originalCameraRotation;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (mainCamera != null)
            {
                // 确保相机设置为2D正交模式
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5f; // 默认2D相机大小
                originalCameraPosition = mainCamera.transform.position;
                originalCameraRotation = mainCamera.transform.rotation;
            }
            
            if (mirrorCamera != null)
            {
                // 确保后视镜相机也设置为2D正交模式
                mirrorCamera.orthographic = true;
                mirrorCamera.orthographicSize = 5f;
            }
        }

        /// <summary>
        /// 点击后视镜时调用
        /// </summary>
        public void OnMirrorClicked()
        {
            if (isViewingMirror)
            {
                // 返回主视角
                ReturnToMainView();
            }
            else
            {
                // 切换到后视镜视角
                SwitchToMirrorView();
            }
        }

        /// <summary>
        /// 切换到后视镜视角
        /// </summary>
        private void SwitchToMirrorView()
        {
            isViewingMirror = true;
            if (mirrorCamera != null)
            {
                mirrorCamera.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 返回主视角
        /// </summary>
        private void ReturnToMainView()
        {
            isViewingMirror = false;
            if (mirrorCamera != null)
            {
                mirrorCamera.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 切换到下一个车内人物视角
        /// </summary>
        public void SwitchToNextOccupant()
        {
            if (carOccupants == null || carOccupants.Length == 0)
                return;

            currentViewIndex = (currentViewIndex + 1) % carOccupants.Length;
            FocusOnOccupant(currentViewIndex);
        }

        /// <summary>
        /// 切换到上一个车内人物视角
        /// </summary>
        public void SwitchToPreviousOccupant()
        {
            if (carOccupants == null || carOccupants.Length == 0)
                return;

            currentViewIndex = (currentViewIndex - 1 + carOccupants.Length) % carOccupants.Length;
            FocusOnOccupant(currentViewIndex);
        }

        /// <summary>
        /// 聚焦到指定的人物
        /// </summary>
        private void FocusOnOccupant(int index)
        {
            if (index < 0 || index >= carOccupants.Length || carOccupants[index] == null)
                return;

            Transform target = carOccupants[index];
            if (mainCamera != null && target != null)
            {
                // 平滑移动到目标位置
                StartCoroutine(MoveCameraToTarget(target.position));
            }
        }

        /// <summary>
        /// 平滑移动相机到目标位置（2D模式，只移动X和Y）
        /// </summary>
        private System.Collections.IEnumerator MoveCameraToTarget(Vector3 targetPosition)
        {
            Vector3 startPosition = mainCamera.transform.position;
            // 2D模式下，保持Z轴不变，只移动X和Y
            Vector3 targetPos2D = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * cameraTransitionSpeed;
                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPos2D, elapsedTime);
                yield return null;
            }

            mainCamera.transform.position = targetPos2D;
        }
    }
}

