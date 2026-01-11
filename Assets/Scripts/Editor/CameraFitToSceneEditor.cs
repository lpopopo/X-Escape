using UnityEngine;
using UnityEditor;
using XEscape.Utilities;

namespace XEscape.Editor
{
    /// <summary>
    /// 相机适配场景编辑器工具
    /// </summary>
    public class CameraFitToSceneEditor
    {
        [MenuItem("GameObject/X-Escape/为摄像机添加场景适配", false, 20)]
        public static void AddCameraFitToScene()
        {
            // 查找主摄像机
            Camera mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                // 如果没有主摄像机，查找场景中的第一个摄像机
                Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
                if (cameras.Length > 0)
                {
                    mainCamera = cameras[0];
                }
            }
            
            if (mainCamera == null)
            {
                EditorUtility.DisplayDialog(
                    "未找到摄像机",
                    "场景中没有找到摄像机。请先创建一个摄像机。",
                    "确定"
                );
                return;
            }
            
            // 检查是否已经有CameraFitToScene组件
            CameraFitToScene existingComponent = mainCamera.GetComponent<CameraFitToScene>();
            if (existingComponent != null)
            {
                bool replace = EditorUtility.DisplayDialog(
                    "组件已存在",
                    $"摄像机 '{mainCamera.name}' 已经包含 CameraFitToScene 组件。\n\n是否要重新配置？",
                    "是",
                    "否"
                );
                
                if (!replace)
                {
                    // 选中现有的摄像机
                    Selection.activeGameObject = mainCamera.gameObject;
                    EditorUtility.FocusProjectWindow();
                    return;
                }
            }
            
            // 添加或获取组件
            CameraFitToScene fitComponent = mainCamera.GetComponent<CameraFitToScene>();
            if (fitComponent == null)
            {
                fitComponent = mainCamera.gameObject.AddComponent<CameraFitToScene>();
            }
            
            // 配置默认设置
            SerializedObject serializedObject = new SerializedObject(fitComponent);
            serializedObject.FindProperty("autoFitOnStart").boolValue = true;
            serializedObject.FindProperty("autoFitOnAwake").boolValue = false;
            serializedObject.FindProperty("padding").floatValue = 1f;
            serializedObject.FindProperty("fitToAllRenderers").boolValue = true;
            serializedObject.FindProperty("fitToAllColliders").boolValue = false;
            serializedObject.ApplyModifiedProperties();
            
            // 立即执行一次适配（在编辑器中预览）
            if (Application.isPlaying)
            {
                fitComponent.FitToScene();
            }
            
            // 选中摄像机
            Selection.activeGameObject = mainCamera.gameObject;
            EditorUtility.FocusProjectWindow();
            
            // 标记场景为已修改
            EditorUtility.SetDirty(mainCamera.gameObject);
            
            Debug.Log($"已为摄像机 '{mainCamera.name}' 添加 CameraFitToScene 组件！\n" +
                     "运行游戏时会自动适配场景。你也可以在Inspector中右键组件选择'适配到场景'来手动适配。");
        }
        
        [MenuItem("GameObject/X-Escape/为摄像机添加场景适配", true)]
        public static bool ValidateAddCameraFitToScene()
        {
            // 只在场景视图中可用
            return !EditorApplication.isPlaying || Application.isPlaying;
        }
        
        [MenuItem("Tools/X-Escape/适配所有场景的摄像机", false, 100)]
        public static void FitAllCamerasInScene()
        {
            Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            
            if (cameras.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "未找到摄像机",
                    "场景中没有找到任何摄像机。",
                    "确定"
                );
                return;
            }
            
            int addedCount = 0;
            foreach (Camera cam in cameras)
            {
                if (cam.orthographic) // 只为2D正交摄像机添加
                {
                    CameraFitToScene fitComponent = cam.GetComponent<CameraFitToScene>();
                    if (fitComponent == null)
                    {
                        fitComponent = cam.gameObject.AddComponent<CameraFitToScene>();
                        addedCount++;
                        
                        // 配置默认设置
                        SerializedObject serializedObject = new SerializedObject(fitComponent);
                        serializedObject.FindProperty("autoFitOnStart").boolValue = true;
                        serializedObject.FindProperty("padding").floatValue = 1f;
                        serializedObject.FindProperty("fitToAllRenderers").boolValue = true;
                        serializedObject.ApplyModifiedProperties();
                        
                        EditorUtility.SetDirty(cam.gameObject);
                    }
                }
            }
            
            if (addedCount > 0)
            {
                Debug.Log($"已为 {addedCount} 个摄像机添加了 CameraFitToScene 组件！");
            }
            else
            {
                Debug.Log("所有正交摄像机都已经包含 CameraFitToScene 组件。");
            }
        }
    }
}

