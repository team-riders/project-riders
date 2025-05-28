using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using RidersRuntime.Input;
using RidersRuntime.VehicleSystem;
using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    public class RaceEventController
    {
        RaceEventConfiguration raceEvent;
        List<RacerComponent> racers;
        List<int> playerIndices;

        // timer Class here

        // Checkpoints class here
        private TrackCheckpoints checkpoints;

        // Pause controller here

        public void SetupRace(
            RaceEventConfiguration raceEvent,
            List<RacerComponent> racers, // We could spawn in RaceMeetController instead?
            List<int> playerIndices
            )
        {
            this.raceEvent = raceEvent;
            this.racers = racers;
            this.playerIndices = playerIndices;

            PrepareRace();
        }

        public void PrepareRace()
        {
            // Figure out the order of this 
            // NOTE: THE DREAM IS THAT RACERS ARE SPAWNED INTO A CINEMATIC SEQUENCE
            // Prepare the race transition sequence
            // Play the sequence

            // Turn off all of the racer inputs here
            foreach (RacerComponent racer in racers)
            {
                if (racer)
                {
                    // Disable the player input component
                    racer.GetComponent<RideMovementController>().canMove = false;
                }
            }

            checkpoints = new GameObject("TrackCheckpoints").AddComponent<TrackCheckpoints>();
            // Need to do a late bind here
            GameObject.Find("CheckpointEvent").GetComponent<CheckpointUIScript>().BindTrackCheckpoints(checkpoints);
            GameObject.FindFirstObjectByType<stopwatchScript>().BindTrackCheckpoints(checkpoints);
            checkpoints.SetupRacers(racers);
            checkpoints.SetupRace(raceEvent);

            LoadRacers();
        }

        public void LoadRacers()
        {
            // Find the spawn points in the map
            // Spawn the racers in the map

            // TODO: Use pooling for the racer instead of instantiating them every time
            // Turn off main camera for now
            Camera.main.gameObject.SetActive(false);

            foreach (RacerComponent racer in racers)
            {
                if (racer)
                {
                    // Enable the camera
                    racer.GetComponent<RidersRuntime.Input.PlayerInput>().GetPlayerInputComponent().GetComponent<UnityInputWrapper>().SetCameraMode(true);
                }
            }

            // For the time being, because we haven't actually implemented a proper countdown system, we need to start the race manually here
            // StartRace();
        }


        // The event scenario that should happen when the announcer says go
        public void StartRace()
        {
            // There might be a delay but anyways
            // Start the timer here

            // this.Timer.StartTimer();
            // Enable the checkpoint system

            // Enable the environment details (maybe do this in "load map" so that we can have a loading screen?)

            // Enable all racer inputs

            // Run these two in sequence
            // 1. Enable split the camera system
            // 2. Enable the UI LAST

            foreach (RacerComponent racer in racers)
            {
                if (racer.isPlayer)
                {
                    // Enable the player input component
                    racer.GetComponent<RideMovementController>().canMove = true;
                }
            }

            checkpoints.StartRace();
        }

        // Triggered by the checkpoint system or timer waiting for the last person to finish

        public void FinishSingleRacer(RacerComponent racer)
        {
            // Disable the racer input
            // Change to "passive" AI and disable some stuff.
        }

        public void FinishRace()
        {

            // Show the finish screen?
        }

        public void EndRace()
        {
            // Disable the checkpoint system
            // Disable the timer
            // Disable the camera system / go back to joined camera state
            // Disable the UI

            // Unload the map (or just disable it?)
            // Unload all racers (or just disable them?)
        }

        public void PauseRace(int playerIndex)
        {
        }
    }
}