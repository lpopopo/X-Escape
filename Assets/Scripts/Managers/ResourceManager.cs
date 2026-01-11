using UnityEngine;
using System;

namespace XEscape.Managers
{
    /// <summary>
    /// 资源管理器，管理体力、油量、物资等
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("资源数值")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina = 100f;
        
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float currentFuel = 100f;

        [Header("资源消耗速率")]
        [SerializeField] private float staminaConsumptionRate = 1f; // 每秒消耗
        [SerializeField] private float fuelConsumptionRate = 2f;    // 每秒消耗

        // 事件
        public event Action<float, float> OnStaminaChanged;
        public event Action<float, float> OnFuelChanged;
        public event Action OnResourcesDepleted;

        private bool isConsumingResources = false;

        private void Update()
        {
            if (isConsumingResources)
            {
                ConsumeResources();
            }
        }

        /// <summary>
        /// 消耗资源
        /// </summary>
        private void ConsumeResources()
        {
            // 消耗体力
            if (currentStamina > 0)
            {
                currentStamina -= staminaConsumptionRate * Time.deltaTime;
                currentStamina = Mathf.Max(0, currentStamina);
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            }

            // 消耗油量（仅在逃亡时消耗）
            if (currentFuel > 0 && GameManager.Instance?.currentGameState == GameState.Escaping)
            {
                currentFuel -= fuelConsumptionRate * Time.deltaTime;
                currentFuel = Mathf.Max(0, currentFuel);
                OnFuelChanged?.Invoke(currentFuel, maxFuel);
            }

            // 检查资源是否耗尽
            if (currentStamina <= 0 || currentFuel <= 0)
            {
                OnResourcesDepleted?.Invoke();
            }
        }

        /// <summary>
        /// 开始消耗资源
        /// </summary>
        public void StartConsumingResources()
        {
            isConsumingResources = true;
        }

        /// <summary>
        /// 停止消耗资源
        /// </summary>
        public void StopConsumingResources()
        {
            isConsumingResources = false;
        }

        /// <summary>
        /// 使用物资恢复体力
        /// </summary>
        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        /// <summary>
        /// 使用物资恢复油量
        /// </summary>
        public void RestoreFuel(float amount)
        {
            currentFuel = Mathf.Min(maxFuel, currentFuel + amount);
            OnFuelChanged?.Invoke(currentFuel, maxFuel);
        }

        /// <summary>
        /// 减少体力
        /// </summary>
        public void ReduceStamina(float amount)
        {
            currentStamina = Mathf.Max(0, currentStamina - amount);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            
            if (currentStamina <= 0)
            {
                OnResourcesDepleted?.Invoke();
            }
        }

        /// <summary>
        /// 减少油量
        /// </summary>
        public void ReduceFuel(float amount)
        {
            currentFuel = Mathf.Max(0, currentFuel - amount);
            OnFuelChanged?.Invoke(currentFuel, maxFuel);
            
            if (currentFuel <= 0)
            {
                OnResourcesDepleted?.Invoke();
            }
        }

        /// <summary>
        /// 获取当前体力
        /// </summary>
        public float GetStamina()
        {
            return currentStamina;
        }

        /// <summary>
        /// 获取当前油量
        /// </summary>
        public float GetFuel()
        {
            return currentFuel;
        }

        /// <summary>
        /// 获取体力百分比
        /// </summary>
        public float GetStaminaPercentage()
        {
            return maxStamina > 0 ? currentStamina / maxStamina : 0;
        }

        /// <summary>
        /// 获取油量百分比
        /// </summary>
        public float GetFuelPercentage()
        {
            return maxFuel > 0 ? currentFuel / maxFuel : 0;
        }
    }
}

