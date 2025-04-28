using UnityEngine;
using TMPro;

public class stopwatchScript : MonoBehaviour
{
    public GameObject CountdownCanvas;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownText;
    
    float elapsedTime;
    bool isRunning = false;
    float countdownTime = 5;
    bool countdownComplete = false;

    void Update()
    {
        // Handle countdown
        if (!countdownComplete)
        {
            countdownTime -= Time.deltaTime;
            
            if (countdownTime > 1)
            {
                countdownText.text = Mathf.FloorToInt(countdownTime).ToString();
            }
            else if (countdownTime > 0)
            {
                countdownText.text = "GO!";
            }
            else
            {
                countdownText.text = "";
                CountdownCanvas.SetActive(false);
                countdownComplete = true;
                startTimer();   
            }
            return; 
        }

        // Handle stopwatch
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            displayTimer();
        }
    }

    public void startTimer()
    {
        elapsedTime = 0f; // Reset timer
        isRunning = true;
    }

    public void displayTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
        timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void stopTimer()
    {
        isRunning = false;
    }
}