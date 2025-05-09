using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using RidersRuntime.Data;
using RidersRuntime.RaceManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.GameSystems
{
    public class RaceHostController : MonoBehaviour
    {
        // This class manages the dispatching of the race and awaits the character selections

        PlayerInputManager pim;

        RaceMeetConfiguration sessionMeet;

        RaceMeetController sessionRaceMeetController;

        void Start()
        {
            // Singleton ahh



            DontDestroyOnLoad(this);
        }

        public void RequestRaceSetup(RaceMeetConfiguration meetConfiguration)
        {
            sessionMeet = meetConfiguration;
            // This method will be called when a race type has been selected
            // For the time being there will be just a single player

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

            int playerCount = selectedRiders.Count;
            int aiCount = sessionMeet.NumberOfRacers - playerCount;

            List<RiderSelection> allRiders = new List<RiderSelection>(selectedRiders);

            for (int i = 0; i < aiCount; i++)
            {
                RiderSelection aiRider = CreateUniqueBotRider(allRiders);
                allRiders.Add(aiRider);
            }

            // Rearrange the list

            for (int i = 0; i < playerCount; i++)
            {
                RiderSelection playerRider = allRiders[i];
                allRiders.RemoveAt(i);
                allRiders.Add(playerRider);
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