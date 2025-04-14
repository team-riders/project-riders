using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
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

        foreach (int i in playerIndices)
        {
            SpawnRacer(racers[i]);
            spawnIndex++;
        }

        foreach (RiderSelection racer in racers)
        {
            // Spawn the AI here
            if (racer.isPlayer) continue; // Skip if it's a player
        }
    }

    public void SpawnRacer(RiderSelection racer)
    {
        GameObject playerAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RacerPrefab.prefab");
        GameObject player = GameObject.Instantiate(playerAsset, Vector3.zero, Quaternion.identity);

        RiderSelection rs = racer;
        GameObject racerPrefab = rs.rider.characterModelPrefab;

        GameObject visual = GameObject.Instantiate(racerPrefab, initialSpawn, Quaternion.identity, player.transform);
        RacerComponent rc = player.GetComponent<RacerComponent>();

        rc.rider = racer.rider;

        // We need to eventually disable the player input & physics here
    }
}
