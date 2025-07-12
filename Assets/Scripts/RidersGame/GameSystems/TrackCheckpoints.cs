using System;
using System.Collections.Generic;
using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    public class RiderProgression
    {
        public int NextCheckpointSingleIndex { get; set; }
        public float LapStartTime { get; set; }
        public float CurrentLapTime { get; set; }
        public float RaceTime { get; set; }
        public int CurrentLap { get; set; }


        public RiderProgression()
        {
            NextCheckpointSingleIndex = 0;
            LapStartTime = 0f;
            CurrentLapTime = 0f;
            CurrentLap = 0;
            RaceTime = 0f;

        }

        public void StartNewLap()
        {
            LapStartTime = Time.time;
        }
    }

    public class TrackCheckpoints : MonoBehaviour
    {
        public event EventHandler OnPlayerCorrectCheckpoint;
        public event EventHandler OnPlayerIncorrectCheckpoint;
        public event EventHandler OnLapCompleted;
        public event EventHandler OnRaceCompleted;
        private List<CheckpointSingle> checkpointSingleList;
        private stopwatchScript _stopwatch;

        [SerializeField]
        private List<RacerComponent> racersList;
        private Dictionary<int, RiderProgression> ridersProgression = new();
        [SerializeField]
        private int NumberOfLaps = 3;

        public bool autoStart = false;
        private Dictionary<int, Rigidbody> riderRigidbodies = new Dictionary<int, Rigidbody>();
        private stopwatchScript _cachedStopwatch;

        private void Start()
        {
            Transform checkpointsTransform = GameObject.Find("Checkpoints").transform;

            checkpointSingleList = new List<CheckpointSingle>();

            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
                checkpointSingle.Setup(OnVehicleThroughCheckpoint);
                checkpointSingleList.Add(checkpointSingle);
            }

            Debug.Log($"Found {checkpointSingleList.Count} checkpoints in the scene.");

            if (autoStart)
            {
                SetupRacers(racersList);
                StartRace();
            }
        }

        private void Update()
        {
            var stopwatch = FindFirstObjectByType<stopwatchScript>();
            if (stopwatch != null && stopwatch.IsCountdownComplete)
            {
                FreezeAllRacers(false);
                // Optional: disable this Update check after unfreezing
                enabled = false;
            }
        }

        // Grabs all the racers information here
        public void SetupRacers(List<RacerComponent> racers)
        {
            if (racersList == null)
            {
                racersList = new List<RacerComponent>();
            }

            foreach (var racer in racers)
            {
                if (!ridersProgression.ContainsKey(racer.racerID))
                {
                    ridersProgression.Add(racer.racerID, new RiderProgression());

                    if (racer.TryGetComponent<Rigidbody>(out var rb))
                    {
                        riderRigidbodies[racer.racerID] = rb;
                    }
                }

                // Ensure that they are actually in the list of racers
                if (!racersList.Contains(racer))
                {
                    racersList.Add(racer);
                }
            }

            FreezeAllRacers(true);
        }

        public void SetupRace(RaceEventConfiguration eventConfiguration)
        {
            NumberOfLaps = eventConfiguration.NumberOfLaps;
        }

        public void StartRace()
        {

            var stopwatch = FindFirstObjectByType<stopwatchScript>();
            if (stopwatch == null || stopwatch.IsCountdownComplete)
            {
                FreezeAllRacers(false);
            }
            foreach (var progress in ridersProgression.Values)
            {
                progress.RaceTime = Time.time;
                progress.LapStartTime = Time.time;
                progress.NextCheckpointSingleIndex = 0;
                progress.CurrentLap = 1;
            }
        }

        public void OnVehicleThroughCheckpoint(CheckpointSingle checkpointSingle, GameObject vehicle)
        {
            if (vehicle.layer != LayerMask.NameToLayer(LayerNames.Vehicle))
            {
                return;
            }

            // TODO: Change script to use rider information explicitly
            if (!vehicle.TryGetComponent(out RacerComponent racer))
            {
                return;
            }

            // Check if the checkpoint is part of the system
            if (checkpointSingleList.Contains(checkpointSingle))
            {
                int racerId = racer.racerID;
                if (!ridersProgression.ContainsKey(racerId))
                {
                    Debug.LogError($"Vehicle {vehicle.name} is not registered in the checkpoint system.");
                    return;
                }
                RiderProgression progress = ridersProgression[racerId];

                // ! WE CAN'T USE TIME.TIME BECAUSE OF PAUSING

                PlayerThroughCheckpoint(racerId, checkpointSingle);
            }
        }

        public void PlayerThroughCheckpoint(int racerId, CheckpointSingle checkpointSingle)
        {
            RiderProgression progress = ridersProgression[racerId];

            if (checkpointSingleList.IndexOf(checkpointSingle) == progress.NextCheckpointSingleIndex)
            {
                // Get lap time
                progress.CurrentLapTime = Time.time - progress.LapStartTime;
                // Add to checkpoint count
                progress.NextCheckpointSingleIndex = (progress.NextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
                OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
                // Start new lap if all checkpoints hit
                if (progress.NextCheckpointSingleIndex == 0)
                {
                    progress.CurrentLap++;
                    OnLapCompleted?.Invoke(this, EventArgs.Empty);
                    Debug.Log($"Lap {progress.CurrentLap - 1} completed by Player {racerId} in {progress.CurrentLapTime}");
                    progress.StartNewLap();
                    if (progress.CurrentLap <= NumberOfLaps) // Assuming 3 laps total
                    {
                        progress.StartNewLap();
                    }
                    else
                    {
                        // Need to change so this event is triggered first so onlapcompleted doesnt trigger on race end
                        progress.RaceTime = Time.time - progress.RaceTime;
                        OnRaceCompleted?.Invoke(this, EventArgs.Empty);
                        Debug.Log($"Race completed by {racerId} in {progress.RaceTime}");
                        // FIX THIS ASAP
                        GameObject.FindFirstObjectByType<RaceMeetController>().currentRaceEventController.FinishSingleRacer(racersList[racerId]);
                    }
                }
            }
            else
            {
                // Wrong way UI 
                OnPlayerIncorrectCheckpoint?.Invoke(this, EventArgs.Empty);
            }

        }

        public float GetCurrentLapTime()
        {
            return ridersProgression[0].CurrentLapTime;
        }

        public float GetRaceTime()
        {
            return ridersProgression[0].RaceTime;
        }

        private void OnEnable()
        {
            _cachedStopwatch = FindFirstObjectByType<stopwatchScript>();
            if (_cachedStopwatch != null)
            {
                _cachedStopwatch.OnCountdownComplete += HandleCountdownComplete;
            }
        }

        private void OnDisable()
        {
            if (_cachedStopwatch != null)
            {
                _cachedStopwatch.OnCountdownComplete -= HandleCountdownComplete;
            }
        }

        private void HandleCountdownComplete()
        {
            FreezeAllRacers(false);
        }

        private void FreezeAllRacers(bool freeze)
        {
            foreach (var kvp in riderRigidbodies)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.isKinematic = freeze;
                    if (freeze)
                    {
                        kvp.Value.linearVelocity = Vector3.zero;
                        kvp.Value.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}