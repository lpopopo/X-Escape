using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XEscape.Managers;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif

namespace XEscape.UI
{
    /// <summary>
    /// 游戏结束UI
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI元素")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Text gameOverText;
#if UNITY_TEXTMESHPRO
        [SerializeField] private TextMeshProUGUI gameOverTextTMP; // 支持TextMeshPro（可选）
#endif
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                // 监听游戏状态变化
                // 这里可以通过事件系统来实现
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }

            // 初始隐藏面板
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        /// <summary>
        /// 显示游戏失败界面
        /// </summary>
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            string gameOverMessage = "逃亡失败！";
            if (gameOverText != null)
            {
                gameOverText.text = gameOverMessage;
            }
#if UNITY_TEXTMESHPRO
            if (gameOverTextTMP != null)
            {
                gameOverTextTMP.text = gameOverMessage;
            }
#endif
        }

        /// <summary>
        /// 显示胜利界面
        /// </summary>
        public void ShowVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        private void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}

