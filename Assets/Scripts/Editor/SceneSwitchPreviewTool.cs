using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace XEscape.Editor
{
    /// <summary>
    /// 场景切换预览工具
    /// 用于在Unity编辑器中配置和预览场景切换
    /// </summary>
    public class SceneSwitchPreviewTool : EditorWindow
    {
        private string pickupScenePath = "Assets/Scenes/PickupScene.unity";
        private string carScenePath = "Assets/Scenes/CarScene.unity";
        private string escapeScenePath = "Assets/Scenes/EscapeScene.unity";

        [MenuItem("X-Escape/场景切换预览工具")]
        public static void ShowWindow()
        {
            GetWindow<SceneSwitchPreviewTool>("场景切换预览");
        }

        [MenuItem("X-Escape/快速切换/打开 PickupScene")]
        public static void OpenPickupScene()
        {
            OpenScene("Assets/Scenes/PickupScene.unity");
        }

        [MenuItem("X-Escape/快速切换/打开 CarScene")]
        public static void OpenCarScene()
        {
            OpenScene("Assets/Scenes/CarScene.unity");
        }

        [MenuItem("X-Escape/快速切换/打开 EscapeScene")]
        public static void OpenEscapeScene()
        {
            OpenScene("Assets/Scenes/EscapeScene.unity");
        }

        [MenuItem("X-Escape/配置/自动配置Build Settings")]
        public static void QuickConfigureBuildSettings()
        {
            var tool = GetWindow<SceneSwitchPreviewTool>("场景切换预览");
            tool.ConfigureBuildSettings();
        }

        /// <summary>
        /// 打开指定场景（不进入播放模式）
        /// </summary>
        private static void OpenScene(string scenePath)
        {
            if (!System.IO.File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("错误", $"场景文件不存在: {scenePath}", "确定");
                return;
            }

            // 保存当前场景（如果有未保存的更改）
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                if (!EditorUtility.DisplayDialog("保存场景", "当前场景有未保存的更改，是否保存？", "保存", "不保存"))
                {
                    return;
                }
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            // 打开场景
            EditorSceneManager.OpenScene(scenePath);
            Debug.Log($"✅ 已打开场景: {System.IO.Path.GetFileNameWithoutExtension(scenePath)}");
        }

        private void OnGUI()
        {
            GUILayout.Label("场景切换预览工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "此工具用于配置Build Settings并预览场景切换。\n" +
                "确保场景已添加到Build Settings后，点击Play按钮运行游戏即可测试场景切换。",
                MessageType.Info);

            EditorGUILayout.Space();

            // 显示当前Build Settings中的场景
            GUILayout.Label("当前Build Settings中的场景:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (EditorBuildSettings.scenes.Length == 0)
            {
                EditorGUILayout.HelpBox("⚠️ Build Settings中没有场景！请点击下方按钮添加场景。", MessageType.Warning);
            }
            else
            {
                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    var scene = EditorBuildSettings.scenes[i];
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                    EditorGUILayout.LabelField($"{i}. {sceneName}", scene.enabled ? EditorStyles.label : EditorStyles.miniLabel);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 配置Build Settings按钮
            if (GUILayout.Button("📋 自动配置Build Settings", GUILayout.Height(30)))
            {
                ConfigureBuildSettings();
            }

            EditorGUILayout.Space();

            // 快速测试按钮
            GUILayout.Label("快速测试:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (GUILayout.Button("▶️ 打开PickupScene并运行", GUILayout.Height(25)))
            {
                OpenSceneAndPlay(pickupScenePath);
            }

            if (GUILayout.Button("🚗 打开CarScene并运行", GUILayout.Height(25)))
            {
                OpenSceneAndPlay(carScenePath);
            }

            if (GUILayout.Button("🏃 打开EscapeScene并运行", GUILayout.Height(25)))
            {
                OpenSceneAndPlay(escapeScenePath);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 场景路径配置
            EditorGUILayout.LabelField("场景路径配置:", EditorStyles.boldLabel);
            pickupScenePath = EditorGUILayout.TextField("PickupScene路径:", pickupScenePath);
            carScenePath = EditorGUILayout.TextField("CarScene路径:", carScenePath);
            escapeScenePath = EditorGUILayout.TextField("EscapeScene路径:", escapeScenePath);
        }

        /// <summary>
        /// 自动配置Build Settings，添加所有需要的场景
        /// </summary>
        private void ConfigureBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

            // 添加PickupScene
            if (System.IO.File.Exists(pickupScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(pickupScenePath, true));
                Debug.Log($"✅ 已添加场景到Build Settings: PickupScene");
            }
            else
            {
                Debug.LogWarning($"⚠️ 场景文件不存在: {pickupScenePath}");
            }

            // 添加CarScene
            if (System.IO.File.Exists(carScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(carScenePath, true));
                Debug.Log($"✅ 已添加场景到Build Settings: CarScene");
            }
            else
            {
                Debug.LogWarning($"⚠️ 场景文件不存在: {carScenePath}");
            }

            // 添加EscapeScene
            if (System.IO.File.Exists(escapeScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(escapeScenePath, true));
                Debug.Log($"✅ 已添加场景到Build Settings: EscapeScene");
            }
            else
            {
                Debug.LogWarning($"⚠️ 场景文件不存在: {escapeScenePath}");
            }

            // 应用设置
            EditorBuildSettings.scenes = scenes.ToArray();
            
            EditorUtility.DisplayDialog(
                "配置完成",
                $"已成功添加 {scenes.Count} 个场景到Build Settings！\n\n" +
                "现在你可以：\n" +
                "1. 点击Play按钮运行游戏\n" +
                "2. 在游戏中触发场景切换来测试\n\n" +
                "注意：确保PickupScene是第一个场景（作为启动场景）。",
                "确定");

            Debug.Log("🎉 Build Settings配置完成！");
        }

        /// <summary>
        /// 打开场景并自动进入播放模式
        /// </summary>
        private void OpenSceneAndPlay(string scenePath)
        {
            if (!System.IO.File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("错误", $"场景文件不存在: {scenePath}", "确定");
                return;
            }

            // 保存当前场景（如果有未保存的更改）
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            // 打开场景
            EditorSceneManager.OpenScene(scenePath);

            // 确保场景在Build Settings中
            bool sceneInBuild = false;
            foreach (var buildScene in EditorBuildSettings.scenes)
            {
                if (buildScene.path == scenePath)
                {
                    sceneInBuild = true;
                    break;
                }
            }

            if (!sceneInBuild)
            {
                bool addToBuild = EditorUtility.DisplayDialog(
                    "场景未添加到Build Settings",
                    $"场景 {System.IO.Path.GetFileNameWithoutExtension(scenePath)} 尚未添加到Build Settings。\n\n" +
                    "是否现在添加？",
                    "添加", "取消");

                if (addToBuild)
                {
                    var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }

            // 进入播放模式
            EditorApplication.isPlaying = true;
            
            Debug.Log($"▶️ 已打开场景并进入播放模式: {System.IO.Path.GetFileNameWithoutExtension(scenePath)}");
        }
    }
}
