using UnityEngine;

namespace XEscape.EscapeScene
{
    /// <summary>
    /// 城镇管理器，处理物资搜寻
    /// </summary>
    public class TownManager : MonoBehaviour
    {
        public static TownManager Instance { get; private set; }

        [Header("物资设置")]
        [SerializeField] private float searchTime = 3f; // 搜寻时间（秒）
        [SerializeField] private float minResourceGain = 10f;
        [SerializeField] private float maxResourceGain = 30f;

        [Header("UI")]
        [SerializeField] private GameObject townMenuUI;

        private bool isSearching = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 打开城镇菜单
        /// </summary>
        public void OpenTownMenu()
        {
            if (townMenuUI != null)
            {
                townMenuUI.SetActive(true);
            }
        }

        /// <summary>
        /// 关闭城镇菜单
        /// </summary>
        public void CloseTownMenu()
        {
            if (townMenuUI != null)
            {
                townMenuUI.SetActive(false);
            }
        }

        /// <summary>
        /// 开始搜寻物资
        /// </summary>
        public void StartSearching()
        {
            if (isSearching)
                return;

            isSearching = true;
            StartCoroutine(SearchResources());
        }

        /// <summary>
        /// 搜寻资源协程
        /// </summary>
        private System.Collections.IEnumerator SearchResources()
        {
            yield return new WaitForSeconds(searchTime);

            // 随机获得资源
            float staminaGain = Random.Range(minResourceGain, maxResourceGain);
            float fuelGain = Random.Range(minResourceGain, maxResourceGain);

            // 恢复资源
            if (GameManager.Instance?.resourceManager != null)
            {
                GameManager.Instance.resourceManager.RestoreStamina(staminaGain);
                GameManager.Instance.resourceManager.RestoreFuel(fuelGain);
            }

            isSearching = false;
            Debug.Log($"搜寻完成！获得体力: {staminaGain:F1}, 油量: {fuelGain:F1}");
        }

        /// <summary>
        /// 是否正在搜寻
        /// </summary>
        public bool IsSearching()
        {
            return isSearching;
        }
    }
}

