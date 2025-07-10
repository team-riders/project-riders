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
        Bike,
        None
    }

    public struct Paths
    {
        public const string Config = "Config/";
        public const string Riders = "Riders/";
        public const string ResourcesPrefabs = "Prefabs/";
    }

    public struct FileNames
    {
        public const string DebugFlags = "debug_flags.json";
        public const string RacerPrefab = "RacerPrefab";
    }

    public struct LayerNames
    {
        public const string Player = "Player";
        public const string Checkpoint = "Checkpoint";
        public const string Track = "Track";
        public const string Vehicle = "Vehicle";
        public const string Obstacle = "Obstacle";
        public const string FinishLine = "FinishLine";
    }

    public enum GameScene
    {
        TitleScreen = 13,
        MainMenu = 0,
        Loading = 1,
        // Add more scenes as needed
        CharacterSelect = 2,
    }

}