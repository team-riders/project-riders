using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using RidersRuntime.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RidersRuntime.RaceManager
{
    // This DOES NOT NEED TO BE A MONOBEHAVIOUR
    public class RaceMeetController : MonoBehaviour
    {
        public List<RiderSelection> racers = new();
        public RaceMeetConfiguration raceMeet;

        // Default to 0 for now (players will be generated in order of player index)
        // Chances are 0 = 0, 1 = 1, 2 = 2, 3 = 3
        public List<int> playerIndexToRacerIndex = new();

        int currentRaceIndex = 0;

        // Reference to the current RaceEventController
        RaceEventController currentRaceEventController;

        Action<object> onRaceComplete;

        List<RacerComponent> pooledRacers = new();

        public void Start()
        {
            // SetupNextRace();
        }
        public void AssignFields(RaceMeetConfiguration meetConfiguration, List<RiderSelection> selectedRiders, Action<object> callback)
        {
            raceMeet = meetConfiguration;
            racers = selectedRiders;

            // Setup the player index to racer index mapping
            for (int i = 0; i < racers.Count; i++)
            {
                if (racers[i].isPlayer)
                {
                    playerIndexToRacerIndex.Add(i);
                }
            }

            onRaceComplete = callback;
        }

        public async Task StartNewMeetSession()
        {
            // ------ SCENE TRANSITION -------
            // await the scene transition to complete
            // Grab the first map scene
            MapConfiguration mapConfiguration = raceMeet.raceEvents[currentRaceIndex].map;
            string map = mapConfiguration.ScenePath;
            int index = SceneLoader.GetBuildIndexByPath(map);
            await SceneLoader.PrepareScene(index, onLoad: SetupRace);
        }

        public void GotoNextRace()
        {
            currentRaceIndex++;
            if (currentRaceIndex >= raceMeet.raceEvents.Count)
            {
                return;
            }

            SetupRace();
        }

        void SetupRace()
        {
            currentRaceEventController ??= new RaceEventController();

            LoadOrSpawnRacers(racers, playerIndexToRacerIndex);

            currentRaceEventController.SetupRace(
                raceMeet.raceEvents[currentRaceIndex],
                pooledRacers,
                playerIndexToRacerIndex
            );
        }

        public List<RacerComponent> LoadOrSpawnRacers(List<RiderSelection> racers, List<int> playerIndices)
        {
            if (pooledRacers.Count == racers.Count) return pooledRacers;

            for (int i = 0; i < racers.Count; i++)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerSet");
                GameObject obj = Instantiate(prefab);
                RacerComponent rc = obj.GetComponentInChildren<RacerComponent>();
                rc.SetupRider(racers[i]);

                pooledRacers.Add(rc);

                // Handle player bindings here

                List<RiderSelection> strayPlayers = new();

                if (racers[i].isPlayer)
                {
                    if (playerIndices.Contains(i))
                    {
                        // We need to find the player object in the scene that matches the player index

                        MultiDeviceControllerSystem mdcs = FindFirstObjectByType<MultiDeviceControllerSystem>();
                        UnityEngine.InputSystem.PlayerInput playerInput = mdcs.GetPlayerByPlayerIndex(i);

                        if (playerInput == null)
                        {
                            strayPlayers.Add(racers[i]);
                            continue;
                        }
                        if (playerInput.GetComponentInChildren<RacerComponent>())
                        {
                            Debug.LogError($"Player {i} already has a racer assigned to it. This is not allowed.");
                            strayPlayers.Add(racers[i]);
                            continue;
                        }
                        // Assign the player input to the racer

                        obj.transform.SetParent(playerInput.transform);
                        // Remove the object's player input component
                        UnityEngine.InputSystem.PlayerInput playerInputComponent = obj.GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>();
                        if (playerInputComponent != null)
                        {
                            Destroy(playerInputComponent);
                        }

                        playerInput.camera = obj.GetComponentInChildren<Camera>();

                    }
                }
            }

            return pooledRacers;
        }
    }
}