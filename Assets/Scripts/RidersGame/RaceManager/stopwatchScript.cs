using UnityEngine;
using TMPro;
using System;

namespace RidersRuntime.RaceManager
{
    public class stopwatchScript : MonoBehaviour
    {
        public static bool IsCountdownComplete { get; private set; } = false;
        public GameObject CountdownCanvas;
        public GameObject LapTimeCanvas;
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI countdownText;
        [SerializeField] TextMeshProUGUI lapTimeText;

        float elapsedTime;
        bool isRunning = false;
        float countdownTime = 5;
        bool countdownComplete = false;

        // Subscribe to OnLapCompleted event on start
        private void Start()
        {

            TrackCheckpoints trackCheckpoints = FindFirstObjectByType<TrackCheckpoints>();
            if (trackCheckpoints != null)
            {
                trackCheckpoints.OnLapCompleted += OnLapCompleted;
            }

            LapTimeCanvas.SetActive(false);
        }
        void Update()
        {
            // Pause
            // if (pauseMenuScript.Instance != null && pauseMenuScript.Instance.isPaused)
            //     return;

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

                    // Call for new lap method
                    TrackCheckpoints trackCheckpoints = FindFirstObjectByType<TrackCheckpoints>();
                    if (trackCheckpoints != null)
                    {
                        // trackCheckpoints.StartNewLap();
                    }
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

        private void OnLapCompleted(object sender, EventArgs e)
        {
            TrackCheckpoints trackCheckpoints = sender as TrackCheckpoints;
            if (trackCheckpoints != null)
            {
                float lapTime = trackCheckpoints.GetCurrentLapTime();
                DisplayLapTime(lapTime);
            }
        }

        private void DisplayLapTime(float time)
        {
            return;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
            lapTimeText.text = string.Format("Lap Time: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

            LapTimeCanvas.SetActive(true);
            // Hide lap time after 3 seconds
            Invoke("HideLapTimeDisplay", 3f);
        }

        private void HideLapTimeDisplay()
        {
            LapTimeCanvas.SetActive(false);
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
}