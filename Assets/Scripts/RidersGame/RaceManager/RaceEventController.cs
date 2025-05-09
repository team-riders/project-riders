using RidersRuntime.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.RaceManager
{
    public class RaceEventController
    {
        RaceEventConfiguration raceEvent;
        List<RiderSelection> racers;

        List<int> playerIndices;

        // timer Class here

        // Checkpoints class here

        [Header("Move these to the map")]
        public Vector3 initialSpawn = new(0, 0, 0);
        public Vector3 incrementalSpawn = new(5, 0, 0);

        public List<GameObject> pooledGameObjects = new();

        int spawnIndex = 0;


        public void SetupRace(
            RaceEventConfiguration raceEvent,
            List<RiderSelection> racers, // We could spawn in RaceMeetController instead?
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
            // Load the map (try use additive instead)
            // Load the racers into the map
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


            foreach (int i in playerIndices)
            {
                SpawnRacer(racers[i]);
                spawnIndex++;
            }

            for (int i = 0; i < racers.Count; i++)
            {
                if (playerIndices.Contains(i)) continue; // Skip if it's a player
                SpawnRacer(racers[i]);
                spawnIndex++;
            }
        }

        public void SpawnRacer(RiderSelection racer)
        {
            GameObject playerAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RacerPrefab.prefab");
            GameObject player = GameObject.Instantiate(playerAsset, initialSpawn + incrementalSpawn * spawnIndex, Quaternion.identity);

            RiderSelection rs = racer;
            GameObject racerPrefab = rs.rider.characterModelPrefab;

            RacerComponent rc = player.GetComponent<RacerComponent>();
            rc.rider = racer.rider;

            // Setup the visuals in the racer component instead honestly
            GameObject visual = GameObject.Instantiate(racerPrefab, player.transform);


            // We need to eventually disable the player input & physics here
        }

        public void SpawnPlayerRacer(RiderSelection racer, int PlayerIndex)
        {

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