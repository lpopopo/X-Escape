using UnityEngine;

namespace XEscape.CarScene
{
    /// <summary>
    /// 车内画面渲染器，用于显示车内的背景画面
    /// </summary>
    public class CarInteriorView : MonoBehaviour
    {
        [Header("画面设置")]
        [SerializeField] private Sprite interiorSprite;
        [SerializeField] private string resourcePath = "car_interior"; // Resources文件夹中的图片路径（不含扩展名）
        [SerializeField] private bool loadFromResources = true; // 是否从Resources文件夹加载
        [SerializeField] private Color defaultColor = new Color(0.2f, 0.2f, 0.25f, 1f); // 深灰色，模拟车内环境
        [SerializeField] private Vector2 defaultSize = new Vector2(10f, 6f); // 默认画面大小
        
        [Header("组件引用")]
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            InitializeRenderer();
        }

        private void OnEnable()
        {
            // 在OnEnable中也初始化，确保在编辑器模式下也能看到
            InitializeRenderer();
        }

        private void Start()
        {
            SetupInteriorView();
        }

        /// <summary>
        /// 初始化渲染器
        /// </summary>
        private void InitializeRenderer()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        /// <summary>
        /// 设置车内画面
        /// </summary>
        private void SetupInteriorView()
        {
            // 尝试从Resources文件夹加载图片
            if (interiorSprite == null && loadFromResources && !string.IsNullOrEmpty(resourcePath))
            {
                LoadInteriorFromResources();
            }
            
            // 如果还是没有指定图片，创建默认图片
            if (interiorSprite == null)
            {
                interiorSprite = CreateDefaultInteriorSprite();
            }
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = interiorSprite;
                // 设置排序层级，确保背景在最底层
                spriteRenderer.sortingOrder = -10;
                // 确保SpriteRenderer是启用的
                spriteRenderer.enabled = true;
                
                // 设置GameObject的位置和缩放，确保画面填满相机视野
                // 2D相机orthographicSize为5时，视野高度为10，宽度根据屏幕比例计算
                Camera mainCam = Camera.main;
                if (mainCam != null && mainCam.orthographic)
                {
                    float cameraHeight = mainCam.orthographicSize * 2f;
                    float cameraWidth = cameraHeight * mainCam.aspect;
                    
                    // 设置Sprite的大小以匹配相机视野
                    if (interiorSprite != null)
                    {
                        // 计算合适的缩放
                        float spriteHeight = interiorSprite.bounds.size.y;
                        float spriteWidth = interiorSprite.bounds.size.x;
                        
                        float scaleY = cameraHeight / spriteHeight;
                        float scaleX = cameraWidth / spriteWidth;
                        float scale = Mathf.Max(scaleX, scaleY) * 1.1f; // 稍微大一点确保覆盖
                        
                        transform.localScale = new Vector3(scale, scale, 1f);
                    }
                    
                    // 确保位置在相机中心
                    if (mainCam.transform.position.z < 0)
                    {
                        transform.position = new Vector3(
                            mainCam.transform.position.x,
                            mainCam.transform.position.y,
                            0f // 2D Sprite的Z位置应该是0
                        );
                    }
                }
            }
        }

        /// <summary>
        /// 创建默认的车内画面Sprite
        /// </summary>
        private Sprite CreateDefaultInteriorSprite()
        {
            // 创建默认的Texture2D
            int width = 1024;
            int height = 768;
            Texture2D texture = new Texture2D(width, height);
            
            // 创建渐变效果，模拟车内环境
            // 上半部分较暗（车顶），下半部分稍亮（座椅区域）
            Color topColor = new Color(0.15f, 0.15f, 0.2f, 1f); // 深灰蓝色（车顶）
            Color bottomColor = new Color(0.25f, 0.25f, 0.3f, 1f); // 稍亮的灰色（座椅区域）
            
            Color[] pixels = new Color[width * height];
            // 使用Unity的Random种子确保每次生成一致（可选）
            Random.State oldState = Random.state;
            Random.InitState(42); // 固定种子，确保每次生成相同的默认图片
            
            for (int y = 0; y < height; y++)
            {
                // 从顶部到底部的渐变
                float t = (float)y / height;
                Color baseColor = Color.Lerp(topColor, bottomColor, t);
                
                for (int x = 0; x < width; x++)
                {
                    // 为每个像素添加轻微的随机噪声，模拟纹理
                    float noise = Random.Range(-0.03f, 0.03f);
                    Color pixelColor = new Color(
                        Mathf.Clamp01(baseColor.r + noise),
                        Mathf.Clamp01(baseColor.g + noise),
                        Mathf.Clamp01(baseColor.b + noise),
                        1f
                    );
                    
                    pixels[y * width + x] = pixelColor;
                }
            }
            
            // 恢复随机状态
            Random.state = oldState;
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            // 转换为Sprite
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f), // 中心点
                100f // pixels per unit
            );
            
            sprite.name = "DefaultCarInterior";
            
            return sprite;
        }

        /// <summary>
        /// 设置车内画面图片
        /// </summary>
        public void SetInteriorSprite(Sprite sprite)
        {
            interiorSprite = sprite;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = interiorSprite;
            }
        }

        /// <summary>
        /// 获取当前车内画面图片
        /// </summary>
        public Sprite GetInteriorSprite()
        {
            return interiorSprite;
        }

        /// <summary>
        /// 从Resources文件夹加载车内图片
        /// </summary>
        private void LoadInteriorFromResources()
        {
            // 尝试加载Sprite
            Sprite loadedSprite = Resources.Load<Sprite>(resourcePath);
            if (loadedSprite != null)
            {
                interiorSprite = loadedSprite;
                Debug.Log($"成功从Resources加载车内图片: {resourcePath}");
                return;
            }

            // 如果Sprite加载失败，尝试加载Texture2D并转换为Sprite
            Texture2D loadedTexture = Resources.Load<Texture2D>(resourcePath);
            if (loadedTexture != null)
            {
                interiorSprite = TextureToSprite(loadedTexture);
                Debug.Log($"成功从Resources加载车内纹理并转换为Sprite: {resourcePath}");
                return;
            }

            Debug.LogWarning($"无法从Resources加载车内图片: {resourcePath}，将使用默认图片");
        }

        /// <summary>
        /// 将Texture2D转换为Sprite
        /// </summary>
        private Sprite TextureToSprite(Texture2D texture)
        {
            if (texture == null)
                return null;

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), // 中心点
                100f // pixels per unit
            );
            
            sprite.name = texture.name;
            return sprite;
        }
    }
}

