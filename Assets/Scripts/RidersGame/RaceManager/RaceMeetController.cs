using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.RaceManager
{
    public class RaceMeetController : MonoBehaviour
    {
        public List<RiderSelection> racers = new();
        public RaceMeetConfiguration raceMeet;

        public List<int> playerIndexToRacerIndex = new();

        private int currentRaceIndex = 0;
        private RaceEventController currentRaceEventController;
        private Action<object> onRaceComplete;

        private List<RacerComponent> pooledRacers = new();

        public void Start()
        {
            // SetupNextRace(); — called externally
        }

        public void AssignFields(RaceMeetConfiguration meetConfiguration, List<RiderSelection> selectedRiders, Action<object> callback)
        {
            raceMeet = meetConfiguration;
            racers = selectedRiders;

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
            MapConfiguration mapConfiguration = raceMeet.raceEvents[currentRaceIndex].map;
            string map = mapConfiguration.ScenePath;
            int index = SceneLoader.GetBuildIndexByPath(map);
            await SceneLoader.PrepareScene(index, onLoad: SetupRace);
        }

        public void GotoNextRace()
        {
            currentRaceIndex++;

            if (currentRaceIndex >= raceMeet.raceEvents.Count)
                return;

            SetupRace();
        }

        private void SetupRace()
        {
            currentRaceEventController ??= new RaceEventController();

            LoadOrSpawnRacers(racers, playerIndexToRacerIndex);

            currentRaceEventController.SetupRace(
                raceMeet.raceEvents[currentRaceIndex],
                pooledRacers,
                playerIndexToRacerIndex
            );

            RaceUIController raceUIController = FindFirstObjectByType<RaceUIController>();
            if (raceUIController != null)
            {
                raceUIController.ShowHUD();
            }
            else
            {
                Debug.LogWarning("[RaceMeetController] RaceUIController not found. Unable to show HUD.");
            }
        }

        public List<RacerComponent> LoadOrSpawnRacers(List<RiderSelection> racers, List<int> playerIndices)
        {
            if (pooledRacers.Count == racers.Count)
                return pooledRacers;

            for (int i = 0; i < racers.Count; i++)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerSet");
                GameObject obj = Instantiate(prefab);

                RacerComponent rc = obj.GetComponentInChildren<RacerComponent>();
                rc.SetupRider(racers[i]);
                pooledRacers.Add(rc);

                List<RiderSelection> strayPlayers = new();

                if (racers[i].isPlayer && playerIndices.Contains(i))
                {
                    MultiDeviceControllerSystem mdcs = FindFirstObjectByType<MultiDeviceControllerSystem>();
                    PlayerInput playerInput = PlayerInput.GetPlayerByIndex(i);

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

                    PlayerInput playerInputComponent = obj.GetComponentInChildren<PlayerInput>();
                    if (playerInputComponent != null)
                    {
                        Destroy(playerInputComponent);
                    }

                    obj.GetComponentInChildren<RidersRuntime.Input.PlayerInput>().BindPlayerInput(playerInput);
                    playerInput.camera = obj.GetComponentInChildren<Camera>();
                }
            }

            return pooledRacers;
        }
    }
}
