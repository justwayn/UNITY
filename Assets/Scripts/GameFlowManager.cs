using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public enum GameState { MainMenu, ForestIntro, Forest, CityIntro, City, LabIntro, Laboratory, Ending }
    public GameState currentState = GameState.MainMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        currentState = GameState.ForestIntro;
        LoadCurrentState();
    }

    public void CompleteCurrentState()
    {
        switch (currentState)
        {
            case GameState.ForestIntro: currentState = GameState.Forest; break;
            case GameState.Forest: currentState = GameState.CityIntro; break;
            case GameState.CityIntro: currentState = GameState.City; break;
            case GameState.City: currentState = GameState.LabIntro; break;
            case GameState.LabIntro: currentState = GameState.Laboratory; break;
            case GameState.Laboratory: currentState = GameState.Ending; break;
            case GameState.Ending: currentState = GameState.MainMenu; break;
        }
        LoadCurrentState();
    }

    public void LoadCurrentState()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;
            case GameState.ForestIntro:
            case GameState.CityIntro:
            case GameState.LabIntro:
            case GameState.Ending:
                SceneManager.LoadScene("Cutscene");
                break;
            case GameState.Forest:
                SceneManager.LoadScene("SampleScene");
                break;
            case GameState.City:
                SceneManager.LoadScene("City");
                break;
            case GameState.Laboratory:
                SceneManager.LoadScene("Laboratory");
                break;
        }
    }
    
    public void QuitToMainMenu()
    {
        currentState = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
