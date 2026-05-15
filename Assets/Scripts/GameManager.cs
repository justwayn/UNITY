using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isGameOver = false;

    [Header("UI References")]
    public GameObject gameOverPanel; // Assign a UI panel in the inspector

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

    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over: " + reason);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // You can add more logic here, like stopping time or disabling input
        // Time.timeScale = 0; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
