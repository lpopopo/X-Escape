using UnityEngine;

public class backGround : MonoBehaviour
{
    [Header("渲染设置")]
    [SerializeField] private int sortingOrder = -10; // 背景应该在最后面
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // 可挂在背景物体上
void Start()
{
    // 获取相机视口的世界空间尺寸
    float cameraHeight = Camera.main.orthographicSize * 2f;
    float cameraWidth = cameraHeight * Camera.main.aspect;
    
    // 获取精灵渲染器和原始精灵大小
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer != null && spriteRenderer.sprite != null)
    {
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;
        
        // 计算需要的缩放比例，确保覆盖整个屏幕
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;
        
        // 使用较大的缩放值确保完全覆盖（避免出现黑边）
        // 添加更大的缓冲值（15%）确保边缘也被完全覆盖
        float scale = Mathf.Max(scaleX, scaleY) * 1.15f;
        
        transform.localScale = new Vector3(scale, scale, 1);
        
        // 设置渲染顺序
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.sortingLayerID = 0; // Default layer
        
        // 确保背景位置在相机前方（保持Z=0，使用本地坐标）
        if (transform.parent != null)
        {
            // 如果是相机的子物体，使用本地坐标
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            // 如果不是子物体，使用世界坐标，但保持Z=0
            transform.position = new Vector3(Camera.main.transform.position.x, 
                                             Camera.main.transform.position.y, 
                                             0);
        }
    }
    else
    {
        // 如果没有精灵渲染器，使用原来的方法，并添加缓冲
        transform.localScale = new Vector3(cameraWidth * 1.15f, cameraHeight * 1.15f, 1);
        
        // 设置渲染顺序（如果有SpriteRenderer）
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;
            sr.sortingLayerID = 0; // Default layer
        }
        
        // 确保背景位置在相机前方（保持Z=0）
        if (transform.parent != null)
        {
            // 如果是相机的子物体，使用本地坐标
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            // 如果不是子物体，使用世界坐标，但保持Z=0
            transform.position = new Vector3(Camera.main.transform.position.x, 
                                             Camera.main.transform.position.y, 
                                             0);
        }
    }
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
