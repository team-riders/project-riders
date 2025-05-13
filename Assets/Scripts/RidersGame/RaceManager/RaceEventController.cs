using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.RaceManager
{
    public class RaceEventController
    {
        RaceEventConfiguration raceEvent;
        List<RacerComponent> racers;

        List<int> playerIndices;

        // timer Class here

        // Checkpoints class here

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
                if (racer.isPlayer)
                {
                    // Enable the camera
                    racer.gameObject.GetComponentInParent<RidersRuntime.Input.UnityInputWrapper>().SetCameraMode(true);

                }
            }
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
    }
}