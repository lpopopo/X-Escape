using UnityEngine;

/// <summary>
/// 简单图片显示脚本 - 在场景中显示Resources中的图片
/// 将此脚本挂载到任意GameObject上即可
/// </summary>
public class SimpleImageDisplay : MonoBehaviour
{
    [Header("图片设置")]
    [Tooltip("Resources中的图片名称（不要写.png扩展名）")]
    [SerializeField] private string imageName = "Snipaste_2026-01-11_18-00-03";

    [Tooltip("图片显示位置")]
    [SerializeField] private Vector3 position = new Vector3(0, 0, 0);

    [Tooltip("图片缩放大小")]
    [SerializeField] private float scale = 1f;

    [Tooltip("图片层级（越大越靠前）")]
    [SerializeField] private int sortingOrder = 0;

    private GameObject imageObject;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        LoadAndDisplayImage();
    }

    /// <summary>
    /// 加载并显示图片
    /// </summary>
    void LoadAndDisplayImage()
    {
        // 加载Resources中的图片
        Sprite sprite = Resources.Load<Sprite>(imageName);

        if (sprite == null)
        {
            Debug.LogError($"❌ 无法加载图片: {imageName}");
            Debug.LogError("请检查：");
            Debug.LogError("1. 图片是否在 Assets/Resources/ 文件夹中");
            Debug.LogError("2. 图片名称是否正确（不要包含.png扩展名）");
            Debug.LogError("3. 图片是否设置为 Sprite (2D and UI) 类型");
            return;
        }

        Debug.Log($"✓ 成功加载图片: {sprite.name}");

        // 创建GameObject来显示图片
        imageObject = new GameObject($"Image_{imageName}");
        imageObject.transform.position = position;
        imageObject.transform.localScale = new Vector3(scale, scale, 1f);

        // 添加SpriteRenderer组件
        spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;

        Debug.Log($"✓ 图片已显示在位置: {position}");
        Debug.Log($"✓ 图片大小: {sprite.rect.width}x{sprite.rect.height} 像素");
    }

    /// <summary>
    /// 在Inspector中修改设置时，实时更新
    /// </summary>
    void OnValidate()
    {
        if (Application.isPlaying && imageObject != null)
        {
            // 更新位置
            imageObject.transform.position = position;
            // 更新缩放
            imageObject.transform.localScale = new Vector3(scale, scale, 1f);
            // 更新层级
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = sortingOrder;
            }
        }
    }

    /// <summary>
    /// 显示/隐藏图片
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (imageObject != null)
        {
            imageObject.SetActive(visible);
        }
    }

    /// <summary>
    /// 改变图片位置
    /// </summary>
    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        if (imageObject != null)
        {
            imageObject.transform.position = position;
        }
    }

    /// <summary>
    /// 改变图片大小
    /// </summary>
    public void SetScale(float newScale)
    {
        scale = newScale;
        if (imageObject != null)
        {
            imageObject.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
