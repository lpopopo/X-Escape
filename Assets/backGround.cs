using UnityEngine;

public class backGround : MonoBehaviour
{
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
        
        // 确保背景位置在相机前方
        transform.position = new Vector3(Camera.main.transform.position.x, 
                                         Camera.main.transform.position.y, 
                                         transform.position.z);
    }
    else
    {
        // 如果没有精灵渲染器，使用原来的方法，并添加缓冲
        transform.localScale = new Vector3(cameraWidth * 1.15f, cameraHeight * 1.15f, 1);
        transform.position = new Vector3(Camera.main.transform.position.x, 
                                         Camera.main.transform.position.y, 
                                         transform.position.z);
    }
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
