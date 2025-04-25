using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    private List<CheckpointSingle> checkpointSingleList;
    private int nextCheckpointSingleIndex;

    private void Awake() {
        Transform checkpointsTransform = transform.Find("Checkpoints");

        foreach (Transform checkpointSingleTransform in checkpointsTransform) {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndex = 0;
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle) {
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex){
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            // Add UI Element to show correct checkpoint hit
        } else {
            // Skipped Checkpoint (Add UI Element)
        }
    }
}
