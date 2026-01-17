using UnityEngine;

namespace XEscape.CarScene
{
    /// <summary>
    /// 饱腹度状态枚举
    /// </summary>
    public enum SatietyStatus
    {
        Full,        // 饱腹
        Normal,      // 正常
        Hungry,      // 饥饿
        Starving     // 饥荒
    }

    /// <summary>
    /// 伪装度状态枚举
    /// </summary>
    public enum DisguiseStatus
    {
        CompletelyDifferent,  // 判若两人
        HardToDetect,         // 难以察觉
        Suspicious,           // 可疑
        Identical             // 一模一样
    }

    /// <summary>
    /// 车内人物信息
    /// </summary>
    public class CarOccupant : MonoBehaviour
    {
        [Header("人物信息")]
        [SerializeField] private string occupantName;
        [SerializeField] private Sprite occupantPortrait;
        
        [Header("人物状态")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool isDead = false;
        [SerializeField] private int daysWithZeroSatiety = 0; // 饱腹度为0持续的天数
        
        [Header("饱腹度设置")]
        [Tooltip("饱腹度值 (0-100)，5档=100, 4档=80, 3档=60, 2档=40, 1档=20, 0档=0")]
        [SerializeField] [Range(0f, 100f)] private float satiety = 100f; // 第一天满档
        [SerializeField] private float satietyFullThreshold = 75f;      // 饱腹阈值
        [SerializeField] private float satietyNormalThreshold = 50f;   // 正常阈值
        [SerializeField] private float satietyHungryThreshold = 25f;    // 饥饿阈值
        
        [Header("伪装度设置")]
        [Tooltip("伪装度值 (0-100)，值越高伪装越好，5档=100, 4档=80, 3档=60, 2档=40, 1档=20, 0档=0")]
        [SerializeField] [Range(0f, 100f)] private float disguise = 100f; // 第一天满档
        [SerializeField] private float disguiseIdenticalThreshold = 75f;      // 一模一样阈值
        [SerializeField] private float disguiseHardToDetectThreshold = 50f;  // 难以察觉阈值
        [SerializeField] private float disguiseSuspiciousThreshold = 25f;      // 可疑阈值
        
        [Header("渲染设置")]
        [SerializeField] private int sortingOrder = 0; // 渲染顺序，确保大于背景的-10

        /// <summary>
        /// 获取人物名称
        /// </summary>
        public string GetName()
        {
            return occupantName;
        }

        /// <summary>
        /// 获取人物头像
        /// </summary>
        public Sprite GetPortrait()
        {
            return occupantPortrait;
        }

        /// <summary>
        /// 获取健康值
        /// </summary>
        public float GetHealth()
        {
            return health;
        }

        /// <summary>
        /// 获取健康百分比
        /// </summary>
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? health / maxHealth : 0;
        }

        /// <summary>
        /// 获取饱腹度值
        /// </summary>
        public float GetSatiety()
        {
            return satiety;
        }

        /// <summary>
        /// 获取饱腹度状态
        /// </summary>
        public SatietyStatus GetSatietyStatus()
        {
            if (satiety >= satietyFullThreshold)
                return SatietyStatus.Full;
            else if (satiety >= satietyNormalThreshold)
                return SatietyStatus.Normal;
            else if (satiety >= satietyHungryThreshold)
                return SatietyStatus.Hungry;
            else
                return SatietyStatus.Starving;
        }

        /// <summary>
        /// 获取饱腹度状态文本
        /// </summary>
        public string GetSatietyStatusText()
        {
            switch (GetSatietyStatus())
            {
                case SatietyStatus.Full:
                    return "饱腹";
                case SatietyStatus.Normal:
                    return "正常";
                case SatietyStatus.Hungry:
                    return "饥饿";
                case SatietyStatus.Starving:
                    return "饥荒";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 获取伪装度值
        /// </summary>
        public float GetDisguise()
        {
            return disguise;
        }

        /// <summary>
        /// 获取伪装度状态（值越高伪装越好）
        /// </summary>
        public DisguiseStatus GetDisguiseStatus()
        {
            if (disguise >= disguiseIdenticalThreshold)
                return DisguiseStatus.CompletelyDifferent; // 判若两人 = 最高（伪装最好）
            else if (disguise >= disguiseHardToDetectThreshold)
                return DisguiseStatus.HardToDetect; // 难以察觉 = 较高
            else if (disguise >= disguiseSuspiciousThreshold)
                return DisguiseStatus.Suspicious; // 可疑 = 较低
            else
                return DisguiseStatus.Identical; // 一模一样 = 最低（伪装最差）
        }

        /// <summary>
        /// 获取伪装度状态文本
        /// </summary>
        public string GetDisguiseStatusText()
        {
            switch (GetDisguiseStatus())
            {
                case DisguiseStatus.Identical:
                    return "一模一样";
                case DisguiseStatus.HardToDetect:
                    return "难以察觉";
                case DisguiseStatus.Suspicious:
                    return "可疑";
                case DisguiseStatus.CompletelyDifferent:
                    return "判若两人";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 设置饱腹度
        /// </summary>
        public void SetSatiety(float value)
        {
            satiety = Mathf.Clamp(value, 0f, 100f);
        }

        /// <summary>
        /// 设置伪装度
        /// </summary>
        public void SetDisguise(float value)
        {
            disguise = Mathf.Clamp(value, 0f, 100f);
        }

        /// <summary>
        /// 降低一档饱腹度（减20）
        /// </summary>
        public void DecreaseSatietyLevel()
        {
            if (isDead) return;
            
            satiety = Mathf.Max(0f, satiety - 20f);
            
            // 检查饱腹度是否为0
            if (satiety <= 0f)
            {
                daysWithZeroSatiety++;
                if (daysWithZeroSatiety >= 2)
                {
                    Die();
                }
            }
            else
            {
                daysWithZeroSatiety = 0; // 重置计数
            }
        }

        /// <summary>
        /// 降低一档伪装度（减20）
        /// </summary>
        public void DecreaseDisguiseLevel()
        {
            if (isDead) return;
            disguise = Mathf.Max(0f, disguise - 20f);
        }

        /// <summary>
        /// 角色死亡
        /// </summary>
        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            gameObject.SetActive(false); // 隐藏角色
        }

        /// <summary>
        /// 检查角色是否死亡
        /// </summary>
        public bool IsDead() => isDead;

        /// <summary>
        /// 获取饱腹度档位（0-5）
        /// </summary>
        public int GetSatietyLevel()
        {
            return Mathf.FloorToInt(satiety / 20f);
        }

        /// <summary>
        /// 获取伪装度档位（0-5）
        /// </summary>
        public int GetDisguiseLevel()
        {
            return Mathf.FloorToInt(disguise / 20f);
        }

        private void Start()
        {
            // 确保人物显示在背景前面
            SetupRenderingOrder();
        }

        /// <summary>
        /// 设置渲染顺序，确保人物显示在背景前面
        /// </summary>
        private void SetupRenderingOrder()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // 确保使用Default Sorting Layer
                spriteRenderer.sortingLayerID = 0;
                
                // 确保人物的排序顺序大于背景的-10
                if (spriteRenderer.sortingOrder <= -10)
                {
                    spriteRenderer.sortingOrder = sortingOrder;
                }
            }
            
            // 确保人物的Z位置是0（2D游戏中所有物体的Z都应该是0）
            Vector3 pos = transform.position;
            if (Mathf.Abs(pos.z) > 0.01f)
            {
                transform.position = new Vector3(pos.x, pos.y, 0);
            }
            
            // 如果是子物体，确保本地Z也是0
            if (transform.parent != null)
            {
                Vector3 localPos = transform.localPosition;
                if (Mathf.Abs(localPos.z) > 0.01f)
                {
                    transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
                }
            }
        }
    }
}

