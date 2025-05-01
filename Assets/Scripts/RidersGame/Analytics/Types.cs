using RidersRuntime.Input;
using System;
using System.Collections.Generic;

namespace RidersRuntime.Analytics
{
    [Serializable]
    public struct InputSnapshot
    {
        public int TimeMs;
        public ActorInputData Input;

        public InputSnapshot(int timeMs, ActorInputData input)
        {
            TimeMs = timeMs;
            Input = input;
        }
    }

    // For JSON serialisation
    [Serializable]
    public class InputHistoryData
    {
        public List<InputSnapshot> inputHistory = new();
    }

    public struct Paths
    {
        public const string Logs = "Logs/";
    }

    public struct FileNames
    {
        public const string InputCsv = "InputHistory.csv";
        public const string InputJson = "InputHistory.json";
    }
}