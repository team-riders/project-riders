using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class stopwatchScript : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime;

    Stopwatch stopwatch = new Stopwatch();

    void Start()
    {
        stopwatch.Start();
    }

    void Update()
    {
        if (PauseHandler.IsPaused)
        {
            stopwatch.Pause();
        }
        else
        {
            stopwatch.Resume();
        }

        if (!PauseHandler.IsPaused)
        {

            stopwatch.Update(Time.deltaTime);
        }
        elapsedTime = stopwatch.Now;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // External entity needs to rock up and pause the timer

}