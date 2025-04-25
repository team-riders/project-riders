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
}