using UnityEngine;

/// <summary>
/// 测试脚本：演示如何加载Resources中的图片
/// 将此脚本挂载到场景中的任意GameObject上即可
/// </summary>
public class TestImageLoader : MonoBehaviour
{
    void Start()
    {
        // 方法1：加载图片并创建GameObject显示
        LoadAndDisplayImage();

        // 方法2：如果GameObject已有SpriteRenderer，直接设置
        LoadToExistingSpriteRenderer();
    }

    /// <summary>
    /// 方法1：动态创建GameObject并显示图片
    /// </summary>
    void LoadAndDisplayImage()
    {
        // 加载Resources中的图片（不要写扩展名.png）
        Sprite sprite = Resources.Load<Sprite>("Snipaste_2026-01-11_18-00-03");

        if (sprite == null)
        {
            Debug.LogError("❌ 图片加载失败！检查文件名是否正确");
            return;
        }

        Debug.Log("✓ 成功加载图片: " + sprite.name);

        // 创建新的GameObject
        GameObject imageObject = new GameObject("LoadedImage");
        imageObject.transform.position = new Vector3(0, 0, 0);

        // 添加SpriteRenderer组件
        SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        Debug.Log("✓ 图片已显示在场景中心位置 (0, 0, 0)");
    }

    /// <summary>
    /// 方法2：给现有的SpriteRenderer设置图片
    /// </summary>
    void LoadToExistingSpriteRenderer()
    {
        // 获取当前GameObject的SpriteRenderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            Debug.Log("当前GameObject没有SpriteRenderer组件");
            return;
        }

        // 加载并设置图片
        Sprite sprite = Resources.Load<Sprite>("Snipaste_2026-01-11_18-00-03");
        if (sprite != null)
        {
            renderer.sprite = sprite;
            Debug.Log("✓ 图片已设置到当前GameObject的SpriteRenderer");
        }
    }
}
