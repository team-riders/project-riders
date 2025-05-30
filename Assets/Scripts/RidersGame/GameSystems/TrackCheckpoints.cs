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

        public RiderProgression()
        {
            NextCheckpointSingleIndex = 0;
            LapStartTime = 0f;
            CurrentLapTime = 0f;
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
        private List<CheckpointSingle> checkpointSingleList;

        public List<RacerComponent> racersList;

        private Dictionary<int, RiderProgression> ridersProgression = new();

        private void Start()
        {
            Transform checkpointsTransform = transform.Find("Checkpoints");

            checkpointSingleList = new List<CheckpointSingle>();

            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
                checkpointSingle.Setup(OnVehicleThroughCheckpoint);
                checkpointSingleList.Add(checkpointSingle);
            }

            SetupRacers(racersList);
        }

        // Grabs all the racers information here
        public void SetupRacers(List<RacerComponent> racers)
        {
            foreach (var racer in racers)
            {
                if (!ridersProgression.ContainsKey(racer.racerID))
                {
                    ridersProgression.Add(racer.racerID, new RiderProgression());
                }
            }

            StartRace();
        }

        public void StartRace()
        {
            foreach (var progress in ridersProgression.Values)
            {
                progress.LapStartTime = Time.time;
                progress.NextCheckpointSingleIndex = 0;
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
                    OnLapCompleted?.Invoke(this, EventArgs.Empty);
                    Debug.Log($"Lap completed by {racerId} in {progress.CurrentLapTime}");
                    progress.StartNewLap();
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
    }
}