using UnityEngine;

namespace XEscape.CarScene
{
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
    }
}

