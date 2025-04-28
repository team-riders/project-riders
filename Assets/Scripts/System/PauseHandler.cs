using UnityEngine;

// Chances are the race manager will be controlling this class, which this will no longer need to be a monobehaviour
public class PauseHandler : MonoBehaviour
{
    // Singleton

    public static PauseHandler Instance { get; private set; }

    bool isPaused = false;

    public static bool IsPaused
    {
        get
        {
            CheckInstance();
            return Instance.isPaused;
        }
    }

    private void Setup()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
    }

    static void CheckInstance()
    {
        if (Instance == null)
        {
            Instance = new GameObject("PauseHandler", typeof(PauseHandler)).GetComponent<PauseHandler>();
            DontDestroyOnLoad(Instance.gameObject);
        }
    }

    public static void TogglePause()
    {
        CheckInstance();
        if (Instance.isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public static void PauseGame()
    {
        CheckInstance();
        Time.timeScale = 0f;
        Instance.isPaused = true;
    }

    public static void ResumeGame()
    {
        CheckInstance();
        Time.timeScale = 1f;
        Instance.isPaused = false;
    }
}
