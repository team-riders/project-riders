using System;
using System.Collections.Generic;
using RidersRuntime.VehicleSystem;
using RidersRuntine.GameSystems;
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
    }

    public class TrackCheckpoints : MonoBehaviour
    {
        public event EventHandler OnPlayerCorrectCheckpoint;
        public event EventHandler OnPlayerIncorrectCheckpoint;
        public event EventHandler OnLapCompleted;
        private List<CheckpointSingle> checkpointSingleList;

        private Dictionary<int, RiderProgression> ridersProgression = new();

        private void Awake()
        {
            Transform checkpointsTransform = transform.Find("Checkpoints");

            checkpointSingleList = new List<CheckpointSingle>();

            foreach (Transform checkpointSingleTransform in checkpointsTransform)
            {
                CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
                checkpointSingle.Setup(OnVehicleThroughCheckpoint);
                checkpointSingleList.Add(checkpointSingle);
            }
        }

        // Grabs all the racers information here
        public void SetupRacers()
        {
            foreach (var vehicle in new List<GameObject>())
            {
                ridersProgression.Add(vehicle.GetInstanceID(), new RiderProgression());
            }
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
            if (vehicle.layer != LayerMask.NameToLayer("Vehicle"))
            {
                return;
            }

            // TODO: Change script to use rider information explicitly
            if (!vehicle.TryGetComponent(out BaseVehicle baseVehicle))
            {
                return;
            }

            // Check if the checkpoint is part of the system
            if (checkpointSingleList.Contains(checkpointSingle))
            {
                int vehicleId = vehicle.GetInstanceID();
                if (!ridersProgression.ContainsKey(vehicleId))
                {
                    Debug.LogError($"Vehicle {vehicle.name} is not registered in the checkpoint system.");
                    return;
                }
                RiderProgression progress = ridersProgression[vehicleId];

                // ! WE CAN'T USE TIME.TIME BECAUSE OF PAUSING
                progress.CurrentLapTime = Time.time - progress.LapStartTime;
                progress.LapStartTime = Time.time;
                progress.NextCheckpointSingleIndex = checkpointSingleList.IndexOf(checkpointSingle);
                PlayerThroughCheckpoint(checkpointSingle);
            }
        }

        public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
        {
            // if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
            // {
            //     // Get lap time
            //     currentLapTime = Time.time - lapStartTime;

            //     // Add to checkpoint count
            //     nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            //     OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);

            //     // Start new lap if all checkpoints hit
            //     if (nextCheckpointSingleIndex == 0)
            //     {
            //         OnLapCompleted?.Invoke(this, EventArgs.Empty);
            //         StartNewLap();
            //     }
            // }
            // else
            // {
            //     // Wrong way UI 
            //     OnPlayerIncorrectCheckpoint?.Invoke(this, EventArgs.Empty);

            // }
        }

        public float GetCurrentLapTime()
        {
            return ridersProgression[0].CurrentLapTime;
        }
    }
}