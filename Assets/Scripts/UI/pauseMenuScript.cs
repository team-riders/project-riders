using UnityEngine;

public class pauseMenuScript : MonoBehaviour
{
    public GameObject pauseGame;
    public bool isPaused = false;
    private static pauseMenuScript _instance;

    public static pauseMenuScript Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseGame.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }
}
