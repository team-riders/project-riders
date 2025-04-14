using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RiderSelection
{
    public RiderConfig rider;
    public Enums.VehicleType vehicleType;
    public bool isPlayer; // We can move this out to playerIndexToRacer
}
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

    public void Start()
    {
        SetupNextRace();
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