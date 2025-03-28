using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class pauseMenuScript : MonoBehaviour
{
    public GameObject pauseGame;
    public bool isPaused = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                resumeGameFunction();
            }
            else
            {
                pauseGameFunction();
            }
        }
    }
    public void pauseGameFunction()
    {
        pauseGame.SetActive(true);
        Time.timeScale = 0f;    
        isPaused = true;
    }

    public void resumeGameFunction()
    {
        pauseGame.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
