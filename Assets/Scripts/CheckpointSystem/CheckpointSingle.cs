using System;
using Assets.Scripts.VehicleSystems;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<BoardVehicle>(out BoardVehicle player)) {
            trackCheckpoints.PlayerThroughCheckpoint(this);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }
}
