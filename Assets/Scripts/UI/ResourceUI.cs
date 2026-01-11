using UnityEngine;
using UnityEngine.UI;
using XEscape.Managers;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif

namespace XEscape.UI
{
    /// <summary>
    /// 资源UI显示，显示体力和油量
    /// </summary>
    public class ResourceUI : MonoBehaviour
    {
        [Header("体力UI")]
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Text staminaText;
#if UNITY_TEXTMESHPRO
        [SerializeField] private TextMeshProUGUI staminaTextTMP; // 支持TextMeshPro（可选）
#endif

        [Header("油量UI")]
        [SerializeField] private Slider fuelSlider;
        [SerializeField] private Text fuelText;
#if UNITY_TEXTMESHPRO
        [SerializeField] private TextMeshProUGUI fuelTextTMP; // 支持TextMeshPro（可选）
#endif

        private ResourceManager resourceManager;

        private void Start()
        {
            resourceManager = GameManager.Instance?.resourceManager;
            
            if (resourceManager != null)
            {
                resourceManager.OnStaminaChanged += UpdateStaminaUI;
                resourceManager.OnFuelChanged += UpdateFuelUI;
                
                // 初始化UI
                UpdateStaminaUI(resourceManager.GetStamina(), 100f);
                UpdateFuelUI(resourceManager.GetFuel(), 100f);
            }
        }

        private void OnDestroy()
        {
            if (resourceManager != null)
            {
                resourceManager.OnStaminaChanged -= UpdateStaminaUI;
                resourceManager.OnFuelChanged -= UpdateFuelUI;
            }
        }

        /// <summary>
        /// 更新体力UI
        /// </summary>
        private void UpdateStaminaUI(float current, float max)
        {
            if (staminaSlider != null)
            {
                staminaSlider.value = max > 0 ? current / max : 0;
            }

            string staminaDisplay = $"体力: {current:F1}/{max:F1}";
            if (staminaText != null)
            {
                staminaText.text = staminaDisplay;
            }
#if UNITY_TEXTMESHPRO
            if (staminaTextTMP != null)
            {
                staminaTextTMP.text = staminaDisplay;
            }
#endif
        }

        /// <summary>
        /// 更新油量UI
        /// </summary>
        private void UpdateFuelUI(float current, float max)
        {
            if (fuelSlider != null)
            {
                fuelSlider.value = max > 0 ? current / max : 0;
            }

            string fuelDisplay = $"油量: {current:F1}/{max:F1}";
            if (fuelText != null)
            {
                fuelText.text = fuelDisplay;
            }
#if UNITY_TEXTMESHPRO
            if (fuelTextTMP != null)
            {
                fuelTextTMP.text = fuelDisplay;
            }
#endif
        }
    }
}

