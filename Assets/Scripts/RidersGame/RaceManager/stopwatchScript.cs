using UnityEngine;
using TMPro;
using System;

namespace RidersRuntime.RaceManager
{
    public class stopwatchScript : MonoBehaviour
    {
        public GameObject CountdownCanvas;
        public GameObject LapTimeCanvas;
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI countdownText;
        [SerializeField] TextMeshProUGUI lapTimeText;
        [SerializeField] TextMeshProUGUI raceTimeText;

        float elapsedTime;
        bool isRunning = false;
        float countdownTime = 5;
        bool countdownComplete = false;
        public bool IsCountdownComplete => countdownComplete;
        public event Action OnCountdownComplete;
        

        // Subscribe to OnLapCompleted + OnRaceCompleted event on start
        private void Start()
        {

            TrackCheckpoints trackCheckpoints = FindFirstObjectByType<TrackCheckpoints>();
            BindTrackCheckpoints(trackCheckpoints);

            LapTimeCanvas.SetActive(false);
        }

        public void BindTrackCheckpoints(TrackCheckpoints trackCheckpoints)
        {
            if (trackCheckpoints != null)
            {
                trackCheckpoints.OnLapCompleted += OnLapCompleted;
                trackCheckpoints.OnRaceCompleted += OnRaceCompleted;
            }

            LapTimeCanvas.SetActive(false);
        }
        void Update()
        {
            // Pause
            // if (pauseMenuScript.Instance != null && pauseMenuScript.Instance.isPaused)
            //     return;

                // Add null checks at the start
            if (timerText == null || countdownText == null || lapTimeText == null || raceTimeText == null)
            {
                Debug.LogError("One or more text elements are not assigned!");
                return;
            }

            if (CountdownCanvas == null || LapTimeCanvas == null)
            {
                Debug.LogError("Canvas references not assigned!");
                return;
            }

            // Handle countdown
            if (!countdownComplete)
            {
                if (CountdownCanvas == null || countdownText == null)
                {
                    Debug.LogError("Countdown references not set!");
                    return;
                }

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
                    OnCountdownComplete?.Invoke();
                    GameObject.FindFirstObjectByType<RaceMeetController>().currentRaceEventController.StartRace();
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
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
            lapTimeText.text = string.Format("Lap Time: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

            LapTimeCanvas.SetActive(true);
            // Hide lap time after 3 seconds
            Invoke("HideLapTimeDisplay", 3f);
        }

        private void OnRaceCompleted(object sender, EventArgs e)
        {
            TrackCheckpoints trackCheckpoints = sender as TrackCheckpoints;
            if (trackCheckpoints != null)
            {
                float raceTime = trackCheckpoints.GetRaceTime();
                DisplayRaceTime(raceTime);
            }
        }
        private void DisplayRaceTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
            raceTimeText.text = string.Format("Race Time: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

            LapTimeCanvas.SetActive(true);
            raceTimeText.gameObject.SetActive(true);
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
            if (timerText == null) 
            {
                Debug.LogError("timerText is not assigned!");
                return;
            }
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