using System.Collections.Generic;
using UnityEngine;

public struct RacerCheckpointProgress
{
    public int RacerID;
    public int CurrentLap;
    public int CurrentCheckpointIndex;
    public bool IsFinished;

    public static RacerCheckpointProgress none => new RacerCheckpointProgress
    {
        RacerID = -1,
        CurrentLap = -1,
        CurrentCheckpointIndex = -1,
        IsFinished = false
    };
}

public class CheckpointData : MonoBehaviour
{
    [Header("Fed from RaceManager")]
    public int NumberOfLaps = 3;
    [Tooltip("If true, you will need to provide checkpoints for each lap")]
    [Header("Checkpoint System")]
    public bool UseNonStandardCheckpointProgression = false;
    public List<Collider> checkpoints = new();

    // Work on a custom directional tree data structure for this.
    public string Path = "CheckpointPath";

    public List<RacerCheckpointProgress> racerCheckpointProgress = new();

    // Every checkpoint will get a function that will be called when a new racer enters it


    public void HandleRacerCheckpoint(int racerID, int checkpointIndex)
    {

    }

    public RacerCheckpointProgress GetRacerCheckpointProgress(int racerID)
    {
        return RacerCheckpointProgress.none;
    }
}