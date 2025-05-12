using System.Collections.Generic;
using RidersRuntime.Data;
using RidersRuntime.RaceManager;
using UnityEngine;

namespace RidersRuntime.GameSystems
{
    public class RaceHostController : MonoBehaviour
    {
        // This class manages the dispatching of the race and awaits the character selections

        RaceMeetConfiguration sessionMeet;

        RaceMeetController sessionRaceMeetController;

        void Start()
        {

            DontDestroyOnLoad(this);
        }

        public void RequestRaceSetup(RaceMeetConfiguration meetConfiguration)
        {
            if (sessionMeet != null)
            {
                Debug.LogError("A race is already in progress.");
                return;
            }
            sessionMeet = meetConfiguration;
            // This method will be called when a race type has been selected
            // For the time being there will be just a single player

            if (GetComponent<MultiDeviceControllerSystem>().GetPlayerCount() == 0)
            {
                Debug.LogError("Please ensure that there is at least one player in the game");
                return;
            }

            // ------ SCENE TRANSITION -------
            // await the scene transition to complete

            // when complete, then we need to pass the meet configuration to the character selection manager

            CharacterSelectionManager selector = FindFirstObjectByType<CharacterSelectionManager>();
            selector.transform.SetParent(transform);
            selector.GrabHostInformation(meetConfiguration, OnRaceMeetSetupComplete);

        }

        public void OnRaceMeetSetupComplete(List<RiderSelection> selectedRiders)
        {
            // This method will be called when the character selection is complete
            // Check with the number of players to see if we need to add any AI riders

            // Check if we already have a race session
            if (sessionRaceMeetController != null)
            {
                Debug.LogError("A race session has already been started.");
                return;
            }

            int playerCount = selectedRiders.Count;
            int aiCount = sessionMeet.NumberOfRacers - playerCount;

            List<RiderSelection> allRiders = new(selectedRiders);

            for (int i = 0; i < aiCount; i++)
            {
                RiderSelection aiRider = CreateUniqueBotRider(allRiders);
                allRiders.Add(aiRider);
            }

            GameObject obj = new GameObject("RaceMeetController");
            obj.transform.SetParent(transform);
            sessionRaceMeetController = obj.AddComponent<RaceMeetController>();

            sessionRaceMeetController.AssignFields(sessionMeet, allRiders, OnRaceMeetComplete);
            sessionRaceMeetController.StartNewMeetSession();
        }

        public void OnRaceMeetComplete(object obj)
        {

        }

        public static RiderSelection CreateUniqueBotRider(List<RiderSelection> alreadySelectedRiders)
        {
            // This method will create a unique bot rider that is not already selected by the players

            // Get a random rider from the list of available riders

            RiderConfig[] obj = Resources.LoadAll<RiderConfig>(Paths.Riders);
            // Why bother choosing random, just pick the first one LMAO
            RiderConfig randomRider = obj[0];
            // If the rider is not already selected, return it as a new RiderSelection
            return new RiderSelection
            {
                rider = randomRider,
                vehicleType = randomRider.racerInformation.defaultVehicleType,
                isPlayer = false
            };
        }
    }
}