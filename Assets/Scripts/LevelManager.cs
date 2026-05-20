using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    public string nextSceneName = "City";
    public float transitionDelay = 2f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isTransitioning) return;

        // Check for enemies
        if (AreAllObjectivesEliminated())
        {
            StartCoroutine(TransitionToNextLevel());
        }
    }

    bool AreAllObjectivesEliminated()
    {
        if (CountActive<SoldierAI>() > 0) return false;
        if (CountActive<CrystalEnemyAI>() > 0) return false;
        if (CountActive<TentacleBossAI>() > 0) return false;
        if (CountActive<OwlAI>() > 0) return false;
        
        foreach (var enemy in Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy.enabled && enemy.name.ToLower().Contains("bear")) return false;
        }

        return true;
    }

    int CountActive<T>() where T : MonoBehaviour
    {
        int count = 0;
        foreach (var obj in Object.FindObjectsByType<T>(FindObjectsSortMode.None))
        {
            if (obj.enabled) count++;
        }
        return count;
    }

    IEnumerator TransitionToNextLevel()
    {
        // Safety: Wait a bit to ensure enemies are actually destroyed (Destroy delay)
        yield return new WaitForSeconds(0.5f);
        if (!AreAllObjectivesEliminated()) yield break;

        isTransitioning = true;
        Debug.Log("All objectives met! Moving to next state.");
        
        yield return new WaitForSeconds(transitionDelay);

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.CompleteCurrentState();
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
