using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceMeet", menuName = "Project Riders/New Event Meet")]
public class RaceMeetConfiguration : ScriptableObject
{
    public int ID;
    public string Name_Meet;
    public List<RaceEventConfiguration> raceEvents = new List<RaceEventConfiguration>();
}