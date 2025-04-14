using System;
using System.Collections.Generic;


[Serializable]
public enum RaceType
{
    TimeAttack,
    StandardRace,
    ScoreAttack,
    UrbanRally,
    HazardHeat,
    Freestyle
}
/// <summary>
/// A race meet consists of multiple race events
/// </summary>
[Serializable]
public class RaceMeet
{
    public int ID;
    public string Name;
    public int NumberOfRacers;
    public List<RaceEvent> RaceEvents;

    public RaceMeet(int id, string name, int NumberOfRacers, List<RaceEvent> raceEvents)
    {
        ID = id;
        Name = name;
        this.NumberOfRacers = NumberOfRacers;
        RaceEvents = raceEvents;
    }
}

[Serializable]
public class RaceEvent
{
    public int ID;
    public string RaceName;
    public RaceType RaceType;
    public int NumberOfLaps;
    // Move this up to race meet
    public MapConfiguration map;

    // constructor
    public RaceEvent(int id, string raceName, RaceType raceType, int numberOfLaps, MapConfiguration map)
    {
        ID = id;
        RaceName = raceName;
        RaceType = raceType;
        NumberOfLaps = numberOfLaps;
        this.map = map;
    }
}

[Serializable]
public struct Racer
{
    public int RiderID;
    public string RiderName;
    public int TeamID;
    public Enums.VehicleType defaultVehicleType;
    public int defaultRideSkin;
}

[Serializable]
public class MapDetails
{
    public int ID;
    public string Name;
    public string SceneName;
}
