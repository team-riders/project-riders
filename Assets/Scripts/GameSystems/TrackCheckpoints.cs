using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerIncorrectCheckpoint;
    public event EventHandler OnLapCompleted;
    private List<CheckpointSingle> checkpointSingleList;
    private int nextCheckpointSingleIndex;
    private float lapStartTime;
    private float currentLapTime;
    private bool isLapInProgress = false;

    private void Awake() {
        Transform checkpointsTransform = transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform) {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndex = 0;
    }

    public void StartNewLap()
    {
        lapStartTime = Time.time;
        isLapInProgress = true;
        nextCheckpointSingleIndex = 0;
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle) {
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex){
            // Get lap time
            currentLapTime = Time.time - lapStartTime;

            // Add to checkpoint count
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);

            // Start new lap if all checkpoints hit
            if (nextCheckpointSingleIndex == 0) {
                OnLapCompleted?.Invoke(this, EventArgs.Empty);
                StartNewLap();
            }
        } else {
            // Wrong way UI 
            OnPlayerIncorrectCheckpoint?.Invoke(this, EventArgs.Empty);
    
        }
    }
    public float GetCurrentLapTime() {
        return currentLapTime;
    }
}
