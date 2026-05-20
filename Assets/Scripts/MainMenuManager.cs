using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.StartNewGame();
        }
        else
        {
            // Fallback if no GameFlowManager is in scene (for testing)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Cutscene");
        }
    }

    public void LoadGame()
    {
        Debug.Log("Load Game clicked (Not implemented)");
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
