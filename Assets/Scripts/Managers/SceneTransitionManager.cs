using UnityEngine;
using UnityEngine.SceneManagement;

namespace XEscape.Managers
{
    /// <summary>
    /// 场景切换管理器
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        [Header("场景名称")]
        [SerializeField] private string carSceneName = "CarScene";
        [SerializeField] private string escapeSceneName = "EscapeScene";

        /// <summary>
        /// 切换到车内场景
        /// </summary>
        public void LoadCarScene()
        {
            SceneManager.LoadScene(carSceneName);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeGameState(GameState.InCar);
            }
        }

        /// <summary>
        /// 切换到逃亡场景
        /// </summary>
        public void LoadEscapeScene()
        {
            SceneManager.LoadScene(escapeSceneName);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeGameState(GameState.Escaping);
                if (GameManager.Instance.resourceManager != null)
                {
                    GameManager.Instance.resourceManager.StartConsumingResources();
                }
            }
        }
    }
}

