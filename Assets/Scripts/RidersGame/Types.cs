using System;

/** 
  * Try to keep UnityEngine references out of here. 
*/

namespace RidersRuntime.Data
{
    [Serializable]
    public struct RacerInformation
    {
        public int VehicleSelected;
        public int CharacterSelected;
        public bool IsPlayer;
    }

    public enum VehicleType
    {
        Skateboard,
        Blades,
        Bike
    }

    public struct Paths
    {
        public const string Config = "Config/";
    }

    public struct FileNames
    {
        public const string DebugFlags = "debug_flags.json";
    }

    public enum GameScene
    {
        TitleScreen = 0,
        MainMenu = 1,
        Loading = 2,
        Game = 3,
        TutorialArea = 4,
        FirstRaceInMelbourne = 5
        // Add more scenes as needed
    }

}