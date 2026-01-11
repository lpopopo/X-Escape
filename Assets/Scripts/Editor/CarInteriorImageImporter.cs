using UnityEngine;
using UnityEditor;
using System.IO;

namespace XEscape.Editor
{
    /// <summary>
    /// 车内图片导入器，自动配置图片导入设置
    /// </summary>
    public class CarInteriorImageImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            // 检查是否是Resources文件夹中的车内图片
            string assetPath = assetImporter.assetPath;
            
            if (assetPath.Contains("Resources") && 
                (assetPath.Contains("car_interior") || assetPath.Contains("CarInterior")))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                
                // 设置为Sprite类型
                textureImporter.textureType = TextureImporterType.Sprite;
                
                // 设置为2D模式
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                
                // 设置像素每单位
                textureImporter.spritePixelsPerUnit = 100;
                
                // 设置过滤模式
                textureImporter.filterMode = FilterMode.Bilinear;
                
                // 设置压缩格式
                textureImporter.textureCompression = TextureImporterCompression.Compressed;
                
                // 设置最大尺寸（根据图片大小自动调整）
                int maxSize = 2048;
                if (textureImporter.maxTextureSize > maxSize)
                {
                    textureImporter.maxTextureSize = maxSize;
                }
                
                Debug.Log($"已自动配置车内图片导入设置: {assetPath}");
            }
        }
    }
}

