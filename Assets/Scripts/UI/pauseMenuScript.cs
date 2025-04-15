using UnityEngine;

public class pauseMenuScript : MonoBehaviour
{
    public GameObject pauseGame;
    public bool isPaused = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseHandler.TogglePause();
            Debug.Log(PauseHandler.IsPaused);
        }
    }
}
