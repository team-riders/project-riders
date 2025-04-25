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
    [System.Serializable]
    public class InputHistoryData
    {
        public List<InputFrameRecord> inputHistory = new();
    }
}