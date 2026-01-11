using UnityEngine;
using UnityEditor;
using XEscape.CarScene;

namespace XEscape.Editor
{
    /// <summary>
    /// 车内画面编辑器工具
    /// </summary>
    public class CarInteriorViewEditor
    {
        [MenuItem("GameObject/X-Escape/创建车内画面", false, 10)]
        public static void CreateCarInteriorView()
        {
            // 检查场景中是否已经存在车内画面
            CarInteriorView existingView = Object.FindFirstObjectByType<CarInteriorView>();
            if (existingView != null)
            {
                bool replace = EditorUtility.DisplayDialog(
                    "车内画面已存在",
                    "场景中已经存在车内画面，是否要删除旧的并创建新的？",
                    "是",
                    "否"
                );
                
                if (replace)
                {
                    Object.DestroyImmediate(existingView.gameObject);
                }
                else
                {
                    // 选中现有的
                    Selection.activeGameObject = existingView.gameObject;
                    return;
                }
            }
            
            // 创建新的GameObject
            GameObject carInterior = new GameObject("CarInterior");
            
            // 添加CarInteriorView组件
            CarInteriorView view = carInterior.AddComponent<CarInteriorView>();
            
            // 设置位置（在相机中心）
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                carInterior.transform.position = new Vector3(
                    mainCam.transform.position.x,
                    mainCam.transform.position.y,
                    0f
                );
            }
            else
            {
                carInterior.transform.position = Vector3.zero;
            }
            
            // 选中新创建的对象
            Selection.activeGameObject = carInterior;
            
            // 标记场景为已修改
            EditorUtility.SetDirty(carInterior);
            
            Debug.Log("车内画面已创建！运行游戏时会自动生成默认图片。");
        }
        
        [MenuItem("GameObject/X-Escape/创建车内画面", true)]
        public static bool ValidateCreateCarInteriorView()
        {
            // 只在场景视图中可用
            return !EditorApplication.isPlaying;
        }
    }
}

