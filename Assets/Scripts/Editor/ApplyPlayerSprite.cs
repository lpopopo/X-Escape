using UnityEngine;
using UnityEditor;

namespace XEscape.Editor
{
    /// <summary>
    /// 快速应用Player贴图
    /// </summary>
    public class ApplyPlayerSprite : EditorWindow
    {
        [MenuItem("Tools/🚗 替换Player为carPlayer贴图")]
        public static void ApplyCarSprite()
        {
            // 查找Player对象
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "场景中未找到Player对象！\n\n" +
                    "请确保场景中有Player对象。",
                    "确定");
                return;
            }

            // 查找carPlayer贴图
            Sprite carSprite = FindSpriteByName("carPlayer");
            if (carSprite == null)
            {
                EditorUtility.DisplayDialog("错误",
                    "未找到carPlayer贴图！\n\n" +
                    "请确保图片已导入到Unity项目中。",
                    "确定");
                return;
            }

            // 配置为像素艺术
            ConfigureSpriteForPixelArt(carSprite);

            // 应用贴图
            ApplySpriteToPlayer(player, carSprite);

            EditorUtility.DisplayDialog("完成",
                "✅ Player已替换为carPlayer贴图！\n\n" +
                "贴图配置:\n" +
                "- 已设置为像素艺术风格\n" +
                "- Sorting Order: 1\n" +
                "- 碰撞体已调整为横向车辆\n\n" +
                "现在可以测试了！",
                "确定");

            Debug.Log("✅ Player贴图已替换为carPlayer");
        }

        private static Sprite FindSpriteByName(string spriteName)
        {
            // 查找Sprite
            string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            // 查找Texture2D并转换
            guids = AssetDatabase.FindAssets(spriteName + " t:Texture2D");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return null;
        }

        private static void ConfigureSpriteForPixelArt(Sprite sprite)
        {
            if (sprite == null) return;

            string path = AssetDatabase.GetAssetPath(sprite);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                bool needsReimport = false;

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    needsReimport = true;
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    needsReimport = true;
                }

                if (needsReimport)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    Debug.Log("✅ 已配置carPlayer为像素艺术风格");
                }
            }
        }

        private static void ApplySpriteToPlayer(GameObject player, Sprite sprite)
        {
            // 移除旧的3D渲染器
            MeshRenderer meshRenderer = player.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Object.DestroyImmediate(meshRenderer);
                Debug.Log("✅ 已移除MeshRenderer");
            }

            MeshFilter meshFilter = player.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Object.DestroyImmediate(meshFilter);
                Debug.Log("✅ 已移除MeshFilter");
            }

            // 添加或更新SpriteRenderer
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = player.AddComponent<SpriteRenderer>();
                Debug.Log("✅ 已添加SpriteRenderer");
            }

            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = 1; // Player在前面
            Debug.Log("✅ 已设置carPlayer贴图，Sorting Order = 1");

            // 调整Transform（车辆大小和位置）
            player.transform.position = new Vector3(0, -2, 0);
            player.transform.localScale = new Vector3(2, 2, 1);
            Debug.Log("✅ 已调整车辆大小和位置");

            // 调整碰撞体（横向车辆）
            BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = new Vector2(1.2f, 0.6f); // 宽>高
                collider.offset = Vector2.zero;
                Debug.Log("✅ 已调整碰撞体为横向车辆");
            }
        }
    }
}
