using System.Collections.Generic;

namespace RidersCore.Data
{
    public enum VehicleType
    {
        Skateboard,
        Blades,
        Bike
    }

    public enum StuntButtons
    {
        StuntA,
        StuntB,
        StuntC
    }

    public struct StuntButtonNamesFull
    {
        public const string StuntA = "Player/StuntA";
        public const string StuntB = "Player/StuntB";
        public const string StuntC = "Player/StuntC";
    }

    public struct StuntButtonNamesShort
    {
        public const string StuntA = "StuntA";
        public const string StuntB = "StuntB";
        public const string StuntC = "StuntC";
        public readonly static string[] AllStuntButtons = { StuntA, StuntB, StuntC };
    }

    public struct InputNameSpecial
    {
        public const string JumpHoldDuration = "JumpHoldDuration";
    }

    public struct ButtonNamesFull
    {
        public const string Jump = "Player/Jump";
        public const string Accelerate = "Player/Accelerate";
        public const string Brake = "Player/Brake";
        public const string TurnInput = "Player/Horizontal";
        public const string Drift = "Player/Drift";
        public const string BoostRam = "Player/BoostRam";
        public const string PauseButton = "Player/PauseButton";
    }

    public struct ButtonNamesShort
    {
        public const string Jump = "Jump";
        public const string Accelerate = "Accelerate";
        public const string Brake = "Brake";
        public const string TurnInput = "Horizontal";
        public const string Drift = "Drift";
        public const string BoostRam = "Boost/Ram";
        public const string PauseButton = "PauseButton";
    };

    public struct InputMap
    {
        public const string Player = "Player";
        public const string UI = "UI";
        public const string Menu = "Menu";
        public const string Debug = "Debug";
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