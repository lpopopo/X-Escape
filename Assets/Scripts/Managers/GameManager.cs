using UnityEngine;
using UnityEngine.SceneManagement;

namespace XEscape.Managers
{
    /// <summary>
    /// 游戏主管理器，负责游戏整体流程和状态管理
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("游戏状态")]
        public GameState currentGameState = GameState.InCar;

        [Header("资源管理")]
        public ResourceManager resourceManager;

        [Header("场景管理")]
        public SceneTransitionManager sceneTransitionManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeManagers()
        {
            if (resourceManager == null)
                resourceManager = GetComponent<ResourceManager>();
            
            if (sceneTransitionManager == null)
                sceneTransitionManager = GetComponent<SceneTransitionManager>();
        }

        /// <summary>
        /// 切换游戏状态
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged(newState);
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.InCar:
                    // 车内场景逻辑
                    break;
                case GameState.Escaping:
                    // 逃亡场景逻辑
                    break;
                case GameState.GameOver:
                    // 游戏结束逻辑
                    break;
                case GameState.Victory:
                    // 胜利逻辑
                    break;
            }
        }

        /// <summary>
        /// 检查游戏是否结束
        /// </summary>
        public void CheckGameOver()
        {
            if (resourceManager != null)
            {
                if (resourceManager.GetFuel() <= 0 || resourceManager.GetStamina() <= 0)
                {
                    ChangeGameState(GameState.GameOver);
                }
            }
        }

        /// <summary>
        /// 检查是否胜利
        /// </summary>
        public void CheckVictory()
        {
            // 到达边境时调用
            ChangeGameState(GameState.Victory);
        }
    }

    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        InCar,      // 车内场景
        Escaping,   // 逃亡场景
        GameOver,   // 游戏失败
        Victory     // 游戏胜利
    }
}

