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
                originalCameraPosition = mainCamera.transform.position;
                originalCameraRotation = mainCamera.transform.rotation;
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
        /// 平滑移动相机到目标位置
        /// </summary>
        private System.Collections.IEnumerator MoveCameraToTarget(Vector3 targetPosition)
        {
            Vector3 startPosition = mainCamera.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * cameraTransitionSpeed;
                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
                yield return null;
            }

            mainCamera.transform.position = targetPosition;
        }
    }
}

