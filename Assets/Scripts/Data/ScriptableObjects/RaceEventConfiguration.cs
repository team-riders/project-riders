using UnityEngine;

[CreateAssetMenu(fileName = "RaceEvent", menuName = "Project Riders/New Race", order = 0)]
public class RaceEventConfiguration : ScriptableObject
{
    public int ID;
    public string RaceName;
    public RaceType RaceType;
    public int NumberOfLaps;
    public int NumberOfRacers;
    public MapConfiguration map;


}