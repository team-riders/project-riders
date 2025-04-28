using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class stopwatchScript : MonoBehaviour
{
    public GameObject CountdownCanvas;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownText;
    float elapsedTime;
    bool isRunning = false;
    GameObject Start;
    float countdownTime = 5;

    Stopwatch stopwatch = new Stopwatch();

    void Start()
    {
        stopwatch.Start();
    }

    void Update()
    {
        if (Start == null)
        {
            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                int seconds = Mathf.FloorToInt(Mathf.Max(1, countdownTime - 1) % 60);
                countdownText.text = string.Format("{0}", seconds);
                if (countdownTime <= 1)
                {
                    countdownText.text = string.Format("GO!");
                    if (countdownTime <= 0)
                    {
                        CountdownCanvas.SetActive(false);
                    }
                }
                
            }
            startTimer();
        }
        if (isRunning) 
        {
            timerFunction();
        }
        displayTimer();
    }

    public void startTimer()
    {
        isRunning = true;
    }

    public void timerFunction()
    {
        elapsedTime += Time.deltaTime;
    }

    public void displayTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt(elapsedTime * 1000) % 1000;
        timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void stopTimer()
    {
        isRunning = false;
    }
}