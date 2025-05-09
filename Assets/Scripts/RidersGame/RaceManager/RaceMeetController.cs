using RidersRuntime.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.Data
{
    [Serializable]
    public class RiderSelection
    {
        public RiderConfig rider;
        public VehicleType vehicleType;
        public bool isPlayer; // We can move this out to playerIndexToRacer
    }
}

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

        int currentRaceIndex = -1;

        // Reference to the current RaceEventController
        RaceEventController currentRaceEventController;

        Action<object> onRaceComplete;

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

        public void StartNewMeetSession()
        {
            // ------ SCENE TRANSITION -------
            // await the scene transition to complete
        }

        public void SetupNextRace()
        {
            currentRaceIndex++;
            if (currentRaceIndex >= raceMeet.raceEvents.Count)
            {
                return;
            }

            currentRaceEventController ??= new RaceEventController();

            currentRaceEventController.SetupRace(
                raceMeet.raceEvents[currentRaceIndex],
                racers,
                playerIndexToRacerIndex
            );
        }
    }
}