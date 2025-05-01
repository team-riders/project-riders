using RidersRuntime.Input;
using System;
using System.Collections.Generic;

namespace RidersRuntime.Analytics
{
    [Serializable]
    public struct InputFrameRecord
    {
        public int Frame;
        public ActorInputData Input;

        public InputFrameRecord(int frame, ActorInputData input)
        {
            Frame = frame;
            Input = input;
        }
    }

    // For JSON serialisation
    [Serializable]
    public class InputHistoryData
    {
        public List<InputFrameRecord> inputHistory = new();
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